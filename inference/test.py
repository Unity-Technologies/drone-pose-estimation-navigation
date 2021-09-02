import torch
import torchvision


if __name__ == "__main__":
    model_file = "drone_target_pose_estimation_ep46.tar.zip"
    m = torch.load(model_file)
