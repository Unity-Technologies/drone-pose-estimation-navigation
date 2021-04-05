# Set up the scene for the data collection of the drone pose estimation and navigation project

In this document, we will go over the steps for setting up a Unity environment and the different randomizers needed for the data collection.  


**Table of Contents**
  - [Setting up the Unity scene](#step-1)
  - [Set up the randomizers](#step-2)
  - [Data collection](#step-3)

---

### <a name="step-1">Setting up the Unity scene</a>
We first need to set up a Unity scene, which enables us to collect training data for training a pose estimation model that predicts the pose of a drone and a landing target. In the provided Unity project template, you should find an environment that has the necessary components already set up for you to generate data out of the box. Once import the project in Unity, navigate to the `Assets/Scenes/DroneTrainingScene.unity` and open it by double-clicking on it. You should see a scene comprised of a `Wall`, multiple `Lights`, a `Main Camera`, and a `SimulationScenario` and a `Post Process Volume` that generate random scenes with our objects of interest (the drone and the target), multiple distractor objects with random textures, random lighting, and random position and orientation for all the Game Objects in the scene. We will go over what each of the randomizers do later. You will also find some Game Object components in the scene hierarchy that are necessary for running `NavMesh` AI navigation for the drone, as well as gRPC communication pipeline for inference.

#### Assets and Prefabs
We provide multiple drone assets which you can find in `Assets/Racing Drone/prefabs`. For this project we import the drone model called `Drone_01 Variant_modified.prefab` for which we created a custom material that has better visibility in the scene. In the same folder you should find a target prefab called `TargetCube_modified.prefab` which is our landing target for the drone. We created this prefab using a `Cube` 3D Object primitive and a `Quad`. The `Quad` has a landing target texture that does not change. On the other hand, the `Cube` itself's texture is randomized with every new frame. This should reasonably vary the appearance of the target landing pad during the simulation. The textures for the drone and target landing pad could be found in `Assets/Racing Drone/texture` and `Assets/textures` respectively.

If you open the `Drone_01 Variant_modified.prefab` you can see it has multiple components attached to it. The `Animator` component runs a script that animates the drone during the simulation, by spinning the propellers and simluating the acceleration and deceleration of the drone during flight. There are multiple perception randomizer tags attached to the drone that vary its rotation around the Y-axis, its scale, and position in the scene. A `Labeling` component is used to generate 3D Bounding Box labels for the drone. The drone has a `Nav Mesh Agent` component and a `Box Collider` which we use later on during inference, to navigate the drone and land it on the target.
Similar to the drone, the `TargetCube_modified.prefab` also has multiple perception randomizer tags that vary its texture, hue offset, rotation around Y-axis, and its position and scale. The `Labeling` component generates 3D Bouding Box labels for the target.

We created a `Wall` Game Object which is used to randomly change the background texture behind our objects of interest. It has perception randomizer tags for varying its texture and hue offset during the simulation.

Inside `Assets/Samples/Perception/0.8.0-preview.2/Tutorial Files/Background Objects/Prefabs` you can find a set of primitive objects provided by the Perception package. We randomly spawn these primitive objects in the scene to create distractions and occlusions. These objects have perception randomizer tags as well, that vary their texture, hue offset, scale, position, and rotation around the XYZ-axis.

We provide random textures in the form of images from the [COCO dataset](https://cocodataset.org/#home) for training and testing the pose estimation model. You can find these inside `Assets/textures`. These textures are randomly applied to the `TargetCube_modified.prefab`, the `Wall`, and the distractor objects during the simulation.

#### Camera and Lighting
The `Main Camera` has a `Perception Camera` component that is responsible for generating the RGB images and ground truth labels for our training data. In this project we used `BoundingBox3DLabeler`, `ObjectCountLabler`, and `RenderedObjectInfoLabeler` camera labelers. We also created a `Custom Camera Randomizer Tag` that varies intrinsic camera components such as its Field of View and Focal Length, as well as its extrinsic properties, such as its rotation and position in the scene. This in combination with our other scene randomizers helps us generate diverse scenes that are necessary for training a robust pose estimation model.

We use multiple `Directional Light` Game Objects to vary the lighting of the scene. Similar to our other Game Objects they have perception randomizer tags that change the intensity, colour, on/off state, and their position and orientation in the scene.

### <a name="step-2">Set up the randomizers</a>
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
### <a name="step-3">Data Collection</a> 
In the framework of the Drone pose estimation and navigation project, we collected 30K data for training and 3K data for validation. 
For this dataset, we set to active all the randomizers. 

After collecting the data, you should rename those files: `drone_training` and `drone_validation`. 
