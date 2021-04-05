# Set up the scene for the data collection of the drone pose estimation and navigation project

In this document, we will go over the different randomizers needed for the data collection and all the elements we need to work properly.  


**Table of Contents**
  - 
  - [Set up the randomizers](#step-1)
  - [Data collection](#step-2)

---


### <a name="step-1">Set up the randomizers</a>
The images you generate to train your deep learning model and the images you later use for inference during the pose estimation task will need to have the same resolution. We will now set this resolution.

- In the ***Game*** view, click on the dropdown menu in front of `Display 1`. Then, click **+** to create a new preset. Make sure `Type` is set to `Fixed Resolution`. Set `Width` to `1027` and `Height` to `592`. The gif below depicts these actions. 


#### Domain Randomization
We will be collecting training data from a simulation, but most real perception use-cases occur in the real world. 
To train a model to be robust enough to generalize to the real domain, we rely on a technique called [Domain Randomization](https://arxiv.org/pdf/1703.06907.pdf). Instead of training a model in a single, fixed environment, we _randomize_ aspects of the environment during training in order to introduce sufficient variation into the generated data. This forces the machine learning model to handle many small visual variations, making it more robust.

### Randomizers 
To perform domain randomization, we use the perception package by using pre-existing randomizers and by adding our custom ones. 
The gameobject `Simulation Scenario` possesses the script component [Pose Estimation Scenario](trainSceneProject/Assets/Scripts/PoseEstimationScenario.cs) which gathers all the following randomizers. 

1. `DistractorObjectRandomizer`: it randomizes the number of objects generated in the scene. Objects are picked among a library. 
2. `DroneObjectPositionRandomizer`: it randomizes the position of the objects in the scene. The available positions are within a volume and we made sure that for the drone and the landing target they always appear in the top 2/3 of the screen and in the bottom 1/3 of the screen respectively. 
3. `RotationRandomizer` and  `CustomRotationRandomizer`: it randomizes the rotation of the landing target and the drone along the y-axis and it randomly rotates all the distractor objects along x, y and z-axis. 
4. `Texture Randomizer`: it randomizes the texture on the wall. There a dataset of textures for the training and one for the testing. 
5. `Hue Offset Randomizer`: it randomizes the color of the different distractor objects. 
6. `Light Randomizer`: it randomizes the color, intensity and direction of the light.
7. `Custom Foreground Scale Randomizer`: it randomizes the scale of the distractor objects but also the drone and the landing target. 
8. `Camera Randomizer`: it randomizes the camera's position


---
### <a name="step-2">Data Collection</a> 
In the framework of the Drone pose estimation and navigation project, we collected 30K data for training and 3K data for validation. 
For this dataset, we set to active all the randomizers. 

After collecting the data, you should rename those files: `drone_training` and `drone_validation`. 
