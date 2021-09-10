# Object Pose Estimation Tutorial: Part 1

In this first part of the tutorial, we will start by downloading and installing the Unity Editor. We will install our project's dependencie(s): the Perception package. We will then use a set of provided prefabs to easily prepare a simulated environment containing a drone, 4 walls, and a target. 


**Table of Contents**
  - [Requirements](#reqs)
  - [Create a New Project](#step-1)
  - [Download the Perception, the URDF and the TCP connector Packages](#step-2)
  - [Set Up the Ground Truth Renderer Feature](#step-3)
  - [Set Up the Scene](#step-4)

---

### <a name="reqs">Requirements</a>

To follow this tutorial you need to **clone** this repository even if you want to create your Unity project from scratch. 

1. Open a terminal and navigate to the folder where you want to host the repository. 
```bash
git clone git@github.com:Unity-Technologies/drone-pose-estimation-navigation.git
```

2. [Install Unity `2020.2.*`.](install_unity.md) or a later version of it. 


### <a name="step-1">Create a New Project</a> 
When you first run Unity, you will be asked to open an existing project, or create a new one.

1. Open Unity and create a new project using the **Universal Render Pipeline**. Name your new project _**Drone Pose Estimation Tutorial**_, and specify a desired location as shown below.

<p align="center">
<img src="Images/1_create_new_project.png" align="center" width=950/>
</p>


### <a name="step-2">Download the Perception Package</a>

Once your new project is created and loaded, you will be presented with the Unity Editor interface. From this point on, whenever we refer to the "editor", we mean the Unity Editor.

#### How to install packages
We will need to download and install the Perception package. In general, packages can be installed in Unity with the following steps:

- From the top menu bar, open _**Window**_ -> _**Package Manager**_. As the name suggests, the _**Package Manager**_ is where you can download new packages, update or remove existing ones, and access a variety of information and additional actions for each package.

- Click on the _**+**_ sign at the top-left corner of the _**Package Manager**_ window and then choose the option _**Add package from git URL...**_. 

- Enter the package address and click _**Add**_. 
  
It can take a few minutes for the manager to download and import packages.

<p align="center">
<img src="Gifs/1_package_imports_short.gif"/>
</p>


#### Install Dependencies
Install the following packages with the provided git URL:

1. [Perception package](https://github.com/Unity-Technologies/com.unity.perception) - `com.unity.perception@0.8.0-preview.3`
    * This will help us collect training data for our machine learning model. 


### <a name="step-3">Set Up Ground Truth Renderer Feature</a>

The ***Hierarchy***, ***Scene***, ***Game***, ***Inspector***, ***Project***, and ***Console*** windows of the Unity Editor, as well as the ***Play/Pause/Step*** toolbar have been highlighted below for reference, based on the default layout. Custom Unity Editor layouts may vary slightly. A top menu bar option is available to re-open any of these windows: ***Window > General***.

<p align="center">
<img src="Images/1_scene_overview.png"/>
</p>


The Perception package relies on a "Ground Truth Renderer Feature" to output labeled images as training data. Follow the steps below to add this component to your rendering settings:

1. The _**Project**_ tab contains a search bar; use it to find the file named `ForwardRenderer`, and click on the file named `ForwardRenderer.asset` as shown below:

<p align="center">
<img src="Images/1_forward_renderer.png"/>
</p>

2. Click on the found file to select it. Then, from the _**Inspector**_ tab of the editor, click on the _**Add Renderer Feature**_ button, and select _**Ground Truth Renderer Feature**_ from the dropdown menu:

<p align="center">
<img src="Images/1_forward_renderer_inspector.png" width=430 height=350/>
</p>


### <a name="step-4">Set Up the Scene</a>

#### The Scene
Simply put in Unity, a Scene contains any object that exists in the world. This world can be a game, or in this case, a data-collection-oriented simulation. Every new project contains a Scene named `SampleScene`, which is automatically opened when the project is created. This Scene comes with several objects and settings that we do not need, so let's create a new one.

1. In the _**Project**_ tab, right-click on the `Assets/Scenes` folder and click _**Create -> Scene**_. Name this new Scene `TutorialPoseEstimation` and double-click on it to open it. 

The _**Hierarchy**_ tab of the editor displays all the Scenes currently loaded, and all the objects currently present in each loaded Scene, as shown below:
<p align="center">
<img src="Images/1_hierarchy.png" width=500 height=200/>
</p>

As seen above, the new Scene already contains a camera (`Main Camera`) and a light (`Directional Light`). We will now modify the camera's field of view and position to prepare it for the tutorial.

2. Still in the _**Inspector**_ tab of the `Main Camera`, modify the camera's `Position` and `Rotation` to match the values shown below. This orients the camera so that it will have a good view of the objects we are about to add to the scene.   

<p align="center">
<img src="Images/1_camera_settings.png" height=117 width=500/>
</p>

3. Click on `Directional Light` and in the _**Inspector**_ tab, modify the light's `Position` and `Rotation` to match the screenshot below. 

<p align="center">
<img src="Images/1_directional_light.png" height=217 width=500/>
</p>

#### Adding Tutorial Files
Now it is time to add some more objects to our scene. Before doing so, we need to import some folders containing the required assets. 

4. Download [TutorialAssets.zip](https://github.com/Unity-Technologies/Robotics-Object-Pose-Estimation/releases/download/v0.0.1/TutorialAssets.zip), and unzip it. It should contain the following subfolders: `Materials`, `Prefabs`, `RosMessages`, `Scripts`, `URDFs`.

5. Drag and Drop the `TutorialAssets` folder from your operating system's file explorer onto the `Assets` folder in the _**Project**_ tab of the editor. 

Your `Assets` folder should like this: 

<p align="center">
<img src="Images/1_assets_preview.png"/>
</p>

#### Using Prefabs
Unity’s [Prefab](https://docs.unity3d.com/2020.2/Documentation/Manual/Prefabs.html) system allows you to create, configure, and store a GameObject complete with all its components, property values, and child GameObjects as a reusable Unity Asset. It is a convenient way to store complex objects. 

A Prefab is just a file, and you can easily create an instance of the object in the scene from a Prefab by dragging it into the _**Hierarchy**_ tab.

For your convenience, we have provided Prefabs for most of the components of the scene (the cube, goal, table, and floor).

6. In the _**Project**_ tab, go to `Assets/TutorialAssets/Prefabs/Part1` and drag and drop the `Cube` Prefab into the _**Hierarchy**_ tab.

7. Repeat the above action with the `Goal`, `Table` and `Floor` Prefabs. 


<p align="center">
<img src="Gifs/1_import_prefabs.gif"/>
</p>

>Note: If you encounter issues with the materials of the imported Prefabs, check the [Troubleshooting Guide](troubleshooting.md) for potential solutions.


#### Importing the Robot
Finally we will add the robot and the URDF files in order to import the UR3 Robot. 

8. In the _**Project**_ tab, go to `Assets/TutorialAssets/URDFs/ur3_with_gripper` and right click on the `ur3_with_gripper.urdf` file and select `Import Robot From Selected URDF file`. A window will pop up, keep the default **Y Axis** type and `VHACD` **Mesh Decomposer** in the Import menu. Then, click Import URDF. These actions are shown in the video below. 

>Note: Unity uses a left-handed coordinate system in which the y-axis points up. However, many robotics packages use a right-handed coordinate system in which the z-axis or x-axis point up. For this reason, it is important to pay attention to the coordinate system when importing URDF files or interfacing with other robotics software.

>Note: The `VHACD` algorithm produces higher quality convex hull for collision detection than the default algorithm.

<p align="center">
<img src="Gifs/1_URDF_importer.gif" width=800 height=548/>
</p>

>Note: If you encounter issues with importing the robot, check the [Troubleshooting Guide](troubleshooting.md) for potential solutions.

#### Setting up the Robot

9. In the _**Hierarchy**_ tab, select the `ur3_with_gripper` GameObject and in the _**Inspector**_ view, go to the `Controller` script and set the `Stiffness` to **10000**, the `Damping` to **1000** and the `Force Limit` to **1000**. These are physics properties that control how the robot moves.

10. In the _**Hierarchy**_ tab, click on the arrow to the left of the `ur3_with_gripper` GameObject to expand it, then expand `world`, and select `base_link`. In the `Articulation Body` component, toggle on `Immovable`. This will fix the robot base to its current position.

<p align="center">
<img src="Gifs/1_robot_settings.gif" width=800 height=465/>
</p>


### Proceed to [Part 2](2_set_up_the_data_collection_scene.md).
