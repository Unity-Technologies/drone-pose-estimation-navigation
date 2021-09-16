import copy
import os
import logging
from .logger import Logger
from .storage.checkpoint import EstimatorCheckpoint

from .model import PoseEstimationNetwork
from .train import train_model
from .evaluate import evaluate_model

import torch
import torchvision


class PoseEstimationEstimator:
    """
    This model is used on the SingleCube dataset.
    Its aim is to predict the position of the drone and the target.
    This class contains the method to train, evaluate, save and load the model.


    Attributes:
        config (dict): estimator config
        logger (Logger object form the class in logger.py): Log the performance
    """

    def __init__(self, *, config, logger, **kwargs):
        self.config = config
        self.data_root = config.system.data_root
        self.writer = Logger(
            log_dir=self.config.system.log_dir_system,
            config=config,
        )
        logger.info(f"writer log at : {config.system.log_dir_system}")

        self.checkpointer = EstimatorCheckpoint(
            estimator_name=self.config.estimator, log_dir=config.system.log_dir_system,
        )
        self.device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")

        # logging config
        self.logger = logger

        # We will create as many networks as there are objects to predict the position
        self.model = PoseEstimationNetwork(config.train.scale_translation)

        # load estimators from file if checkpoint_file_dir exists
        checkpoint_file_dir = config.checkpoint.checkpoint_file_dir
        if checkpoint_file_dir != "None":
            self.checkpointer.load(self, os.path.join(checkpoint_file_dir, config.checkpoint.checkpoint_file_name))

    def train(self):
        """
        This method is to train the PoseEstimationEstimator model
        """
        self.logger.info("Start training")
        train_model(self)

    def evaluate(self):
        """
        This method is to evaluate the PoseEstimationEstimator model
        """
        self.logger.info("Start evaluation")
        evaluate_model(self)

    def save(self, path):
        """Save all models into a directory

        Args:
            path (str): full path to save serialized estimator

        Returns:
            saved full path of the serialized estimator
        """
        save_dict = {
            "model": self.model.state_dict(),
            "config": self.config,
        }
        torch.save(save_dict, path)

    def load(self, path):
        """Load Estimator from path

        Args:
            path (str): full path to the serialized estimator
        """
        self.logger.info(f"loading checkpoint from file")
        checkpoint = torch.load(path, map_location=self.device)
        self.model.load_state_dict(checkpoint["model"])

        loaded_config = copy.deepcopy(checkpoint["config"])
        stored_config = copy.deepcopy(self.config)
        del stored_config["checkpoint"]["checkpoint_file_dir"]
        del stored_config["checkpoint"]["checkpoint_file_name"]
        if stored_config != loaded_config:
            self.logger.warning(
                f"Found difference in estimator config."
            )
