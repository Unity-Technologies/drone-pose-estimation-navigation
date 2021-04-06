# Setting up the NavMesh for drone navigation

In this document we described how we used Unity's [Navigation and Pathfinding](https://docs.unity3d.com/Manual/Navigation.html) system that allows intelligent navigation of characters in the game world. This system uses navigation meshes that are automatically created from the scene geometry, called [NavMesh](https://docs.unity3d.com/ScriptReference/AI.NavMesh.html). 
We extended NavMesh AI to our 3D path planning for drone navigation using a 2D surrogate solution that allows us to alter the navigation of the drone at runtime with scene geometry and obstacle defintions.

**Table of Contents**
  - [Setting up the NavMesh agent](#step-1)
  - [Setting up the NavMesh](#step-2)
  - [Baking NavMesh at runtime](#step-3)

---

### <a name="step-1">Setting up the NavMesh agent</a>

by instantiating a plane at runtime that stretches from the estimated drone position to the estimated target position. We then bake a new navmesh based this plane and object colliders that exist in the scene. This allows us to maneuver the drone to the target while performing reasonable collision avoidance, to the extent that Navmesh allows.

---

### <a name="step-2">Setting up the NavMesh</a>


---

### <a name="step-3">Baking NavMesh at runtime</a>
