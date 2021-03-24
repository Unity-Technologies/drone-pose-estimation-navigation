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
    void Start()
    {
        _channel = new Channel(_server, ChannelCredentials.Insecure);
        _poseEstimationClient = new PostEstimationService.PostEstimationServiceClient(_channel);
    }

    private void Update()
    {
        // var rawBytes = CaptureScreenshot();
        // var poseEstimation = GetPoseEstimation(rawBytes);
        //
        // Debug.Log("PoseEstimationData for drone quaternion: " +
        //           poseEstimation.DroneQuaternion.X + ", " + poseEstimation.DroneQuaternion.Y + ", " +
        //           poseEstimation.DroneQuaternion.Z +
        //           ", " + poseEstimation.DroneQuaternion.W);

        //Get Data from model
        //Update the navmesh plane
        // Apply the quaternion to the drone
        //SetDestination (navmesh)
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

    public void StartPoseEstimation()
    {
        var rawBytes = CaptureScreenshot();
        var poseEstimation = GetPoseEstimation(rawBytes);

        Debug.Log("PoseEstimationData for drone quaternion: " +
                  poseEstimation.DroneQuaternion.X + ", " + poseEstimation.DroneQuaternion.Y + ", " +
                  poseEstimation.DroneQuaternion.Z +
                  ", " + poseEstimation.DroneQuaternion.W);
        var droneEstimatedPose = poseEstimation.DroneQuaternion;
        var targetEstimatedPose = poseEstimation.TargetQuaternion;
        var targetEstimatedPosition = poseEstimation.TargetTransformPosition;
        drone.transform.rotation = new Quaternion(droneEstimatedPose.X, droneEstimatedPose.Y, droneEstimatedPose.Z, droneEstimatedPose.W);
        target.transform.rotation = new Quaternion(targetEstimatedPose.X, targetEstimatedPose.Y, targetEstimatedPose.Z, targetEstimatedPose.W);
        target.transform.position = new Vector3(targetEstimatedPosition.X, targetEstimatedPosition.Y, targetEstimatedPosition.Z);
        
        // Do next steps here..
        // Apply the data
        // Navmesh building etc...
    }

    public void GenerateNewEnvironment()
    {
        scenario.Move();
    }

    public PoseEstimationResponse GetPoseEstimation(byte[] encodedImageData)
    {
        return _poseEstimationClient.GetPoseEstimation(new ImageInfo {Image = ByteString.CopyFrom(encodedImageData)});
    }
}
