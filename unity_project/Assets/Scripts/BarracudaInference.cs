using System.Collections;
using System.Collections.Generic;
using System.IO;
using Protocolor;
using Unity.Barracuda;
using Unity.Simulation;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class BarracudaInference : MonoBehaviour
{

    public Model dronePoseEstimationModel;

    public NNModel nnmodel;

    public Camera mainCamera;

    public GameObject canvas;

    public IWorker engine;

    public DronePoseEstimationInference grpcInference;
    // Start is called before the first frame update
    void Start()
    {
        Unity.Barracuda.ComputeInfo.channelsOrder = ComputeInfo.ChannelsOrder.NCHW;
        dronePoseEstimationModel = ModelLoader.Load(nnmodel, false);
        Debug.Log($"Name - {dronePoseEstimationModel.outputs}");
        engine = WorkerFactory.CreateWorker(type: WorkerFactory.Type.CSharp, model: dronePoseEstimationModel, verbose: true);
    }

    public void StartInference()
    {
        StartCoroutine(StartPoseEstimationInternal());
    }

    public RenderTexture rt;
    
    Vector2 scale = new Vector2(1,-1);
    private Vector2 offset = Vector2.up;
    public IEnumerator StartPoseEstimationInternal()
    {
        canvas.SetActive(false);
        yield return new WaitForEndOfFrame();
        // rt = new RenderTexture(224,224,0);
        // _scale.x = mainCamera.pixelHeight / mainCamera.pixelWidth;
        // _offset.x = (1 - _scale.x) / 2f;
        Graphics.Blit(mainCamera.targetTexture, rt);
        
        
        //FlipY
        var rt2 = RenderTexture.GetTemporary(rt.width, rt.height,0,rt.graphicsFormat);
        
        Graphics.Blit(rt, rt2, scale, offset);
        var tex = new Texture2D(rt.width,rt.height,TextureFormat.RGBA32, false, false);
        var rect = new Rect(0,0,224,224);
        RenderTexture.active = rt2;//mainCamera.targetTexture;
        tex.ReadPixels(rect,0,0);
        tex.Apply();
        RenderTexture.active = null;
        var pixels = tex.GetPixels();
        var encodedData =
            ImageConversion.EncodeArrayToPNG(tex.GetRawTextureData(), GraphicsFormat.R8G8B8A8_UNorm, (uint)rt.width, (uint)rt.height);
        
        File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "image_barracuda.png"), encodedData);
        //var input = TransformInput(pixels);
        var tensor = new Tensor(rt2, 3, "test");
        //Debug.Log(y.shape);
        var output = engine.Execute(tensor);
        var data = output.CopyOutput().data;
        var outputshape = new TensorShape(1,1,1,2);
        var inf = data.Download(outputshape);
        var inf2 = output.PeekOutput("82").data.Download(outputshape);
        Debug.Log("Inference : " + inf2[0]);
        
        grpcInference.targetEstimatedPosition = new TransformPosition()
        {
            X = inf[0],
            Y = inf[1],
            Z = 0
        };

        grpcInference.droneEstimatedPosition = new TransformPosition()
        {
            X = inf2[0],
            Y = inf2[1],
            Z = 0
        };
        
        grpcInference.StartNavigation();
        
        //var dataFromGrpcInf = grpcInference.GetPoseEstimation(tex.GetRawTextureData());
        //Debug.Log("From GRPC-Python inf: " + dataFromGrpcInf.TargetTransformPosition);

        //var data = tex2D.GetRawTextureData();
        
        

        canvas.SetActive(true);
    }

    Vector2 _scale = new Vector2(1,1);
    private Vector2 _offset = Vector2.zero;

    Tensor TransformInput(Color[] pixels)
    {
        float[] transformedPixels = new float[pixels.Length*3];
        int index = 0;
        for (int i = 0; i < pixels.Length; i++)
        {
            transformedPixels[index++] = pixels[i].r;
            transformedPixels[index++] = pixels[i].g;
            transformedPixels[index++] = pixels[i].b;
        }
        
        return new Tensor(1, 224,224,3,transformedPixels);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
