using System.IO;

using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Unity.Simulation
{   
    public class ChannelGrab : MonoBehaviour
    {
        public CaptureCamera.Channel _channel;
#if UNITY_2019_3_OR_NEWER
        public NameGenerator       _nameGenerator;
#endif
        public CaptureImageEncoder.ImageFormat _imageFormat = CaptureImageEncoder.ImageFormat.Jpg;
        public float               _screenCaptureInterval = 1.0f;
        public GraphicsFormat      _format = GraphicsFormat.R8G8B8A8_UNorm;

        float                      _elapsedTime;
        string                     _baseDirectory;
        int                        _sequence = 0;
        public Camera              _camera;
        int                        _width;
        int                        _height;

        void Start()
        {
            _baseDirectory = Manager.Instance.GetDirectoryFor(DataCapturePaths.ScreenCapture);
            if (_camera != null)
            {
                if (_channel == CaptureCamera.Channel.Depth)
                    _camera.depthTextureMode |= DepthTextureMode.Depth;
                if (_channel == CaptureCamera.Channel.Normal)
                    _camera.depthTextureMode |= DepthTextureMode.Depth | DepthTextureMode.DepthNormals;
                if (_channel == CaptureCamera.Channel.Motion)
                    _camera.depthTextureMode |= DepthTextureMode.MotionVectors;
                _width = _camera.pixelWidth;
                _height = _camera.pixelHeight;
            }
        }

        void Update()
        {   
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > _screenCaptureInterval)
            {
                _elapsedTime -= _screenCaptureInterval;

                string path = "";
#if UNITY_2019_3_OR_NEWER
                if (_nameGenerator != null)
                    path = _nameGenerator.Generate(Path.Combine(_baseDirectory, $"{_camera.name}.{_imageFormat.ToString().ToLower()}"));
                else
#endif
                    path = Path.Combine(_baseDirectory, _camera.name + _channel.ToString().ToLower() + _sequence + "." + _imageFormat.ToString().ToLower());

                switch (_channel)
                {
                    case CaptureCamera.Channel.Color:
                        CaptureCamera.Capture(_camera, colorFunctor: request => { WriteChannelToPath(path, (byte[])request.data.colorBuffer); return AsyncRequest.Result.Completed; }, colorFormat: _format, forceFlip: ForceFlip.None);
                        break;
                    case CaptureCamera.Channel.Depth:
                        CaptureCamera.Capture(_camera, depthFunctor: request => { WriteChannelToPath(path, (byte[])request.data.depthBuffer); return AsyncRequest.Result.Completed; }, depthFormat: _format, forceFlip: ForceFlip.None);
                        break;
                    case CaptureCamera.Channel.Normal:
                        CaptureCamera.Capture(_camera, normalFunctor: request => { WriteChannelToPath(path, (byte[])request.data.normalBuffer); return AsyncRequest.Result.Completed; }, normalFormat: _format, forceFlip: ForceFlip.None);
                        break;
                    case CaptureCamera.Channel.Motion:
                        CaptureCamera.Capture(_camera, motionFunctor: request => { WriteChannelToPath(path, (byte[])request.data.motionBuffer); return AsyncRequest.Result.Completed; }, motionFormat: _format, forceFlip: ForceFlip.None);
                        break;
                }

                if (!_camera.enabled)
                    _camera.Render();

                ++_sequence;
            }
        }

        void WriteChannelToPath(string path, byte[] buffer)
        {
            File.WriteAllBytes(path, (byte[])CaptureImageEncoder.EncodeArray(buffer, _width, _height, _format, _imageFormat));
        }

        void OnValidate()
        {
            // Automatically add the camera component if there is one on this game object.
            if (_camera == null)
                _camera = GetComponent<Camera>();
        }
    }
}