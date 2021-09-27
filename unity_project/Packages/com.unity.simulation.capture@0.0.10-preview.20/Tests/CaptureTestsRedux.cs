using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Unity.Collections;
using Unity.Simulation;

using UnityEngine.TestTools;
using NUnit.Framework;
#if ENABLE_CLOUDTESTS
using Unity.Simulation.Tools;
#endif

/// Note: add parametric variant for UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData.FlipYMode.ForceFlipY

using Channel = Unity.Simulation.CaptureCamera.Channel;

public class CaptureTestsRedux
{
    ///
    /// Color Orientation Tests
    ///

    [UnityTest]
    public IEnumerator CaptureTestsRedux_Async_Color_Orientation()
    {
        if (!GraphicsUtilities.SupportsAsyncReadback()) Assert.Ignore();
        return CaptureTestsRedux_Parametric(true, false, false, Channel.Color);
    }

    [UnityTest]
    public IEnumerator CaptureTestsRedux_AsyncRT_Color_Orientation()
    {
        if (!GraphicsUtilities.SupportsAsyncReadback()) Assert.Ignore();
        return CaptureTestsRedux_Parametric(true, false, true, Channel.Color);
    }

#if UNITY_2019_3_OR_NEWER
    [UnityTest]
    public IEnumerator CaptureTestsRedux_AsyncBatched_Color_Orientation()
    {
        if (!GraphicsUtilities.SupportsAsyncReadback()) Assert.Ignore();
        return CaptureTestsRedux_Parametric(true, true, false, Channel.Color);
    }

    [UnityTest]
    public IEnumerator CaptureTestsRedux_AsyncBatchedRT_Color_Orientation()
    {
        if (!GraphicsUtilities.SupportsAsyncReadback()) Assert.Ignore();
        return CaptureTestsRedux_Parametric(true, true, true, Channel.Color);
    }
#endif

    [UnityTest]
    public IEnumerator CaptureTestsRedux_Slow_Color_Orientation()
    {
        return CaptureTestsRedux_Parametric(false, false, false, Channel.Color);
    }

    [UnityTest]
    public IEnumerator CaptureTestsRedux_SlowRT_Color_Orientation()
    {
        return CaptureTestsRedux_Parametric(false, false, true, Channel.Color);
    }

#if UNITY_2019_3_OR_NEWER
    [UnityTest]
    public IEnumerator CaptureTestsRedux_SlowBatched_Color_Orientation()
    {
        return CaptureTestsRedux_Parametric(false, true, false, Channel.Color);
    }

    [UnityTest]
    public IEnumerator CaptureTestsRedux_SlowBatchedRT_Color_Orientation()
    {
        return CaptureTestsRedux_Parametric(false, true, true, Channel.Color);
    }
#endif

    ///
    /// Depth Orientation Tests
    ///

    [UnityTest]
    public IEnumerator CaptureTestsRedux_Async_Depth_Orientation_float32()
    {
        if (!GraphicsUtilities.SupportsAsyncReadback()) Assert.Ignore();
        return CaptureTestsRedux_Parametric(true, false, false, Channel.Depth);
    }

    [UnityTest]
    public IEnumerator CaptureTestsRedux_AsyncRT_Depth_Orientation_float32()
    {
        if (!GraphicsUtilities.SupportsAsyncReadback()) Assert.Ignore();
        return CaptureTestsRedux_Parametric(true, false, true, Channel.Depth);
    }

#if UNITY_2019_3_OR_NEWER
    [UnityTest]
    public IEnumerator CaptureTestsRedux_AsyncBatched_Depth_Orientation_float32()
    {
        if (!GraphicsUtilities.SupportsAsyncReadback()) Assert.Ignore();
        return CaptureTestsRedux_Parametric(true, true, false, Channel.Depth);
    }

    [UnityTest]
    public IEnumerator CaptureTestsRedux_AsyncBatchedRT_Depth_Orientation_float32()
    {
        if (!GraphicsUtilities.SupportsAsyncReadback()) Assert.Ignore();
        return CaptureTestsRedux_Parametric(true, true, true, Channel.Depth);
    }
#endif

    [UnityTest]
    public IEnumerator CaptureTestsRedux_Slow_Depth_Orientation_float32()
    {
        return CaptureTestsRedux_Parametric(false, false, false, Channel.Depth);
    }

    [UnityTest]
    public IEnumerator CaptureTestsRedux_SlowRT_Depth_Orientation_float32()
    {
        return CaptureTestsRedux_Parametric(false, false, true, Channel.Depth);
    }

#if UNITY_2019_3_OR_NEWER
    [UnityTest]
    public IEnumerator CaptureTestsRedux_SlowBatched_Depth_Orientation_float32()
    {
        return CaptureTestsRedux_Parametric(false, true, false, Channel.Depth);
    }

    [UnityTest]
    public IEnumerator CaptureTestsRedux_SlowBatchedRT_Depth_Orientation_float32()
    {
        return CaptureTestsRedux_Parametric(false, true, true, Channel.Depth);
    }
#endif

    [UnityTest]
    public IEnumerator CaptureTestsRedux_Async_Depth_Orientation_short16()
    {
        if (!GraphicsUtilities.SupportsAsyncReadback()) Assert.Ignore();
        return CaptureTestsRedux_Parametric(true, false, false, Channel.Depth, 16);
    }

    [UnityTest]
    public IEnumerator CaptureTestsRedux_AsyncRT_Depth_Orientation_short16()
    {
        if (!GraphicsUtilities.SupportsAsyncReadback()) Assert.Ignore();
        return CaptureTestsRedux_Parametric(true, false, true, Channel.Depth, 16);
    }

#if UNITY_2019_3_OR_NEWER
    [UnityTest]
    public IEnumerator CaptureTestsRedux_AsyncBatched_Depth_Orientation_short16()
    {
        if (!GraphicsUtilities.SupportsAsyncReadback()) Assert.Ignore();
        return CaptureTestsRedux_Parametric(true, true, false, Channel.Depth, 16);
    }

    [UnityTest]
    public IEnumerator CaptureTestsRedux_AsyncBatchedRT_Depth_Orientation_short16()
    {
        if (!GraphicsUtilities.SupportsAsyncReadback()) Assert.Ignore();
        return CaptureTestsRedux_Parametric(true, true, true, Channel.Depth, 16);
    }
#endif

    [UnityTest]
    public IEnumerator CaptureTestsRedux_Slow_Depth_Orientation_short16()
    {
        return CaptureTestsRedux_Parametric(false, false, false, Channel.Depth, 16);
    }

    [UnityTest]
    public IEnumerator CaptureTestsRedux_SlowRT_Depth_Orientation_short16()
    {
        return CaptureTestsRedux_Parametric(false, false, true, Channel.Depth, 16);
    }

#if UNITY_2019_3_OR_NEWER
    [UnityTest]
    public IEnumerator CaptureTestsRedux_SlowBatched_Depth_Orientation_short16()
    {
        return CaptureTestsRedux_Parametric(false, true, false, Channel.Depth, 16);
    }

    [UnityTest]
    public IEnumerator CaptureTestsRedux_SlowBatchedRT_Depth_Orientation_short16()
    {
        return CaptureTestsRedux_Parametric(false, true, true, Channel.Depth, 16);
    }
#endif

    ///
    /// Motion Orientation Tests
    ///

    [UnityTest]
    public IEnumerator CaptureTestsRedux_Async_Motion_Orientation()
    {
        if (!GraphicsUtilities.SupportsAsyncReadback()) Assert.Ignore();
        return CaptureTestsRedux_Parametric(true, false, false, Channel.Motion);
    }

    [UnityTest]
    public IEnumerator CaptureTestsRedux_AsyncRT_Motion_Orientation()
    {
        if (!GraphicsUtilities.SupportsAsyncReadback()) Assert.Ignore();
        return CaptureTestsRedux_Parametric(true, false, true, Channel.Motion);
    }

#if UNITY_2019_3_OR_NEWER
    [UnityTest]
    public IEnumerator CaptureTestsRedux_AsyncBatched_Motion_Orientation()
    {
        if (!GraphicsUtilities.SupportsAsyncReadback()) Assert.Ignore();
        return CaptureTestsRedux_Parametric(true, true, false, Channel.Motion);
    }

    [UnityTest]
    public IEnumerator CaptureTestsRedux_AsyncBatchedRT_Motion_Orientation()
    {
        if (!GraphicsUtilities.SupportsAsyncReadback()) Assert.Ignore();
        return CaptureTestsRedux_Parametric(true, true, true, Channel.Motion);
    }
#endif

    [UnityTest]
    public IEnumerator CaptureTestsRedux_Slow_Motion_Orientation()
    {
        return CaptureTestsRedux_Parametric(false, false, false, Channel.Motion);
    }

    [UnityTest]
    public IEnumerator CaptureTestsRedux_SlowRT_Motion_Orientation()
    {
        return CaptureTestsRedux_Parametric(false, false, true, Channel.Motion);
    }

#if UNITY_2019_3_OR_NEWER
    [UnityTest]
    public IEnumerator CaptureTestsRedux_SlowBatched_Motion_Orientation()
    {
        return CaptureTestsRedux_Parametric(false, true, false, Channel.Motion);
    }

    [UnityTest]
    public IEnumerator CaptureTestsRedux_SlowBatchedRT_Motion_Orientation()
    {
        return CaptureTestsRedux_Parametric(false, true, true, Channel.Motion);
    }
#endif

    public IEnumerator CaptureTestsRedux_Parametric(bool async, bool batched, bool useRT, Channel channel, int depthBits = 32)
    {
        Debug.Assert(depthBits == 16 || depthBits == 32);

        CaptureOptions.useAsyncReadbackIfSupported = async;
#if UNITY_2019_3_OR_NEWER
        CaptureOptions.useBatchReadback = batched;
        if (batched)
            BatchReadback.Instance.BatchSize = 1;
#endif

        var scene = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Capture.Test.Scene"));

        var block1 = scene.transform.Find("Top Cube").gameObject;
        Debug.Assert(block1 != null);
        var unlitRed = new Material(Shader.Find("Hidden/DataCaptureTestsUnlitShader"));
        unlitRed.color = Color.red;
        block1.GetComponent<MeshRenderer>().sharedMaterial = unlitRed;


        var block2 = scene.transform.Find("Bottom Cube").gameObject;
        Debug.Assert(block2 != null);
        var unlitBlack = new Material(Shader.Find("Hidden/DataCaptureTestsUnlitShader"));
        unlitBlack.color = Color.black;
        block2.GetComponent<MeshRenderer>().sharedMaterial = unlitBlack;

        var camera = Camera.main;
        Debug.Assert(camera != null);

        camera.depthTextureMode = DepthTextureMode.Depth | DepthTextureMode.DepthNormals | DepthTextureMode.MotionVectors;

        var delay = 20;
        while (delay-- > 0)
            yield return null;

        var colorFormat  = GraphicsFormat.R8G8B8A8_UNorm;
        var depthFormat  = depthBits == 16 ? GraphicsFormat.R16_UNorm : GraphicsFormat.R32_SFloat;
        var motionFormat = GraphicsFormat.R8G8B8A8_UNorm; 

        var label = $"CaptureTestRedux_{(async?"Async":"Slow")}{(batched?"Batched":"")}{(useRT?"RT":"")}_{channel.ToString()}_Orientation{(channel == Channel.Depth ? $"_Depth{depthBits}" : "")}";
        var path  = Path.Combine(Application.persistentDataPath, label);

        var request = CaptureDataForChannel(camera, channel, colorFormat, depthFormat, motionFormat);

        var count = 20;
        while (!request.completed && count-- > 0)
            yield return null;
        Assert.False(request.error);

        switch (channel)
        {
            case Channel.Color:
                Assert.NotNull(request.data.colorBuffer);
                Assert.True(BufferNotEmpty((byte[])request.data.colorBuffer, ArrayUtilities.Count<byte>((byte[])request.data.colorBuffer)));
                Test_Color_Orientation(request.data.camera, colorFormat, (byte[])request.data.colorBuffer, label, path);
                break;

            case Channel.Depth:
                Assert.NotNull(request.data.depthBuffer);
                switch (depthFormat)
                {
                    case GraphicsFormat.R32_SFloat:
                        Assert.True(BufferNotEmpty(ArrayUtilities.Cast<float>((Array)request.data.depthBuffer), ArrayUtilities.Count((byte[])request.data.depthBuffer)));
                        Test_Depth_Orientation_float(request.data.camera, depthFormat, (byte[])request.data.depthBuffer, label, path);
                        break;
                    case GraphicsFormat.R16_UNorm:
                        Assert.True(BufferNotEmpty(ArrayUtilities.Cast<ushort>((Array)request.data.depthBuffer), ArrayUtilities.Count((byte[])request.data.depthBuffer)));
                        Test_Depth_Orientation_ushort(request.data.camera, depthFormat, (byte[])request.data.depthBuffer, label, path);
                        break;
                    default:
                        throw new InvalidOperationException("Invalid depth format");
                }
                break;

            case Channel.Motion:
                Assert.Ignore();
                break;
        }

        var rt = camera.targetTexture;
        if (rt != null)
        {
            rt.Release();
            camera.targetTexture = null;
        }

        UnityEngine.Object.DestroyImmediate(scene);
        scene = null;
    }

    AsyncRequest<CaptureCamera.CaptureState> CaptureDataForChannel(Camera camera, Channel channel, GraphicsFormat colorFormat, GraphicsFormat depthFormat, GraphicsFormat motionFormat)
    {
        Func<AsyncRequest<CaptureCamera.CaptureState>, AsyncRequest<CaptureCamera.CaptureState>.Result> functor = r =>
        {
            return AsyncRequest.Result.Completed;
        };

        return CaptureCamera.Capture
        (
            camera,
            channel == Channel.Color ? functor : null,
            colorFormat,
            channel == Channel.Depth ? functor : null,
            depthFormat,
            channel == Channel.Motion ? functor : null,
            motionFormat,
            forceFlip: ForceFlip.None
        );
    }

    void Test_Color_Orientation(Camera camera, GraphicsFormat format, byte[] data, string label, string path)
    {
        var colors = ArrayUtilities.Cast<Color32>(data);
        var length = ArrayUtilities.Count<Color32>(colors);

        var top = (Color)colors[length-1];
        var bot = (Color)colors[0];

        // HDRP has all sorts of effects that alter color significantly, so we compare differently.
        if (SRPSupport.GetCurrentPipelineRenderingType() == RenderingPipelineType.URP ||
            SRPSupport.GetCurrentPipelineRenderingType() == RenderingPipelineType.HDRP)
        {
            var failed = !ColorCloser(top, Color.red, Color.black) || !ColorCloser(bot, Color.black, Color.red);

            if (failed)
                Log.I($"{label} : Failed: Colors top {top.ToString()} bottom {bot.ToString()}");

            var texture = ConvertToRGBATexture(format, data, camera.pixelWidth, camera.pixelHeight);
            File.WriteAllBytes(CaptureImageEncoder.EnforceFileExtension(failed ? path + "_FAILED" : path, CaptureImageEncoder.ImageFormat.Jpg), texture.EncodeToJPG());

            Assert.True(ColorCloser(top, Color.red,   Color.black));
            Assert.True(ColorCloser(bot, Color.black, Color.red));
        }
        else
        {
            var tolerance = 0.01f;

            var failed = !CompareColors(top, Color.red, tolerance) || !CompareColors(bot, Color.black, tolerance);

            if (failed)
                Log.I($"{label} : Failed: Colors top {top.ToString()} bottom {bot.ToString()}");

            var texture = ConvertToRGBATexture(format, data, camera.pixelWidth, camera.pixelHeight);
            File.WriteAllBytes(CaptureImageEncoder.EnforceFileExtension(failed ? path + "_FAILED" : path, CaptureImageEncoder.ImageFormat.Jpg), texture.EncodeToJPG());

            Assert.True(CompareColors(top, Color.red,   tolerance));
            Assert.True(CompareColors(bot, Color.black, tolerance));
        }
    }

    void Test_Depth_Orientation_float(Camera camera, GraphicsFormat format, byte[] data, string label, string path)
    {
        var depth  = ArrayUtilities.Cast<float>(data);
        var length = ArrayUtilities.Count<float>(depth);

        // Depth is laid out left to right, bottom to top.
        var top = CalculateAverageDepthForLine(depth, camera.pixelWidth, camera.pixelHeight - 1);
        var bot = CalculateAverageDepthForLine(depth, camera.pixelWidth, 0);

        // Top should be closer, and both planes should have depth info.
        var condition = top < bot && top > 0 && top < 1 && bot > 0 && bot < 1;

        var texture = ConvertToRGBATexture(format, data, camera.pixelWidth, camera.pixelHeight);
        File.WriteAllBytes(CaptureImageEncoder.EnforceFileExtension(condition ? path : path + "_FAILED", CaptureImageEncoder.ImageFormat.Jpg), texture.EncodeToJPG());

        Log.I($"{label} : Average depth top({top}) bottom({bot})");

        Assert.True(condition);
    }

    void Test_Depth_Orientation_ushort(Camera camera, GraphicsFormat format, byte[] data, string label, string path)
    {
        var depth  = ArrayUtilities.Cast<ushort>(data);
        var length = ArrayUtilities.Count<ushort>(depth);

        // Depth is laid out left to right, bottom to top.
        var top = CalculateAverageDepthForLine(depth, camera.pixelWidth, camera.pixelHeight - 1);
        var bot = CalculateAverageDepthForLine(depth, camera.pixelWidth, 0);

        // Top should be closer, and both planes should have depth info.
        var condition = top < bot && top > 0 && top < ushort.MaxValue && bot > 0 && bot < ushort.MaxValue;

        var texture = ConvertToRGBATexture(format, data, camera.pixelWidth, camera.pixelHeight);
        File.WriteAllBytes(CaptureImageEncoder.EnforceFileExtension(condition ? path : path + "_FAILED", CaptureImageEncoder.ImageFormat.Jpg), texture.EncodeToJPG());

        Log.I($"{label} : Average depth top({top}) bottom({bot})");

        Assert.True(condition);
    }

    // from https://www.compuphase.com/cmetric.htm
    double ColorDistance(Color32 e1, Color32 e2)
    {
        long rmean = ( (long)e1.r + (long)e2.r ) / 2;
        long r = (long)e1.r - (long)e2.r;
        long g = (long)e1.g - (long)e2.g;
        long b = (long)e1.b - (long)e2.b;
        return Math.Sqrt((((512+rmean)*r*r)>>8) + 4*g*g + (((767-rmean)*b*b)>>8));
    }

    bool ColorCloser(Color color, Color to, Color than)
    {
        return ColorDistance(color, to) < ColorDistance(color, than);
    }

    bool CompareColors(Color value, Color expected, float tolerance)
    {
        var result = Math.Abs(value.r - expected.r) < tolerance && Math.Abs(value.g - expected.g) < tolerance && Math.Abs(value.b - expected.b) < tolerance;
        if (!result)
            Log.I($"Color value({value.ToString()}) expected({expected.ToString()}) delta {Math.Abs(value.r - expected.r)} {Math.Abs(value.g - expected.g)} {Math.Abs(value.b - expected.b)}");
        return result;  
    }

    float CalculateAverageDepthForLine(float[] depth, int stride, int line)
    {
        var index = line * stride;
        float value = 0;
        for (var i = 0; i < stride; ++i)
            value += depth[index++];
        return value / stride;
    }

    ushort CalculateAverageDepthForLine(ushort[] depth, int stride, int line)
    {
        var index = line * stride;
        long value = 0;
        for (var i = 0; i < stride; ++i)
            value += (long)depth[index++];
        return (ushort)(value / stride);
    }

    Texture2D ConvertToRGBATexture(GraphicsFormat format, byte[] inputs, int width, int height)
    {
        var length = width * height;
        var colors = new Color[length];

        switch (format)
        {
            case GraphicsFormat.R8G8B8A8_UNorm:
                var rgba = ArrayUtilities.Cast<Color32>(inputs);
                for (var i = 0; i < length; ++i)
                    colors[i] = rgba[i];
                break;

            case GraphicsFormat.R32_SFloat:
                {
                    var depth = ArrayUtilities.Cast<float>(inputs);
                    for (var i = 0; i < length; ++i)
                    {
                        var d = depth[i];
                        colors[i] = new Color(d, d, d, 1);
                    }
                }
                break;

            case GraphicsFormat.R16_UNorm:
                {
                    var depth = ArrayUtilities.Cast<ushort>(inputs);
                    for (var i = 0; i < length; ++i)
                    {
                        var d = (float)depth[i] / (float)ushort.MaxValue;
                        colors[i] = new Color(d, d, d, 1);
                    }
                }
                break;

            default:
                throw new InvalidOperationException();
        }

        var texture = new Texture2D(width, height, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);
        texture.SetPixels(colors);
        texture.Apply();

        return texture;
    }

    bool BufferNotEmpty<T>(T[] data, int length) where T : IComparable
    {
        T def = default;
        for (var i = 0; i < length; ++i)
            if (def.CompareTo(data[i]) != 0)
                return true;
        return false;
    }
}