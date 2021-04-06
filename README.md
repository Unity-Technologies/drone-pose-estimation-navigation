# AI Hackweek 2021 - Drone Pose Estimation and Navigation

<img src="https://github.com/Unity-Technologies/com.unity.perception/blob/master/com.unity.perception/Documentation~/images/unity-wide.png" align="middle" width="3000"/>
<img src="https://github.com/Unity-Technologies/ai-hw21-drone-pose-estimation-navigation/blob/readme/Documentation/images/project_teaser_github.png" align="middle" width="3000"/>

<!-- ![ReleaseBadge](https://badge-proxy.cds.internal.unity3d.com/5ab9a162-9dd0-4ba1-ba41-cf25378a927a) -->

<img src="https://img.shields.io/badge/unity-2020.2-green.svg?style=flat-square" alt="unity 2020.2">

> Note: This project has been developed with Python 3 and relies on Unity version `2020.2.7f1` and Perception SDK `0.8.0-preview.2`.

This project is on multi-object pose estimation and navigation, for a drone and a target landing pad. To do so, we used Unity’s perception package, in order to capture randomly generated synthetic data, which can be used to train a multi-object pose estimation model. This model can then be used to estimate the pose of our drone and target objects in a newly generated scene that was never seen before. The estimated pose, in other words, the position and orientation of the objects, allow us to perform path planning, navigation, and obstacle avoidance, for landing the drone onto the target.



### Cloning the repository
Prior to cloning via command line, ensure that you have [Git LFS](https://docs.github.com/en/github/managing-large-files/installing-git-large-file-storage) installed, otherwise large files will not download correclty. Then open a new terminal and set yourself where you want to host the repository and run the following command: 
```bash
git clone git@github.com:Unity-Technologies/ai-hw21-drone-pose-estimation-navigation.git
```


**Table of Contents**
- [Part 1: Setting up Unity scene for data collection](#link-part-1)
- [Part 2: Data Collection and Model Training](#link-part-2)
- [Part 3: gRPC connection](#link-part-3)
- [Part 4: Navigation with NavMesh](#link-part-4)
  
---

### <a name="link-part-1">Part 1: Setting up Unity scene for data collection</a>

<!-- <img src="Documentation/Images/0_scene.png" width=400 /> -->
 
 This part focuses on setting up the scene for data collection using the Unity Computer Vision [Perception Package](https://github.com/Unity-Technologies/com.unity.perception).We use the Perception Package [Randomizers](https://github.com/Unity-Technologies/com.unity.perception/blob/master/com.unity.perception/Documentation~/Randomization/Index.md) to randomize aspects of the scene in order to create variety in the training data. 

If you want more information on how the scene is set up for the data collection, go [here](Documentation/1_set_up_scene_data_collection.md).

---

### <a name="link-part-2">Part 2: Data Collection and Model Training</a> 

<!-- <img src="Documentation/Images/0_data_collection_environment.png" width=400/> -->


This part includes running data collection with the Perception Package, and using that data to train a deep learning model. The training step can take some time. If you'd like, you can skip that step by using our pre-trained model.

To measure the success of grasping in simulation using our pre-trained model for pose estimation, we did 100 trials and got the following results:

|                  | Success | Failures | Percent Success |
|:----------------:|:-------:|:--------:|:---------------:|

If you want to have more information on how the model works and how to train your own, go [here](Model/README.md).

---
### <a name="link-part-3">Part 3: gRPC connection</a> 

The gRPC pipeline drives the communication between unity process and external python process that serves the trained model input/output. gRPC is a modern high performant Remote Procedure Call framework that can run in any environment. More information on the gPRC can be found [here](https://grpc.io/docs/what-is-grpc/introduction).

In this project we use gRPC during inference phase to get the prediction of the pose estimation from the model. More information on how gRPC pipeline is setup for this project can be found [here](Documentation/1_set_up_grpc_pipeline.md).

<!-- <img src="Documentation/Images/0_grpc.png" width=400/> -->

---

### <a name="link-part-4">Part 4: Navigation with NavMesh</a> 

<!-- <img src="Documentation/Gifs/0_demo.gif" width=400/> -->

We used Unity's [Navigation and Pathfinding](https://docs.unity3d.com/Manual/Navigation.html) system that allows intelligent navigation of characters in the game world. This system uses navigation meshes that are automatically created from the scene geometry, called [NavMesh](https://docs.unity3d.com/ScriptReference/AI.NavMesh.html). More information on how we set up our NavMesh can be found [here](Documentation/1_set_up_navmesh.md).

---

### <a name="link-part-5">: Part 5: closed loop control
