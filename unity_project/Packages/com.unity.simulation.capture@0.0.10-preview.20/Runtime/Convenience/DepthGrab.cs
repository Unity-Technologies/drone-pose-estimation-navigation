using System.IO;

using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Unity.Simulation
{   
    public class DepthGrab : MonoBehaviour
    {
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

        void Start()
        {
            _baseDirectory = Manager.Instance.GetDirectoryFor(DataCapturePaths.ScreenCapture);
            if (_camera != null && _camera.depthTextureMode == DepthTextureMode.None)
                _camera.depthTextureMode = DepthTextureMode.Depth;
        }

        void Update()
        {   
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > _screenCaptureInterval)
            {
                _elapsedTime -= _screenCaptureInterval;

                if (Application.isBatchMode && _camera.targetTexture == null)
                {
                    _camera.targetTexture = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight,0, _format);
                }

                string path = "";
#if UNITY_2019_3_OR_NEWER
                if (_nameGenerator != null)
                    path = _nameGenerator.Generate(Path.Combine(_baseDirectory, $"{_camera.name}.{_imageFormat.ToString().ToLower()}"));
                else
#endif
                    path = Path.Combine(_baseDirectory, _camera.name + "_depth_" + _sequence + "." + _imageFormat.ToString().ToLower());

                CaptureCamera.CaptureDepthToFile
                (
                    _camera, 
                    _format, 
                    path,
                    _imageFormat
                );

                if (!_camera.enabled)
                    _camera.Render();

                ++_sequence;
            }
        }

        void OnValidate()
        {
            // Automatically add the camera component if there is one on this game object.
            if (_camera == null)
                _camera = GetComponent<Camera>();
        }
    }
}