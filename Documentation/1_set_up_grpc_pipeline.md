#  gRPC Setup

## Protobuf definition

First we define the interface definition by creating a protobuf file as shown below

```syntax = "proto3";

package protocolor;

service PostEstimationService {
    rpc GetPoseEstimation (ImageInfo) returns (PoseEstimationResponse);
}

message ImageInfo {
    bytes image = 1;
}

message PoseEstimationResponse {
    Quaternion droneQuaternion = 1;
    Quaternion targetQuaternion = 2;
    TransformPosition droneTransformPosition = 3;
    TransformPosition targetTransformPosition = 4;
}
message Quaternion {
    float x = 1;
    float y = 2;
    float z = 3;
    float w = 4;
}

message TransformPosition {
    float x = 1;
    float y = 2;
    float z = 3;
}
```

The protobuf file provides a service definition `PostEstimationService` which contains definition of the `rpc` call. Custom datatypes that rpc call takes in are defined as protobuf message. In this case, the `GetPoseEstimation` function expects a stream of bytes of the image as an input and returns `PoseEstimationResponse` which contains drone and target quaternion and position.

## Stubs generation

Now that we have protobuf definition for the PoseEstimationService, we can go ahead and generate stubs for our client and server. In this case, unity process requesting for poseEstimation will act as a client and python service serving the prediction will act as a server. So  we will go ahead and create c# stub for client which can be included in our Unity Project assets and python stub for the server.

C# stubs generation for unity
`protoc -I grpc/ --csharp_out=grpc/client --grpc_out=grpc/client --plugin=protoc-gen-grpc=/usr/local/bin/grpc_csharp_plugin PoseEstimation.proto`

This will generate two `.cs` files `PostEstimation.cs` and `PostEstimationGrpc.cs` under client directory.

Python stub generation for server
`python -m grpc_tools.protoc -I grpc/ --python_out=. --grpc_python_out=. PostEstimation.proto`

This will generate `PostEstimation_pb2_grpc.py` and `PostEstimation_pb2.py`. All of our server side code is under `inference` directory. Let move these stubs generated to that directory.

### Server side setup
Now that we have all stubs in place, we can go ahead and provide implementation for the interface method `GetPoseEstimation`. Inside server.py you can see how we have provided the implementation of the RPC call. Every time the client invokes this remote procedure call, inside our implementation, we forward the data passed in by the client  to the mode, get the model prediction and pass back the response to unity.


### Client side setup
On the client side, we need to import all the GRPC and Protobuf related dlls into the project. Prebuilt binaries can be found [here](https://packages.grpc.io/archive/2019/11/6950e15882f28e43685e948a7e5227bfcef398cd-6d642d6c-a6fc-4897-a612-62b0a3c9026b/index.xml).
Import these dlls in your project. In this project we have placed these dlls under `Assets/Plugins`. 
Now go ahead and create a channel and a GRPC service client to make the remote procedure call.

```
void Start()
{
    _channel = new Channel(_server, ChannelCredentials.Insecure);
    _poseEstimationClient = new PostEstimationService.PostEstimationServiceClient(_channel);
}
```

After this, you can invoke the `GetPoseEstimation` function using `_poseEstimationClient` instance.

