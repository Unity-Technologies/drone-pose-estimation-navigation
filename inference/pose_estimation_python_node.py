#!/usr/bin/env python3
# -*- coding: utf-8 -*-

import numpy as np
import torch 
import torchvision
import os

from PIL import Image, ImageOps

from Model.pose_estimation.model import PoseEstimationNetwork


def preload():
    '''pre-load VGG model weights, for transfer learning. Automatically cached for later use.'''
    torchvision.models.vgg16(pretrained=True)

#
# class PoseEstimationNetwork(torch.nn.Module):
#     """
#     PoseEstimationNetwork: Neural network based on the VGG16 neural network
#     architecture developed by Tobin at al. (https://arxiv.org/pdf/1703.06907.pdf).
#     The model is a little bit different from the original one
#     but we still import the model as it has already been trained on a huge
#     dataset (ImageNet) and even if we change a bit its architecture, the main
#     body of it is unchanged and the weights of the final model will not be too
#     far from the original one. We call this method "transfer learning".
#     The network is composed by two branches: one for the translation
#     (prediction of a 3 dimensional vector corresponding to x, y, z coordinates for the drone and the target) and
#     one for the orientation (prediction of a 4 dimensional vector corresponding to
#     a quaternion for the drone and the target)
#     """
#
#     def __init__(self):
#         super(PoseEstimationNetwork, self).__init__()
#         self.model_backbone = torchvision.models.vgg16(pretrained=True) # uses cache
#         # remove the original classifier
#         self.model_backbone.classifier = torch.nn.Identity()
#
#         # drone
#         self.translation_block_drone = torch.nn.Sequential(
#             torch.nn.Linear(25088, 256),
#             torch.nn.ReLU(inplace=True),
#             torch.nn.Linear(256, 64),
#             torch.nn.ReLU(inplace=True),
#             torch.nn.Linear(64, 3),
#         )
#         self.orientation_block_drone = torch.nn.Sequential(
#             torch.nn.Linear(25088, 256),
#             torch.nn.ReLU(inplace=True),
#             torch.nn.Linear(256, 64),
#             torch.nn.ReLU(inplace=True),
#             torch.nn.Linear(64, 4),
#             LinearNormalized(),
#         )
#         # target
#         self.translation_block_cube = torch.nn.Sequential(
#             torch.nn.Linear(25088, 256),
#             torch.nn.ReLU(inplace=True),
#             torch.nn.Linear(256, 64),
#             torch.nn.ReLU(inplace=True),
#             torch.nn.Linear(64, 3),
#         )
#         self.orientation_block_cube = torch.nn.Sequential(
#             torch.nn.Linear(25088, 256),
#             torch.nn.ReLU(inplace=True),
#             torch.nn.Linear(256, 64),
#             torch.nn.ReLU(inplace=True),
#             torch.nn.Linear(64, 4),
#             LinearNormalized(),
#         )
#
#     def forward(self, x):
#         x = self.model_backbone(x)
#         output_translation_drone = self.translation_block_drone(x)
#         output_orientation_drone = self.orientation_block_drone(x)
#
#         output_translation_cube = self.translation_block_cube(x)
#         output_orientation_cube = self.orientation_block_cube(x)
#
#         return output_translation_drone, output_orientation_drone, output_translation_cube, output_orientation_cube


## We are not using quaternions anymore so we don't need this.
# class LinearNormalized(torch.nn.Module):
#     """
#     Custom activation function which normalizes the input.
#     It will be used to normalized the output of the orientation
#     branch in our model because a quaternion vector is a
#     normalized vector
#     """
#
#     def __init__(self):
#         super(LinearNormalized, self).__init__()
#
#     def forward(self, x):
#         return self._linear_normalized(x)
#
#     def _linear_normalized(self, x):
#         """
#         Activation function which normalizes an input
#         It will be used in the orientation network because
#         a quaternion is a normalized vector.
#         Args:
#             x (pytorch tensor with shape (batch_size, 4)): the input of the model
#         Returns:
#             a pytorch tensor normalized vector with shape(batch_size, 4)
#         """
#         norm = torch.norm(x, p=2, dim=1).unsqueeze(0)
#         for index in range(norm.shape[1]):
#             if norm[0, index].item() == 0.0:
#                 norm[0, index] = 1.0
#         x = torch.transpose(x, 0, 1)
#         x = torch.div(x, norm)
#         return torch.transpose(x, 0, 1)


def pre_process_image(path_image, device): #(image_data, image_width, image_height, device):
    """
    Pre-processing on the image 

    Args:
        image_data: row byte data 
        image_width (int): width of the image in Unity 
        image_height (int): height of the image in Unity 
        device (torch.device): device on which is the script is running 
    """

    image_origin = Image.open(path_image).convert("RGB")
    #image_origin = Image.frombytes('RGBA', (image_width,image_height), image_data)
    transform = get_transform()
    image = [transform(image_origin).unsqueeze(0)]
    image = list(img.to(device) for img in image)
    return image

def get_transform():
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

global model
model = None

def run_model_main(image_data, image_width, image_height, model_file_name):
    global model

    device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")
    if model is None:
        checkpoint = torch.load(model_file_name, map_location=device)
        model = PoseEstimationNetwork(scale_translation=1)
        model.load_state_dict(checkpoint["model"])
        model.eval()

    image_path = _save_image(image_data)
    image = pre_process_image(image_path, device)
    output_translation_drone, output_translation_cube = model(torch.stack(image).reshape(-1, 3, 224, 224))
    output_translation_drone = output_translation_drone.detach().numpy()
    output_translation_cube = output_translation_cube.detach().numpy()
    return output_translation_drone, output_translation_cube

def run_model_main_2(image_data, image_width, image_height, model_file_name):
    global model

    device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")
    if model is None:
        checkpoint = torch.load(model_file_name, map_location=device)
        model = PoseEstimationNetwork()
        model.load_state_dict(checkpoint["model"])
        model.eval()

    #image = pre_process_image(image_data, image_width, image_height, device)
    output_translation_drone, output_orientation_drone, output_translation_cube, output_orientation_cube = model(torch.stack(image_data).reshape(-1, 3, 224, 224))
    output_translation_drone, output_orientation_drone = output_translation_drone.detach().numpy(), output_orientation_drone.detach().numpy()
    output_translation_cube, output_orientation_cube = output_translation_cube.detach().numpy(), output_orientation_cube.detach().numpy()
    return output_translation_drone, output_orientation_drone, output_translation_cube, output_orientation_cube


count = 0
PACKAGE_LOCATION = "."

def _save_image(image_data):
    """  convert raw image data to a png and save it
    Args:
        req (PoseEstimationService msg): service request that contains the image data
     Returns:
        image_path (str): location of saved png file
    """
    global count
    count += 1
    #image_height = req.image.width
    #image_width = req.image.height
    image = Image.frombytes('RGBA', (1027,592), image_data)
    image = ImageOps.flip(image)
    image_name = "Input" + str(count) + ".png"
    if not os.path.exists(PACKAGE_LOCATION + "/images/"):
        os.makedirs(PACKAGE_LOCATION + "/images/")
    image_path = PACKAGE_LOCATION + "/images/" + image_name
    image.save(image_path)
    return image_path
