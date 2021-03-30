Drone Pose Estimation Model
=====================
This section contains code for training and evaluating a deep neural network to predict the pose of a single object from RGB images. We provide support for running both locally and with Docker.  

This model is a modified implementation of [Domain Randomization for Transferring Deep Neural Networks from Simulation to the Real World](https://arxiv.org/pdf/1703.06907.pdf), by Tobin et. al. It is based on the classic VGG-16 backbone architecture, and initialized with weights pre-trained on the ImageNet dataset. The head of the network is replaced with a 3D position prediction head that outputs (x, y, z), and an orientation predicton head that outputs a quaternion (q<sub>x</sub>, q<sub>y</sub>, q<sub>z</sub>, q<sub>w</sub>) for both the drone and landing target. 

<p align='center'>
  <img src='documentation/docs/network.png' height=400/>
</p>

## Quick Start (Recommended)
We trained this model on sythetic data collected in Unity. To learn how to collect this data and train the model yourself, see our [data collection and training tutorial](../Documentation/quick_demo_train.md).

## Pre-Trained Model

## Setup
 * [For running on docker](documentation/running_on_docker.md#docker-requirements)
 * [For running in the cloud](documentation/running_on_the_cloud.md)
 * [For running locally with Conda](../Documentation/3_data_collection_model_training.md#option-b-using-conda)

## CLI
This model supports a `train` and an `evaluate` command. Both of these have many arguments, which you can examine in `cli.py`. They will default to the values in `config.yaml` for convenience, but can be overridden via the command line.

The most important `train` arguments to be aware of are:
* `--data_root`: Path to the directory containing your data folders. These directory should include `drone_training` and `drone_validation`, containing the training and validation data, respectively. 
* `--log-dir-system`: Path to directory where you'd like to save Tensorboard log files and model checkpoint files.

The most important `evaluate` arguments to be aware of are:
* `--load-dir-checkpoint`: Path to model to be evaluated. 
* `--data_root`: Path to the directory containing your data folders. These directory should include `drone_training` and `drone_validation`, containing the training and validation data, respectively. 


## Performance

## Unit Testing

We use [pytest](https://docs.pytest.org/en/latest/) to run tests located under `tests/`. You can run the tests after following the setup instructions in [Running on Local with Conda](../documentation/3_data_collection_model_training.md#option-b-using-conda) commands.

You can run the entire test suite with:

```bash
python -m pytest
```

or run individual test files with:

```bash
python -m pytest tests/test_average_translation_mean_square_error.py
```

## Resources
* [Documentation](documentation/codebase_structure.md) describing structure of the model code
* [Domain Randomization for Transferring Deep Neural Networks from Simulation to the Real World](https://arxiv.org/pdf/1703.06907.pdf)