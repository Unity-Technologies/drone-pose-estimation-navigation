import torch
import torchvision


def preload():
    '''pre-load VGG model weights, for transfer learning. Automatically cached for later use.'''
    torchvision.models.vgg16(pretrained=True)


class PoseEstimationNetwork(torch.nn.Module):
    """
    PoseEstimationNetwork: Neural network based on the VGG16 neural network
    architecture developed by Tobin at al. (https://arxiv.org/pdf/1703.06907.pdf). 
    The model is a little bit different from the original one
    but we still import the model as it has already been trained on a huge
    dataset (ImageNet) and even if we change a bit its architecture, the main
    body of it is unchanged and the weights of the final model will not be too
    far from the original one. We call this method "transfer learning".
    The network is composed by two branches: one for the translation
    (prediction of a 3 dimensional vector corresponding to x, y, z coordinates for 
    the drone and one for the target.
    """

    def __init__(self, scale_translation):
        super(PoseEstimationNetwork, self).__init__()
        self.model_backbone = torchvision.models.vgg16(pretrained=True) # uses cache
        # remove the original classifier
        self.model_backbone.classifier = torch.nn.Identity()

        # drone
        self.translation_block_drone = torch.nn.Sequential(
            torch.nn.Linear(25088, 256),
            torch.nn.ReLU(inplace=True),
            torch.nn.Linear(256, 64),
            torch.nn.ReLU(inplace=True),
            torch.nn.Linear(64, 3),
        )

        # target
        self.translation_block_cube = torch.nn.Sequential(
            torch.nn.Linear(25088, 256),
            torch.nn.ReLU(inplace=True),
            torch.nn.Linear(256, 64),
            torch.nn.ReLU(inplace=True),
            torch.nn.Linear(64, 3),
        )

        # scale factor on the translation 
        self.scale_translation = scale_translation

    def forward(self, x):
        x = self.model_backbone(x)
        output_translation_drone = self.translation_block_drone(x) * self.scale_translation
        output_translation_cube = self.translation_block_cube(x) * self.scale_translation

        return output_translation_drone, output_translation_cube
