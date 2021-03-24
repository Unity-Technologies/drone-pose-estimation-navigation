using System;
using System.Collections;
using System.Collections.Generic;
using Grpc.Core;
using Protocolor;
using UnityEngine;

public class TestProtobuf : MonoBehaviour
{

    private ColorGenerator.ColorGeneratorClient _colorGeneratorClient;
    private Channel _channel;
    private readonly string _server = "127.0.0.1:50051";
    void Start()
    {
        _channel = new Channel(_server, ChannelCredentials.Insecure);
        _colorGeneratorClient = new ColorGenerator.ColorGeneratorClient(_channel);
    }

    private void Update()
    {
        GetRandomColor();
        //Get Data from model
        //Update the navmesh plane
        // Apply the quaternion to the drone
        //SetDestination (navmesh)
    }

    public void GetRandomColor()
    {
        var randomColor =
            _colorGeneratorClient.GetRandomColor(new CurrentColor{ Color = ColorUtility.ToHtmlStringRGB(Color.black)});
        
        Debug.Log("random color received: " + randomColor.Color);
    }
}
