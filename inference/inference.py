# import torch
# import torchvision
#
# from Model.pose_estimation.drone_cube_dataset import DroneCubeDataset
# from Model.pose_estimation.model import PoseEstimationNetwork
#
#
# class Inference:
#     def __init__(self, ckpt, transforms, estimator, scale_translation=1):
#         self.device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")
#         ckpt = torch.load(ckpt, map_location=self.device)
#         if transforms is None:
#             self.transforms = self.get_default_transform()
#         else:
#             self.transforms = transforms
#             self.model = PoseEstimationNetwork(scale_translation)
#         self.model.load_state_dict(ckpt["model"])
#
#         config = estimator.config
#
#         dataset_test = DroneCubeDataset(
#             config=config,
#             data_root=estimator.data_root,
#             split="train",
#             zip_file_name=config.train.dataset_zip_file_name_training,
#         )
#
#         test_loader = torch.utils.data.DataLoader(
#             dataset_train,
#             batch_size=config.train.batch_training_size,
#             num_workers=0,
#             drop_last=True,
#         )
#         self.model.eval()
#
#     def get_default_transform(self):
#         """
#         Apply a transform on the input image tensor
#         Returns:
#             https://pytorch.org/docs/stable/torchvision/transforms.html
#         """
#         transform = torchvision.transforms.Compose(
#             [
#                 torchvision.transforms.Resize(
#                     (
#                         224,
#                         224,
#                     )
#                 ),
#                 torchvision.transforms.ToTensor(),
#             ]
#         )
#         return transform
#
#     def get_pose(self, image):
#         image = [self.transforms(image).unsqueeze(0)]
#         image = list(img.to(self.device) for img in image)
#         drone_t, cube_t = self.model(torch.stack(image).reshape(-1, 3, 224, 224))
#         drone_t = drone_t.detach().numpy()
#         cube_t = cube_t.detach().numpy()
#         return drone_t, cube_t
#
#
# def test_model(estimator):
#     config = estimator.config
#
#     dataset_test = DroneCubeDataset(
#         config=config,
#         split="test",
#         zip_file_name=config.test.dataset_zip_file_name_test,
#         data_root=config.system.data_root,
#     )