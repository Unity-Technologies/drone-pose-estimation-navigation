# Drone Pose Estimation And Navigation Tutorial: Part 3

In [Part 1](1_create_unity_project_with_unity_packages.md) of the tutorial, we learned how to create our Scene in Unity Editor.

In [Part 2](2_set_up_the_scene_for_data_collection.md) of the tutorial, we learned:
* How to equip the camera for the data collection
* How to set up labelling and label configurations
* How to create your own Randomizer 
* How to add our custom Randomizer 

In [Part 3](3_data_collection_and_model_training) of the tutorial we learned:
* How to collect large dataset of RGB images and the corresponding poses of the target and the drone. 
* How to use that data to train machine learning model to predict the target's position and drone's position from images taken by our camera.

In this part, we will setup the grpc connection in order to communicate between our python model and our Unity environment.

Steps included in this part of the tutorial:

**Table of Contents**
  - [Setup](#setup)
  - [Add the Pose Estimation Model](#step-2)
  - [Set up the grpc connection](#step-3)
  - [Put It All Together](#step-4)

---

### <a name="setup">Set up</a>

### <a name="step-2">Add the Pose Estimation Model</a>

Here you have two options for the model:

#### Option A: Use Our Pre-trained Model

1. To save time, you may use the model we have trained. Download this [UR3_single_cube_model.tar](https://github.com/Unity-Technologies/Robotics-Object-Pose-Estimation/releases/download/v0.0.1/UR3_single_cube_model.tar) file, which contains the pre-trained model weights.

#### Option B: Use Your Own Model

2. You can also use the model you have trained in [Part 3](3_data_collection_model_training.md). However, be sure to rename your model to `UR3_single_cube_model.tar` as the script that will call the model is expecting this name.

### <a name="step-3">Set Up the grpc connection</a>

### Proceed to [Part 5](5_navigation_and_inference.md).

### 

### Go back to [Part 3](3_data_collection_and_model_training.md)
