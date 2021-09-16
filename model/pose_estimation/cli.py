"""
Usage:
    cli.py train [options] [config] [dataset] [training-options] [hyperparameter-options] [save-options] [loading-options]
    cli.py evaluate [options] [config] [dataset] [evaluation-options] [save-options] [loading-options]
    cli.py convert_onnx [options] [config] [onnx]

config: 
    --config-file=<str>                         Path to the file containing the yaml file for the model configuration. [default: config.yaml]

dataset:
    --sample-size-train=<int>                   Size of a dataset training sample. It is used to test operations/commands on a few examples.
    --sample-size-val=<int>                     Size of a dataset validation sample. It is used to test operations/commands on a few examples.
    --sample-size-test=<int>                    Size of a dataset test sample. It is used to test operations/commands on a few examples.
    --gcs-bucket=<string>                       Name of GCS Bucket where the datasets are located.
    --download-data-gcp=<boolean>               If True it will download the data from gcp otherwise it will use the data you have on local.
    --pose-estimation-gcs-path=<string>         Path inside the gcp bucket where the datasets are located.

training-options:
    --batch-training-size=<int>                 Batch size of the training dataset.
    --batch-validation-size=<int>               Batch size of the validation dataset.
    --epochs=<int>                              Number of epoch we want to train the model.
    --accumulation-steps=<int>                  Accumulated Gradients are only updated after X steps. This creates an effective batch size of 
                                                batch_size * accumulation_steps.
    --checkpoint-freq=<n>                       Save a model checkpoint every n training iterations.
    --dataset-zip-file-name-training=<str>      Name of the zip file for the training dataset.
    --dataset-zip-file-name-validation=<str>    Name of the zip file for the training dataset.
    --eval_freq=<int>                           Frequency to launch the evaluation process 
    --scale-factor=<float>                      Scaling factor we apply on the translation in order to avoid exploding gradients

evaluation-options:
    --batch-test-size=<int>                     Batch size of the validation dataset.
    --dataset-zip-file-name-test=<str>          Name of the zip file for the training dataset.

hyperparameter-options:
    --lr=<float>                                Learning rate for Adam optimization.
    --beta-1=<float>                            Beta1 for the Adam optimization.
    --beta-2=<float>                            Beta2 for the Adam optimization.
    --beta-loss=<int>                           Value of the beta in the weighted loss function.

save-options:
    --log-dir-system=<dir>                      Where to save the Tensorboard event files.

loading-options:
    --load-dir-checkpoint=<dir>                 Path to checkpoint folder from which to load weights and continue training.
    --data-root=<dir>                           Upper directory of the data folders (training, evaluation and test)

onnx-options:
    --onnx-file-dir=<dir>                       Directory towards the created onnx file
    --onnx-file-name=str                        Name of the created onnx file
    --batch-onnx-size=int                       Batch size for the onnx file
    --input-shape=list                          Shape of the model input
"""
from docopt import docopt

from functools import reduce
import operator

import torch
from easydict import EasyDict
import yaml

from .pose_estimation_estimator import PoseEstimationEstimator
from .onnx_conversion import ONNXModel

import logging
logger = logging.getLogger(__name__)

# PARSE CLI
def _get_config(args):
    """Fetches default hyperparameters from provided file, and overwrites
    them with any provided command line arguments.
    Args:
        args (dict): dictionnary created using the docopt libary based on the default
        parameters and the parameters entered in the command line
    """
    # get default config
    config = yaml.load(open(args["--config-file"], "r"), Loader=yaml.FullLoader)

    # overwrite with cli args if provided
    config = _overwrite_config(config, args)

    config = EasyDict(config)
    return config


def _overwrite_config(config, args):
    """Overwrites config from yaml file with any matching command line args provided.
    If overwriting, casts the command line value to the type of the corresponding default config value.
    Args:
        config (dict): Dictionary from parsed yaml file. May be nested. Keys expected to be snake_case.
        args (dict): Dictionary from parsed command line arguments. Keys expected to be in --this-format.
    Returns:
        config (dict): Config with appropriate values overwritten.
    """
    assert type(config) == dict

    for key, value in config.items():

        # recurse if found nested dictionary
        if type(value) == dict:
            result = _overwrite_config(value, args)
            config[key] = result

        # overwrite config if necessary
        else:
            arg_key = _arg_from_snakecase_key(key)
            arg_val = args.get(arg_key)
            if arg_val:
                config_val = config[key]
                arg_val = type(config_val)(arg_val)
                config[key] = arg_val

    return config


def _arg_from_snakecase_key(snakecase_key):
    """Converts input snake case to command line argument format.
    Args:
        snakecase_key (string): Expected to be in this_format.
    Returns:
        string: Converted to --this-format.
    """
    key = snakecase_key.replace("_", "-")
    key = "--" + key
    return key


## MAIN
def main():
    args = docopt(__doc__)
    config = _get_config(args)

    estimator = PoseEstimationEstimator(config=config, logger=logger)

    print("device: ", estimator.device)

    if args["train"]:
        estimator.train()
    elif args["evaluate"]:
        estimator.evaluate()
    elif args["convert_onnx"]:
        onnx_model = ONNXModel(
            config=config,
            logger=logger,
            checkpoint_file_dir=config.checkpoint.checkpoint_file_dir,
            checkpoint_file_name=config.checkpoint.checkpoint_file_name, 
            onnx_file_dir=config.onnx.onnx_file_dir,
            onnx_file_name=config.onnx.onnx_file_name,
            input_shape=config.onnx.input_shape,
            batch_size=config.onnx.batch_onnx_size
        )

        # we do the conversion of the model to onnx format 
        onnx_model.export_model()
        logger.info("Successful convertion of the model to onnx format")
    else:
        print("command not found!")

    estimator.writer.done()


if __name__ == "__main__":
    main()
