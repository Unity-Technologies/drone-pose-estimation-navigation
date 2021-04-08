using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
using Grpc.Core;
using Protocolor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Perception.Randomization.Scenarios;
using Quaternion = UnityEngine.Quaternion;

using System.Numerics;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class TestProtobuf : MonoBehaviour
{

    private PostEstimationService.PostEstimationServiceClient _poseEstimationClient;
    private Channel _channel;
    private readonly string _server = "127.0.0.1:50051";
    private int _seq = 0;
    public RenderTexture renderTexture;
    public PoseEstimationScenario scenario;

    public GameObject drone;
    public GameObject target;


    // navmesh stuff
    public NavMeshSurface Surface;
    private GameObject targetObject;
    private GameObject droneObject;
    private GameObject planeObject;
    public Material planeMaterial;
    public Camera cam;
    public NavMeshAgent agent;

    // post process volume
    public CustomModifyPostProcessVolume ppv;


    void Start()
    {
        _channel = new Channel(_server, ChannelCredentials.Insecure);
        _poseEstimationClient = new PostEstimationService.PostEstimationServiceClient(_channel);
    }

    private byte[] CaptureScreenshot()
    {
        Camera.main.targetTexture = renderTexture;
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;
        Camera.main.Render();
        Texture2D mainCameraTexture = new Texture2D(renderTexture.width, renderTexture.height);
        mainCameraTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        mainCameraTexture.Apply();
        RenderTexture.active = currentRT;
        // Get the raw byte info from the screenshot
        byte[] imageBytes = mainCameraTexture.GetRawTextureData();
        var encodedImageBytes = ImageConversion.EncodeArrayToPNG(imageBytes, GraphicsFormat.R8G8B8A8_UNorm,
            (uint) renderTexture.width, (uint) renderTexture.height);
        var path = Application.persistentDataPath + "Image_" + _seq + ".png";
        _seq++;
        File.WriteAllBytes(path, encodedImageBytes);
        Camera.main.targetTexture = null;
        return imageBytes;
    }


    private Protocolor.Quaternion droneEstimatedPose;
    private Protocolor.Quaternion targetEstimatedPose;
    private TransformPosition targetEstimatedPosition;
    private TransformPosition droneEstimatedPosition;
    public void StartPoseEstimation()
    {
        // cleanup agent and navmesh plane
        agent.enabled = false;
        planeObject = GameObject.Find("Plane");
        Destroy(planeObject);

        var rawBytes = CaptureScreenshot();
        var poseEstimation = GetPoseEstimation(rawBytes);

        Debug.Log("PoseEstimationData for drone quaternion: " +
                  poseEstimation.DroneQuaternion.X + ", " + poseEstimation.DroneQuaternion.Y + ", " +
                  poseEstimation.DroneQuaternion.Z +
                  ", " + poseEstimation.DroneQuaternion.W);
        droneEstimatedPose = poseEstimation.DroneQuaternion;
        targetEstimatedPose = poseEstimation.TargetQuaternion;
        targetEstimatedPosition = poseEstimation.TargetTransformPosition;
        droneEstimatedPosition = poseEstimation.DroneTransformPosition;

    }

    public void StartNavigation()
    {
        // cleanup agent and navmesh plane
        agent.enabled = false;
        planeObject = GameObject.Find("Plane");
        Destroy(planeObject);
        
        // var estimatedPosition = Camera.main.transform.TransformPoint(translationObject);
        // var estimatedRotation = Camera.main.transform.rotation * rotationObject;
        
        //Debug.Log("TargetPosition : " + target.transform.position);
        var pos = new Vector3(targetEstimatedPosition.X, targetEstimatedPosition.Y, targetEstimatedPosition.Z);

        targetObject = GameObject.Find("TargetCube_modified");
        var targetPosition = Camera.main.transform.TransformPoint(pos);
        //Debug.Log("Estimated Target Position: " + targetPosition);

        droneObject = GameObject.Find("Drone_01 Variant_modified");
        var dronePosition = Camera.main.transform.TransformPoint(new Vector3(droneEstimatedPosition.X, droneEstimatedPosition.Y, droneEstimatedPosition.Z));

        // Get the bottom of box collider y for drawing the plane correctly under the drone
        var droneCollider = droneObject.GetComponentInChildren<BoxCollider>();
        var yHalfExtents = droneCollider.bounds.extents.y;
        var yCenter = droneCollider.bounds.center.y;
        float yLower = transform.position.y + (yCenter - yHalfExtents);
        var offsetY = droneCollider.center.y;
        // set the y of drone to the bottom of box collider
        dronePosition.y = yLower - offsetY;

        // Get the top of box collider y for drawing the plane correctly over the target
        var targetCollider = targetObject.GetComponentInChildren<BoxCollider>();
        var yHalfExtentsT = targetCollider.bounds.extents.y;
        var yCenterT = targetCollider.bounds.center.y;
        float yUpperT = transform.position.y + (yCenterT + yHalfExtentsT);
        // set the y of target to the top of box collider
        targetPosition.y = yUpperT;

        // compare with ground truth
        var dronePositionGT = droneObject.transform.position;
        dronePositionGT.y = yLower - offsetY;
        var targetPositionGT = targetObject.transform.position;
        targetPosition.y = yUpperT;
        Debug.Log("Target Position : " + targetPositionGT + "  Estimated Target Position: " + targetPosition);
        Debug.Log("Drone Position : " + dronePositionGT + "  Estimated Drone Position: " + dronePosition);

        // Do navmesh calculations with ground truth drone and target positions - comment out for pose estimation demo
        //targetPosition = targetPositionGT;
        //dronePosition = dronePositionGT;

        Vector3 midPointPosition = (targetPosition - dronePosition).normalized;

        // create navmesh plane
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.GetComponent<Renderer>().material = planeMaterial;
        plane.transform.position = (dronePosition + targetPosition) / 2;
        UnityEngine.Quaternion rot = UnityEngine.Quaternion.LookRotation(midPointPosition, Vector3.up);
        plane.transform.rotation = rot;
        plane.transform.localScale = new Vector3(20, 20, 20);

        // build navmesh
        Surface.BuildNavMesh();

        // restart the agent
        agent.enabled = true;

        // move agent to target
        agent.SetDestination(targetPosition);

    }



    public void GenerateNewEnvironment()
    {
        // cleanup agent and navmesh plane
        agent.enabled = false;
        planeObject = GameObject.Find("Plane");
        Destroy(planeObject);

        scenario.Move();
        ppv.CustomUpdate();
    }

    public PoseEstimationResponse GetPoseEstimation(byte[] encodedImageData)
    {
        return _poseEstimationClient.GetPoseEstimation(new ImageInfo {Image = ByteString.CopyFrom(encodedImageData)});
    }

}
