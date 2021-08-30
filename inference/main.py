import torch
import torchvision
import yaml
from easydict import EasyDict
from Model.pose_estimation.pose_estimation_estimator import PoseEstimationEstimator
from Model.pose_estimation.drone_cube_dataset import DroneCubeDataset
from Model.pose_estimation.model import PoseEstimationNetwork
from Model.pose_estimation.evaluate import evaluation_over_batch


class Inference:
    def __init__(self, ckpt, transforms=None, estimator=None, scale_translation=1):
        self.device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")
        self.device = "cpu"
        ckpt = torch.load(ckpt, map_location=self.device)
        if transforms is None:
            self.transforms = self.get_default_transform()
        else:
            self.transforms = transforms
        self.model = PoseEstimationNetwork(scale_translation)
        self.model.load_state_dict(ckpt["model"])
        self.estimator = estimator
        self.config = estimator.config
        self.criterion = torch.nn.MSELoss()

        dataset_test = DroneCubeDataset(
            config=self.config,
            data_root=estimator.data_root,
            split="train",
            zip_file_name=self.config.test.dataset_zip_file_name_test,
        )

        self.test_loader = torch.utils.data.DataLoader(
            dataset_test,
            batch_size=self.config.test.batch_test_size,
            num_workers=0,
            drop_last=False,
        )
        self.model.eval()

    def get_default_transform(self):
        """
        Apply a transform on the input image tensor
        Returns:
            https://pytorch.org/docs/stable/torchvision/transforms.html
        """
        transform = torchvision.transforms.Compose(
            [
                torchvision.transforms.Resize(
                    (
                        224,
                        224,
                    )
                ),
                torchvision.transforms.ToTensor(),
            ]
        )
        return transform

    def test(self):
        metric_translation_drone = 0
        metric_translation_cube = 0
        len_data_loader = len(self.test_loader)
        for index, (images, target_t_drone_list, target_t_cube_list) in enumerate(self.test_loader):
            images = list(image.to(self.device) for image in images)
            # T1
            output_t_drone, output_t_cube = self.model(
                torch.stack(images).reshape(-1, 3, 224, 224)
                )
            #T2
            #T2-T1
            target_t_drone = target_t_drone_list.to(self.device)
            target_t_cube = target_t_cube_list.to(self.device)
            print("wefwefwe")
            metric_translation_cube += self.criterion(output_t_cube, target_t_cube)
            metric_translation_drone += self.criterion(output_t_drone, target_t_drone)

            print(f"Cube loss : {metric_translation_cube}, Drone loss: {metric_translation_drone}")

            avg_loss = (metric_translation_drone + metric_translation_cube) / (index + 1)
            print(f"Avg Loss : {avg_loss}")

            

    def get_pose(self, image):
        image = [self.transforms(image).unsqueeze(0)]
        image = list(img.to(self.device) for img in image)
        drone_t, cube_t = self.model(torch.stack(image).reshape(-1, 3, 224, 224))
        drone_t = drone_t.detach().numpy()
        cube_t = cube_t.detach().numpy()
        return drone_t, cube_t


def get_config(filepath):
    f = open(filepath, 'r')
    config = yaml.load(f, Loader=yaml.FullLoader)
    config = EasyDict(config)
    return config

def test_model():
    config = get_config("/Users/souranil/ai/hackathon/ai-2021/ai-hw21-pose-estimation-model/Model/config-local.yml")
    estimator = PoseEstimationEstimator(config=config)
    transforms = torchvision.transforms.Compose(
            [
                torchvision.transforms.Resize(
                    (
                        224,
                        224,
                    )
                ),
                torchvision.transforms.ToTensor(),
            ]
        )

    i = Inference(
        "/Users/souranil/ai/hackathon/ai-2021/ai-hw21-pose-estimation-model/inference/drone_target_pose_estimation_ep50.tar",
        transforms=transforms,
        estimator=estimator
    )
    i.test()

if __name__ == "__main__":
    test_model()