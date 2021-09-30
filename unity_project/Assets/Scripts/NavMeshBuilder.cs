using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class NavMeshBuilder : MonoBehaviour
{
    public NavMeshSurface Surface;

    public GameObject targetObject;
    public GameObject droneObject;

    private GameObject planeObject;

    public Material planeMaterial;

    private int _counter = 1; //20;
    
    // void FixedUpdate()
    // {
    //     //InvokeRepeating("SpawnObjects", 2, 2);
    //     SpawnObjects();

    //     //HidePlaneMesh();
    // }

    private void Start()
    {
        targetObject = GameObject.Find("Target");
        var targetPosition = targetObject.transform.position;

        droneObject = GameObject.Find("DroneModel");
        var dronePosition = droneObject.transform.position;

        // Get the bottom of box collider y for drawing the plane correctly under the drone
        var droneCollider = droneObject.GetComponentInChildren<BoxCollider>();
        var yHalfExtents = droneCollider.bounds.extents.y;
        var yCenter = droneCollider.bounds.center.y;
        //float yUpper = transform.position.y + (yCenter + yHalfExtents);
        float yLower = transform.position.y + (yCenter - yHalfExtents);
        //Debug.Log("Drone Upper border: " + yUpper);
        //Debug.Log("Drone Lower border: " + yLower);
        var offsetY = droneCollider.center.y;
        // set the y of drone to the bottom of box collider
        dronePosition.y = yLower - offsetY;

        // Get the top of box collider y for drawing the plane correctly over the target
        var targetCollider = targetObject.GetComponentInChildren<BoxCollider>();
        var yHalfExtentsT = targetCollider.bounds.extents.y;
        var yCenterT = targetCollider.bounds.center.y;
        float yUpperT = transform.position.y + (yCenterT + yHalfExtentsT);
        //float yLowerT = transform.position.y + (yCenterT - yHalfExtentsT);
        //Debug.Log("Target Upper border: " + yUpperT);
        //Debug.Log("Target Lower border: " + yLowerT);
        // set the y of target to the top of box collider
        targetPosition.y = yUpperT;

        Vector3 midPointPosition = (targetPosition - dronePosition).normalized;

        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //plane.GetComponent<Renderer>().material.color = new Color(0.5f, 1, 1, 0);
        //plane.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);

        plane.GetComponent<Renderer>().material = planeMaterial;
        plane.transform.position = (dronePosition + targetPosition) / 2;
        UnityEngine.Quaternion rot = UnityEngine.Quaternion.LookRotation(midPointPosition, Vector3.up);
        plane.transform.rotation = rot;
        plane.transform.localScale = new Vector3(20, 20, 20);

        Surface.BuildNavMesh();
        Debug.Log("built navmesh");
    }

    void SpawnObjects()
    {


        if (_counter > 0)
        {
            // var rx = Random.Range(-5f, 5f);
            // var rz = Random.Range(-5f, 5f);
            // var ry = Random.Range(-1f, 2f);
            // Vector3 pos = new Vector3(rx, ry, rz);
            // GenerateLevel(pos);
            Surface.BuildNavMesh();
            --_counter;  
        }
    }

    void GenerateLevel(Vector3 pos)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = pos; 
        sphere.transform.localScale = new Vector3(1,1,1);
    }


    //void HidePlaneMesh()
    //{
    //    planeObject = GameObject.Find("Plane");
    //    //planeObject.GetComponent<MeshRenderer>().enabled = false;
    //    //planeObject.active = true;
    //    //planeObject.GetComponent<Renderer>().material.color.a.Equals(0);
    //}
}
