using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Perception.Randomization.Parameters;


public class PlacementConstraint
{
    /* Determines if a particular object placement on the x-z plane is valid. See
     subclasses for details. */

    public virtual bool Passes(Vector3 point, Vector3 boundExtents)
    {
        return true;
    }
}

public class CollisionConstraint : PlacementConstraint
{
    /* Checks if objects are placed far enough apart, so they cannot collide. */

    public Vector3 placedObject;
    public Vector3 boundsOldObjectExtents;

    public override bool Passes(Vector3 placementPt, Vector3 boundsExtents)
    {
        float distance = Vector3.Distance(placementPt, placedObject);

        // we will take the diagonal of the bounding box
        float diagonalBoxOldObject = Mathf.Pow(3, 1 / 3f) * Mathf.Sqrt(Mathf.Pow(boundsOldObjectExtents[0], 2) + Mathf.Pow(boundsOldObjectExtents[1], 2) + Mathf.Pow(boundsOldObjectExtents[2], 2));
        float diagonalBoxCurrentObject = Mathf.Pow(3, 1 / 3f) * Mathf.Sqrt(Mathf.Pow(boundsExtents[0], 2) + Mathf.Pow(boundsExtents[1], 2) + Mathf.Pow(boundsExtents[2], 2));

        float minDistance = diagonalBoxOldObject + diagonalBoxCurrentObject;
            
        return distance > minDistance;
    }
}

public class ClosestPositionConstraint : PlacementConstraint
{
    /* Checks if an object is placed close enough to the drone. */

    public Vector3 drone;
    public Vector3 boundsDroneExtents;
    public float droneClosestLimit;

    public override bool Passes(Vector3 placementPt, Vector3 boundObjectExtents)
    {
        float distance = Vector3.Distance(placementPt, drone);

        float minDistance = droneClosestLimit + Mathf.Pow(3, 1 / 3) * Mathf.Sqrt(Mathf.Pow(boundsDroneExtents[0], 2) + Mathf.Pow(boundsDroneExtents[1], 2) + Mathf.Pow(boundsDroneExtents[2], 2));
        return distance > droneClosestLimit;
    }
}

public class SurfaceObjectPlacer
{
    private FloatParameter random; //[0, 1]
    private ClosestPositionConstraint droneClosestLimit;
    Vector3 boundsBoxExtents;
    Vector3 boxPosition;
    private int maxPlacementTries = 100;


    private List<PlacementConstraint> collisionConstraints = new List<PlacementConstraint>();


    public SurfaceObjectPlacer(
        FloatParameter random,
        ClosestPositionConstraint droneClosestLimit,
        Vector3 boundsBoxExtents,
        Vector3 boxPosition,
        int maxPlacementTries)
    {
        this.random = random;
        this.droneClosestLimit = droneClosestLimit;
        this.boundsBoxExtents = boundsBoxExtents;
        this.boxPosition = boxPosition;
        this.maxPlacementTries = maxPlacementTries;
    }


    public void IterationStart()
    {
        collisionConstraints = new List<PlacementConstraint>();
    }

    public bool PlaceObject(GameObject obj, bool downObject, bool upObject)
    {
        if (obj.activeInHierarchy)
        {
            // try to sample a valid point
            Bounds objBounds = obj.GetComponent<Renderer>().bounds;
            Vector3 boundsObjectExtents = objBounds.extents;
            
            List<PlacementConstraint> constraints = GetAllConstraints();
            Vector3? point = SampleValidGlobalPointOnPlane(boundsBoxExtents, boxPosition, boundsObjectExtents, downObject, upObject, constraints);

            if (point.HasValue)
            {
                // place object
                Vector3 foundPoint = point ?? Vector3.zero;
                obj.transform.position = new Vector3(foundPoint.x, foundPoint.y, foundPoint.z);

                // update constraints so subsequently placed object cannot collide with this one
                CollisionConstraint newConstraint = new CollisionConstraint();
                newConstraint.placedObject = foundPoint;
                newConstraint.boundsOldObjectExtents = boundsObjectExtents;
                collisionConstraints.Add(newConstraint);

            }
            else
            {
                return false;
            }
        }
        return true;

    }

    // PRIVATE HELPERS

    private Vector3? SampleValidGlobalPointOnPlane(Vector3 boundBoxExtents, Vector3 boxPosition, Vector3 boundsObjectExtents, bool downObject, bool upObject, List<PlacementConstraint> constraints)
    {
        // return a valid point and if not found one it return null 
        int tries = 0;

        while (tries < maxPlacementTries)
        {
            Vector3 point = RandomPointInVolume(boundBoxExtents, boxPosition, boundsObjectExtents, downObject, upObject);
            bool valid = PassesConstraints(point, boundsObjectExtents, constraints);
            if (valid) { return point; }

            tries += 1;
        }
        return null;
    }

    private List<PlacementConstraint> GetAllConstraints()
    {
        // return a list of all the constraints: combination of permanent constraint and additional constraint like the maxReachabilityConstraint or the 
        // collision constraint 
        List<PlacementConstraint> allConstraints = new List<PlacementConstraint>();
        allConstraints.AddRange(collisionConstraints);
        allConstraints.Add(droneClosestLimit);
        return allConstraints;
    }

    private Vector3 RandomPointInVolume(Vector3 boundBoxExtents, Vector3 boxPosition, Vector3 boundsObjectExtents, bool downObject, bool upObject)
    {
        float x = - boundBoxExtents[0] + random.Sample() * 2 * boundBoxExtents[0];
        float y; // the y coordinates will might change 
        float z = -boundBoxExtents[2];//+ random.Sample() * 2 * boundBoxExtents[2];

        if (downObject)
        {
            float min = -boundBoxExtents[1];
            float max = -(1 / 3f) * boundBoxExtents[1];
            y = min + (max - min) * random.Sample();
        }
        else if (upObject)
        {
            float min = -(1 / 3f) * boundBoxExtents[1];
            float max = boundBoxExtents[1];
            y = min + (max - min) * random.Sample();            
        }
        else
        {
            float min = -boundBoxExtents[1];
            float max = boundBoxExtents[1];
            y = min + (max - min) * random.Sample();
        }
        
        // then we need to avoid the case where a part of the object is outside of the box
        x = TooCloseToTheBorder(boundBoxExtents[0], x, boundsObjectExtents[0]);
        y = TooCloseToTheBorder(boundBoxExtents[1], y, boundsObjectExtents[1]);
        z = TooCloseToTheBorder(boundBoxExtents[2], z, boundsObjectExtents[2]);
        
        // we need to adjust the position based on the position of the box 
        
        x += boxPosition[0];
        y += boxPosition[1];
        z += boxPosition[2];
        
        return new Vector3(x, y, z);
    }

    private static float TooCloseToTheBorder(float border, float point, float ExtentObject)
    {
        if ((point + Mathf.Sign(point) * ExtentObject) > border)
        {
            point -= Mathf.Sign(point) * ExtentObject;
        }
        return point;
    }
    private static bool PassesConstraints(Vector3 point, Vector3 boundsObjectExtents, List<PlacementConstraint> constraints)
    {
        /* Checks if sampled point on plane passes all provided constraints. */

        foreach (PlacementConstraint constraint in constraints)
        {
            bool pass = constraint.Passes(point, boundsObjectExtents);
            if (!pass) { return false; }
        }
        return true;
    }
}