using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Samplers;


[Serializable]
[AddRandomizerMenu("Perception/Object Position Randomizer")]
public class ObjectPositionRandomizer : Randomizer
{
    /*  Chooses positions on the plane for placing all objects with the corresponding tag.
     *      - Each object has a radius (defined on the tag, computed per-object based on its bounds)
     *      - No object will be close enough to the edge of the plane to fall off
     *      - All objects will be within the min and max RobotReachability distance to the robot base link (as measured 
     *          on the surface of the plane).
     *      - No object will be close enough to another tagged object to collide with it
     *      
     *  Example use case: placing objects on a table with a robot arm, at random valid positions
     *  where they can be reached by the robot arm. 
     *  
     *  The plane can be manipulated in the editor for easy visualization of the placement surface.
     *  
     *  Assumptions:
     *      - The placement surface is parallel to the global x-z plane. 
     *      - The robot arm is sitting on the placement surface
     */

    public int maxPlacementTries = 100;
    public GameObject box;
    public GameObject drone;
    public float minDistanceDrone;
    private FloatParameter random = new FloatParameter {value = new UniformSampler(0f, 1f)};

    private SurfaceObjectPlacer placer;


    protected override void OnAwake()
    {
        Bounds droneBounds = drone.GetComponent<Renderer>().bounds;
        Vector3 boundsObjectExtents = droneBounds.extents;
        ClosestPositionConstraint minDistanceDroneConstraint = CreateClosestPositionConstraint(drone.transform.position, boundsObjectExtents, minDistanceDrone);
        
        Bounds boundsBox = box.GetComponent<Renderer>().bounds;
        Vector3 boundsBoxExtents = boundsBox.extents;
        Vector3 boxPosition = box.transform.position;
        placer = new SurfaceObjectPlacer(random, minDistanceDroneConstraint, boundsBoxExtents, boxPosition, maxPlacementTries);
    }


    protected override void OnIterationStart()
    {
        placer.IterationStart();

        IEnumerable<ObjectPositionRandomizerTag> tags = tagManager.Query<ObjectPositionRandomizerTag>();
        
        (List<GameObject> downObjects, List<GameObject> upObjects, List<GameObject> otherObjects) = SeparateTags(tags);

        foreach (GameObject downObj in downObjects)
        {
            bool success = placer.PlaceObject(downObj, true, false);
            if (!success)
            {
                return;
            }
        }
        
        foreach (GameObject upObj in upObjects)
        {
            bool success = placer.PlaceObject(upObj, false, true);
            if (!success)
            {
                return;
            }
        }

        foreach (GameObject otherObj in otherObjects)
        {
            bool success = placer.PlaceObject(otherObj, false, false);
            if (!success)
            {
                return;
            }
        }
    }


    // HELPERS
    
    private (List<GameObject> reachableObjects, List<GameObject> upObjects, List<GameObject> otherObjects) SeparateTags(IEnumerable<ObjectPositionRandomizerTag> tags)
    {
        List<GameObject> downObjects = new List<GameObject>();
        List<GameObject> upObjects = new List<GameObject>();
        List<GameObject> otherObjects = new List<GameObject>();

        foreach (ObjectPositionRandomizerTag tag in tags)
        {
            GameObject obj = tag.gameObject;
            if (tag.downObject)
            {
                downObjects.Add(obj);
            }
            else if (tag.upObject)
            {
                upObjects.Add(obj);
            }
            else
            {
                otherObjects.Add(obj);
            }
        }
        return (downObjects, upObjects, otherObjects);
    }

    public static ClosestPositionConstraint CreateClosestPositionConstraint(Vector3 dronePosition, Vector3 boundsObjectExtents, float minDistanceDrone)
    {
        ClosestPositionConstraint constraint = new ClosestPositionConstraint();
        constraint.drone = dronePosition;
        constraint.boundsDroneExtents = boundsObjectExtents;
        constraint.droneClosestLimit = minDistanceDrone;
        return constraint;
    }

}