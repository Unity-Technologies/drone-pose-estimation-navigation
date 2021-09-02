import torch
import torchvision

from train.pose_estimation.drone_cube_dataset import DroneCubeDataset
from train.pose_estimation.evaluation_metrics.translation_average_mean_square_error import (
    translation_average_mean_square_error,
)


def evaluate_model(estimator):
    """
    Do the evaluation process for the estimator

    Args:
        estimator: pose estimation estimator
    """
    config = estimator.config

    dataset_test = DroneCubeDataset(
        config=config,
        split="test",
        zip_file_name=config.test.dataset_zip_file_name_test,
        data_root=config.system.data_root,
    )

    estimator.logger.info("Start evaluating estimator: %s", type(estimator).__name__)

    test_loader = torch.utils.data.DataLoader(
        dataset_test,
        batch_size=config.test.batch_test_size,
        num_workers=0,
        drop_last=False,
    )

    estimator.model.to(estimator.device)
    evaluate_one_epoch(
        estimator=estimator,
        config=config,
        data_loader=test_loader,
        epoch=0,
        scale_translation=config.train.scale_translation,
        test=True,
    )


def evaluate_one_epoch(*, estimator, config, data_loader, epoch, scale_translation, test):
    """Evaluation of the model on one epoch
    Args:
        estimator: pose estimation estimator
        config: configuration of the model
        data_loader (DataLoader): pytorch dataloader
        epoch (int): the current epoch number
        scale_translation (float): scale factor we apply on the translation
        test (bool): specifies which type of evaluation we are doing
    """
    estimator.model.eval()
    estimator.logger.info(f" evaluation started")

    if test:
        batch_size = config.test.batch_test_size
    elif test == False:
        batch_size = config.val.batch_validation_size
    else:
        raise ValueError(f"You need to specify a boolean value for the test argument")

    number_batches = len(data_loader) / batch_size
    with torch.no_grad():
        metric_translation_drone, metric_translation_cube = evaluation_over_batch(
            estimator=estimator,
            config=config,
            data_loader=data_loader,
            batch_size=batch_size,
            epoch=epoch,
            scale_translation=scale_translation,
            is_training=False,
        )

        estimator.writer.log_evaluation(
            evaluation_metric_translation_drone=metric_translation_drone,
            evaluation_metric_translation_cube=metric_translation_cube,
            epoch=epoch,
            test=test,
        )


# HELPER
def evaluation_over_batch(
    *,
    estimator,
    config,
    data_loader,
    batch_size,
    epoch,
    scale_translation,
    is_training=True,
    optimizer=None,
    criterion_translation=None,
):
    """
    Do the training process for the drone and the cube 

    Args:
        estimator: pose estimation estimator
        config: configuration of the model
        data_loader (DataLoader): pytorch dataloader
        batch_size (int): size of the batch
        epoch (int): the current epoch number
        scale_translation (float): scale factor we apply on the translation
        is_training (bool): boolean to say if we are in a training process or not
        optimizer: optimizer of the model
        criterion_translation (torch.nn): criterion for the evaluation of the translation loss
    """
    
    len_data_loader = len(data_loader)
    metric_translation_drone = 0
    metric_translation_cube = 0
    print(f"Evaluation over batch, len: {len(data_loader)}")
    for index, (images, target_trans_drone_list, target_trans_cube_list) in enumerate(data_loader):
        images = list(image.to(estimator.device) for image in images)

        loss_translation_drone = 0
        loss_translation_cube = 0

        output_translation_drone, output_translation_cube = estimator.model(
            torch.stack(images).reshape(
                -1, 3, config.dataset.image_scale, config.dataset.image_scale
            )
        )
        
        # translation_filter = torch.LongTensor([0, 1]) # Only taking x, y translation

        target_translation_drone = target_trans_drone_list.to(estimator.device)
        target_translation_cube = target_trans_cube_list.to(estimator.device)

        # target_translation_drone = target_translation_drone[:, translation_filter]
        # target_translation_cube = target_translation_cube[:, translation_filter]
        
        metric_translation_drone += translation_average_mean_square_error(
            output_translation_drone, target_translation_drone
        )              
        metric_translation_cube += translation_average_mean_square_error(
            output_translation_cube, target_translation_cube
        )
       

        estimator.logger.info(f"INTERMEDIATE::: drone_loss: {metric_translation_drone / (index + 1)}, cube_loss: {metric_translation_cube / (index + 1)}, index : {index} \n Divided by {index+1}")

        intermediate_mean_loss_translation = (metric_translation_drone + metric_translation_cube)/ (index + 1)
        
        estimator.logger.info(
            f"intermediate mean translation loss after mini batch {index + 1} in epoch {epoch} is: {intermediate_mean_loss_translation}"
        )

               
        if is_training:
            # we change the scale of the translation in order to avoid exploding gradients

            # estimator.logger.info(f"PRE SCALE ::: t_drone: {output_translation_drone[0]}, t_cube: {output_translation_cube[0]}")

            output_translation_drone /= scale_translation
            output_translation_cube /= scale_translation

            # estimator.logger.info(f"POST SCALE ::: t_drone: {output_translation_drone[0]}, t_cube: {output_translation_cube[0]}")

            loss_translation_drone += criterion_translation(
                output_translation_drone, target_translation_drone
            )

            # estimator.logger.info(f"Drone | predicted: {output_translation_drone[0]}, target: {target_translation_drone[0]}")

            
            loss_translation_cube += criterion_translation(
                output_translation_cube, target_translation_cube
            )

            # estimator.logger.info(f"Cube | predicted: {output_translation_cube[0]}, target: {target_translation_cube[0]}")

            estimator.logger.info(f"LOSS : drone_loss: {loss_translation_drone}, cube_loss: {loss_translation_cube}, sum : {loss_translation_drone.item() + loss_translation_cube.item()}")
                   
            train_loss = (
                loss_translation_drone + loss_translation_cube
            )

            estimator.logger.info(f"LOSS : {train_loss.item()} vs {train_loss}, Loss shape: {train_loss.shape}")


            train_loss.backward()
            # print(f"Index: {index}, accumulation step : {config.train.accumulation_steps}")
            if (index + 1) % config.train.accumulation_steps == 0:
                optimizer.step()
                print("Stepping..")
                optimizer.zero_grad()

    estimator.logger.info(f"Done with epoch - {epoch}")
    metric_translation_drone = metric_translation_drone / len_data_loader
    metric_translation_cube = metric_translation_cube / len_data_loader

    return metric_translation_drone, metric_translation_cube
