# Object Pose Estimation Tutorial: Part 5


In [Part 1](1_create_unity_project_with_unity_packages.md) of the tutorial, we learned how to create our Scene in Unity Editor. In [Part 2](2_set_up_the_scene_for_data_collection.md), we set up the Scene for data collection.
In [Part 3](3_data_collection_model_training.md) we have learned: 
* How to collect the data 
* How to train the deep learning model
In [Part 4](4_setup_grpc_connection.md), we have learned how to setup the grpc connection to communicate between our python model and our Unity environment.   
In [Part 5](5_navigation_with_navmesh.md), we learnt how to integrate NavMesh into our Unity environment in order to perform the drone navigation (path planning and objects avoidance).

In this part, we will start the grpc server and setup the Unity scene in order to perform the drone navigation. 


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

#### Moving the Model to the ROS Folder

3. Go inside the `ROS/src/ur3_moveit` folder and create a folder called `models`. Then copy your model file (.tar) into it.

### <a name="step-3">Set Up the grpc connection</a>


### Click here to go back to [Part 3](3_data_collection_model_training.md).
