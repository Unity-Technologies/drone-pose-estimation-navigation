# Setting up the NavMesh for drone navigation

In this document we described how we used Unity's [Navigation and Pathfinding](https://docs.unity3d.com/Manual/Navigation.html) system that allows intelligent navigation of characters in the game world. This system uses navigation meshes that are automatically created from the scene geometry, called [NavMesh](https://docs.unity3d.com/ScriptReference/AI.NavMesh.html). 
We extended NavMesh AI to our 3D path planning for drone navigation using a 2D surrogate solution that allows us to alter the navigation of the drone at runtime with scene geometry and obstacle defintions.

**Table of Contents**
  - [Setting up the NavMesh agent](#step-1)
  - [Setting up the NavMesh](#step-2)
  - [Baking NavMesh at runtime](#step-3)

---

### <a name="step-1">Setting up the NavMesh agent</a>

We begin by importing necessary NavMesh Components from [this Git repository](https://github.com/Unity-Technologies/NavMeshComponents/tree/master/Assets/NavMeshComponents). Place the `NavMeshComponents` folder inside the `Assets` folder. Note that this step is already done for this project. 

Select the `Drone_01 Variant_modified.prefab` add a `Nav Mesh Agent` component to it. We set the `Agent Type` to `Humanoid`, and increase the `Steering Speed` to 100. The rest of the properties must be set as shown in the image below:

<img src="https://github.com/Unity-Technologies/ai-hw21-drone-pose-estimation-navigation/blob/readme/Documentation/images/NavMeshAgent.png" width="400">

In the Inspector window, create a new layer called `Player` and assign it to the `Drone_01 Variant_modified.prefab` as shown below:

<img src="https://github.com/Unity-Technologies/ai-hw21-drone-pose-estimation-navigation/blob/readme/Documentation/images/InspectorLayer.png" width="400">

Ensure that the `Box Collider` component is enabled for all the following Game Objects:
1. `Drone_01 Variant_modified.prefab`
2. `TargetCube_modified.prefab`
3. `Wall`
4. Background distractor/occluder primitive objects in `Assets/Samples/Perception/0.8.0-preview.2/Tutorial Files/Background Objects/Prefabs`

---

### <a name="step-2">Setting up the NavMesh</a>

We then create an Empty Game Object in the scene hierarchy and rename it to `NavMesh`. Next, we add a `NavMeshSurface` component to it and set the properties according to the image shown below:

<img src="https://github.com/Unity-Technologies/ai-hw21-drone-pose-estimation-navigation/blob/readme/Documentation/images/NavMeshSurface.png" width="400">

---

### <a name="step-3">Baking NavMesh at runtime</a>

by instantiating a plane at runtime that stretches from the estimated drone position to the estimated target position. We then bake a new navmesh based this plane and object colliders that exist in the scene. This allows us to maneuver the drone to the target while performing reasonable collision avoidance, to the extent that Navmesh allows.
