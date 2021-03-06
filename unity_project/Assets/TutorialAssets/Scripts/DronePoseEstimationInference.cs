using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
using Grpc.Core;
using Protocolor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class DronePoseEstimationInference : MonoBehaviour
{

    private PostEstimationService.PostEstimationServiceClient _poseEstimationClient;
    private Channel _channel;
    private readonly string _server = "127.0.0.1:50051";
    private int _seq = 0;
    public RenderTexture renderTexture;
    public PoseEstimationScenario scenario;

    public GameObject drone;
    public GameObject target;

    public GameObject _canvas;


    // navmesh stuff
    public NavMeshSurface Surface;
    // private GameObject targetObject;
    // private GameObject droneObject;
    private GameObject planeObject;
    public Material planeMaterial;
    public Camera cam;
    public NavMeshAgent agent;
    
    private bool captureDone = false;


    void Start()
    {
        _channel = new Channel(_server, ChannelCredentials.Insecure);
        _poseEstimationClient = new PostEstimationService.PostEstimationServiceClient(_channel);
    }

    public IEnumerator StartPoseEstimationInternal()
    {
        _canvas.SetActive(false);
        yield return new WaitForEndOfFrame();
        
        var tex2D = new Texture2D(cam.pixelWidth, cam.pixelHeight);
        RenderTexture.active = cam.targetTexture;
        var rect = new Rect(0,0,cam.pixelWidth, cam.pixelHeight);
        tex2D.ReadPixels(rect,0,0);
        tex2D.Apply();
        RenderTexture.active = null;

        var data = tex2D.GetRawTextureData();
        var encodedImageBytes = ImageConversion.EncodeArrayToPNG(data, GraphicsFormat.R8G8B8A8_UNorm,
            (uint) cam.pixelWidth, (uint) cam.pixelHeight);
        var path = Application.persistentDataPath + "Image_" + _seq + ".png";
        _seq++;
        File.WriteAllBytes(path, encodedImageBytes);

        var poseEstimates = GetPoseEstimation(data);
        targetEstimatedPosition = poseEstimates.TargetTransformPosition;//poseEstimates.TargetTransformPosition;
        droneEstimatedPosition = poseEstimates.DroneTransformPosition;
        Debug.Log("Done With capture..: " + targetEstimatedPosition);
        _canvas.SetActive(true);
    }

    public void StartPoseEstimation()
    {
        StartCoroutine(StartPoseEstimationInternal());
    }

    private TransformPosition targetEstimatedPosition;
    private TransformPosition droneEstimatedPosition;
    
    public void StartNavigation()
    {
        // cleanup agent and navmesh plane

        agent.enabled = false;
        planeObject = GameObject.Find("Plane");
        Destroy(planeObject);
        
        var pos = new Vector3(targetEstimatedPosition.X, targetEstimatedPosition.Y, cam.transform.InverseTransformPoint(target.transform.position).z);
        var targetPosition = cam.transform.TransformPoint(pos);
        var dronePosition = Camera.main.transform.TransformPoint(new Vector3(droneEstimatedPosition.X, droneEstimatedPosition.Y, cam.transform.InverseTransformPoint(drone.transform.position).z));
        
        Debug.Log("Target Position : " + target.transform.position + "  Estimated Target Position: " + targetPosition);
        Debug.Log("Drone Position : " + drone.transform.position + "  Estimated Drone Position: " + dronePosition);

        // Get the bottom of box collider y for drawing the plane correctly under the drone
        var actualDronePosition = dronePosition;
        var droneCollider = drone.GetComponent<BoxCollider>();
        var yHalfExtents = droneCollider.bounds.extents.y;
        var yCenter = droneCollider.bounds.center.y;
        float yLower = transform.position.y + (yCenter - yHalfExtents);
        var offsetY = droneCollider.center.normalized.y;
        // set the y of drone to the bottom of box collider
        actualDronePosition.y = yLower - offsetY;

        // Get the top of box collider y for drawing the plane correctly over the target
        var actualTargetPosition = targetPosition;
        var targetCollider = target.GetComponent<BoxCollider>();
        var yHalfExtentsT = targetCollider.bounds.extents.y;
        var yCenterT = targetCollider.bounds.center.y;
        float yUpperT = transform.position.y + (yCenterT + yHalfExtentsT);
        // set the y of target to the top of box collider
        actualTargetPosition.y = yUpperT;

        // compare with ground truth
        // var dronePositionGT = droneObject.transform.position;
        // dronePositionGT.z = yLower - offsetY;
        // var targetPositionGT = targetObject.transform.position;
        // targetPosition.z = yUpperT;

        // Do navmesh calculations with ground truth drone and target positions - comment out for pose estimation demo
        //targetPosition = targetPositionGT;
        //dronePosition = dronePositionGT;

        Vector3 midPointPosition = (actualDronePosition - actualTargetPosition).normalized;

        // create navmesh plane
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.GetComponent<Renderer>().material = planeMaterial;
        plane.transform.position = (actualDronePosition + actualTargetPosition) / 2;
        UnityEngine.Quaternion rot = UnityEngine.Quaternion.LookRotation(midPointPosition, Vector3.up);
        plane.transform.rotation = rot;
        plane.transform.localScale = new Vector3(20, 20, 20);

        // agent.enabled = false;
        // build navmesh
        Surface.BuildNavMesh();

        // restart the agent
        agent.enabled = true;
        // agent.isStopped = true;
        // move agent to target
        // agent.Warp(dronePosition);
        agent.SetDestination(targetPosition);
        // agent.isStopped = false;
    }
    
    
    public void GenerateNewEnvironment()
    {
        // cleanup agent and navmesh plane
        agent.enabled = false;
        planeObject = GameObject.Find("Plane");
        Destroy(planeObject);

        scenario.Move();
    }

    public PoseEstimationResponse GetPoseEstimation(byte[] encodedImageData)
    {
        return _poseEstimationClient.GetPoseEstimation(new ImageInfo {Image = ByteString.CopyFrom(encodedImageData)});
    }

}
