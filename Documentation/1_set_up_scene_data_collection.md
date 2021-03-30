# Object Pose Estimation Tutorial: Part 1

In this document, we will go over the different randomizers needed for the data collection and all the elements the need to work properly.  


**Table of Contents**
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
In our environment we decided to randomize a lot of different elements:
- randomization of the number of distractor objects: randomly generates objects in the scene. Objects are picked among a library. 
- randomization of the position of objects: the available positions are within a volume and we made sure that for the drone and the landing target they always appear in the top 2/3 of the screen and in the bottom 1/3 of the screen respectively. 
- randomization of the orientation: randomly rotates the landing target and the drone along the y-axis and randomly rotates all the distractor objects along x, y and z-axis. 
- randomization of the texture on the wall. There a dataset of textures for the training and one for the testing. 
- randomization of the color of the different distractor objects. 
- randomization of the color, intensity and direction of the light.
- randomization of the scale of the distractor objects but also the drone and the landing target. 
- randomization of the camera's position


---
### <a name="step-2">Data Collection</a> 
In the framework of the Drone pose estimation and navigation project, we collected 30K data for training and 3K data for validation. 
After collecting the data, you should rename those files: `drone_training` and `drone_validation`. 

### Proceed to [Part 2](2_set_up_the_data_collection_scene.md).
