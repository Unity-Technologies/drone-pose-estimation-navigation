#!/usr/bin/env python3
# -*- coding: utf-8 -*-

import numpy as np
import torch 
import torchvision
import os
import sys
sys.path.append('../model/pose_estimation')
from PIL import Image, ImageOps

from model import PoseEstimationNetwork
from util import convert_image_to_tensor

def preprocess_image(path_image, device):
    """
    Pre-processing on the image 

    Args:
        path_image (str): path to the image that will be the input of the model 
        device (torch.device): device on which is the script is running 
    """
    image = convert_image_to_tensor(path_image, 224, 224)
    image = [image]
    image = list(img.to(device) for img in image)
    return image

global model
model = None

def run_model_main(image_data, image_width, image_height, model_file_name):
    global model

    device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")
    if model is None:
        checkpoint = torch.load(model_file_name, map_location=device)
        model = PoseEstimationNetwork(scale_translation=1)
        model.load_state_dict(checkpoint["model"])
        model = model.to(device)
        model.eval()

    image_path = _save_image(image_data)
    image = preprocess_image(image_path, device)
    output_translation_drone, output_translation_cube = model(torch.stack(image).reshape(-1, 3, 224, 224))
    output_translation_drone = output_translation_drone.to(device)
    output_translation_cube = output_translation_cube.to(device)
    output_translation_drone = output_translation_drone.detach().cpu().numpy()
    output_translation_cube = output_translation_cube.detach().cpu().numpy()
    return output_translation_drone, output_translation_cube


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
    image = Image.frombytes('RGBA', (640,480), image_data)
    image = ImageOps.flip(image)
    image_name = "Input" + str(count) + ".png"
    if not os.path.exists(PACKAGE_LOCATION + "/images/"):
        os.makedirs(PACKAGE_LOCATION + "/images/")
    image_path = PACKAGE_LOCATION + "/images/" + image_name
    image.save(image_path)
    return image_path
