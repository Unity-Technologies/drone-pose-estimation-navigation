import torch
import os
from pathlib import Path

from .storage.gcs import download_file_from_gcs, copy_file_to_gcs
from .storage.checkpoint import EstimatorCheckpoint
from .pose_estimation_estimator import PoseEstimationEstimator

class ONNXModel:

    def __init__(self, *, config, logger, checkpoint_file_dir, checkpoint_file_name, onnx_file_dir, onnx_file_name, batch_size, input_shape):
        '''
        Class responsible for the deep learning model in the ONNX format

        Args: 
            checkpoint_file_dir (string): directory to the checkpoint file. If the checkpoint is located in the Documents it will 
                be: /Users/x.x/Documents
            checkpoint_file_name (string): name of the checkpoint
            onnx_file_dir (string): directory where you want to save the onnx file or directory where you saved the onnx file
            onnx_file_name (string): name of the onnx file you want to give if you want to create it or name of the file already created
            batch_size (int): batch size you want to use for the inference 
            input_shape (list): shape of the image input 
        '''        
        if checkpoint_file_dir.startswith("gs://"):
            checkpoint_file_local_dir = "models"
            download_file_from_gcs(checkpoint_file_dir, checkpoint_file_local_dir, checkpoint_file_name)
            checkpoint_file_path = os.path.join(checkpoint_file_local_dir, checkpoint_file_name)
        else:
            checkpoint_file_path = os.path.join(checkpoint_file_dir, checkpoint_file_name)
        
        self.checkpointer = EstimatorCheckpoint(
            estimator_name=config.estimator, log_dir=config.system.log_dir_system,
        )

        # we provide the path to the checkpoint
        config.checkpoint.load_dir_checkpoint = checkpoint_file_path
        # we load the checkpoint model
        estimator = PoseEstimationEstimator(config=config, logger=logger)
        self.model = estimator.model
        
        self.model.eval()

        self.onnx_file_name = onnx_file_name
        self.onnx_file_dir = onnx_file_dir 

        self.save_on_cloud = False
        if self.onnx_file_dir.startswith("gs://"):
            self.onnx_file_local_path = os.path.join("models", self.onnx_file_name)
            self.save_on_cloud = True        
        else:
            self.onnx_file_local_path = os.path.join(self.onnx_file_dir, self.onnx_file_name)
            Path(self.onnx_file_dir).mkdir(parents=True, exist_ok=True)

        self.input_shape = [batch_size, *input_shape]

    def export_model(self, input_names=None, output_names=None, dynamic_axes=None):
        '''
        Exports the deep learning model into ONNX format 

        Args:
            input_names (list): list of the model inputs
            output_names (list): list of the model outputs
            dynamic_axes (dict): allow variable size for the input of the model
        '''
        inputs = torch.ones(*self.input_shape)
        self.model(inputs)
        torch.onnx.export(self.model, 
                          inputs, 
                          self.onnx_file_local_path, 
                          input_names=input_names, 
                          output_names=output_names, 
                          dynamic_axes=dynamic_axes
                          )
        
        if self.save_on_cloud:
            copy_file_to_gcs(self.onnx_file_dir, self.onnx_file_name, self.onnx_file_local_path)


    def check_model(self):
        '''
        Checks if the intermediate representation (IR) is well formed
        '''
        onnx_model = torch.onnx.load(self.onnx_file_local_path)
        torch.onnx.checker.check_model(onnx_model)

