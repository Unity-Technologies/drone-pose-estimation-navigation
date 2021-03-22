from tensorboardX import SummaryWriter
import os


class Logger:
    def __init__(self, *, log_dir, config):
        self.writer = SummaryWriter(log_dir, write_to_disk=True)
        self.config = config

    def log_training(
        self,
        *,
        training_metric_translation_drone,
        training_metric_orientation_drone,
        training_metric_translation_cube,
        training_metric_orientation_cube,
        epoch,
    ):
        """
        Write the training translation and orientation error in the writer object to
        print it out on tensorboard for each epoch

        Args:
            training_metric_translation_drone (float): error on the translation for the drone calculated on one training epoch
            training_metric_orientation_drone (float): error on the orientation for the drone calculated on one training epoch
            training_metric_translation_cube (float): error on the translation for the cube calculated on one training epoch
            training_metric_orientation_cube (float): error on the orientation for the cube calculated on one training epoch
            epoch (int): number of the current epoch
        """
        # loss training
        self.writer.add_scalar(
            f"training/loss_translation_drone",
            training_metric_translation_drone,
            epoch,
        )

        self.writer.add_scalar(
            f"training/loss_orientation_drone",
            training_metric_orientation_drone,
            epoch,
        )
        self.writer.add_scalar(
            f"training/loss_translation_cube",
            training_metric_translation_cube,
            epoch,
        )

        self.writer.add_scalar(
            f"training/loss_orientation_cube",
            training_metric_orientation_cube,
            epoch,
        )

    def log_evaluation(
        self,
        *,
        evaluation_metric_translation_drone,
        evaluation_metric_orientation_drone,
        evaluation_metric_translation_cube,
        evaluation_metric_orientation_cube,
        epoch,
        test,
    ):
        """
        Write the evaluatin translation and orientation error in the writer object to
        print it out on tensorboard for each epoch. It is the validation translation and orientation
        error if we evaluate on the validation dataset and it is the test translation and orientation
        error if we evaluate it on the test set

        Args:
            evaluation_metric_translation_drone (float): error on the translation for the drone calculated on one validation epoch
            evaluation_metric_orientation_drone (float): error on the orientation for the drone calculated on one validation epoch
            evaluation_metric_translation_cube (float): error on the translation for the cube calculated on one validation epoch
            evaluation_metric_orientation_cube (float): error on the orientation for the cube calculated on one validation epoch
            epoch (int): number of the current epoch
            test (bool): specify if it is an evaluation in the training framework or in the test one.
        """

        if test:
            # loss test
            self.writer.add_scalar(
                f"test/loss_translation_drone", evaluation_metric_translation_drone
            )
            self.writer.add_scalar(
                f"test/loss_orientation_drone",
                evaluation_metric_orientation_drone,
            )
            self.writer.add_scalar(
                f"test/loss_translation_cube", evaluation_metric_translation_cube
            )
            self.writer.add_scalar(
                f"test/loss_orientation_cube",
                evaluation_metric_orientation_cube,
            )
        else:
            # loss validation
            self.writer.add_scalar(
                f"val/loss_translation_drone",
                evaluation_metric_translation_drone,
                epoch,
            )

            self.writer.add_scalar(
                f"val/loss_orientation_drone",
                evaluation_metric_orientation_drone,
                epoch,
            )
            self.writer.add_scalar(
                f"val/loss_translation_cube",
                evaluation_metric_translation_cube,
                epoch,
            )

            self.writer.add_scalar(
                f"val/loss_orientation_cube",
                evaluation_metric_orientation_cube,
                epoch,
            )

    def done(self):
        """
        Close the writer after the whole training + evaluation
        """
        self.writer.close()


# HELPER
def is_master():
    rank = int(os.getenv("RANK", 0))
    return rank == 0
