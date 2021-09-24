﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Unity.Simulation
{
    public enum LoggerSuffixOption
    {
        TIME_STAMP,
        SEQ_NO,
        BOTH
    }
    public abstract class BaseLogger
    {
        protected string _type;
        protected string _userPath;
        protected string _name;
        protected string _extension;
        protected int _sequence = -1;
        protected Func<string> _suffixAction;


        protected LoggerSuffixOption _loggerSuffixOption = LoggerSuffixOption.BOTH;

        public void SetOutputPath(string type, string name, string extension, string userPath = "", Func<string> suffixAppend = null, LoggerSuffixOption option = LoggerSuffixOption.SEQ_NO)
        {
            _type = type;
            _userPath = userPath;
            _name = name;
            _extension = extension;
            _suffixAction = suffixAppend;
            _loggerSuffixOption = option;

            var path = Manager.Instance.GetDirectoryFor(_type, _userPath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            Log.V("Output path set to : " + path);
        }

        public string GetPath()
        {
            var suffix = _suffixAction?.Invoke();
            if (String.IsNullOrEmpty(suffix))
            {
                switch (_loggerSuffixOption)
                {
                    case LoggerSuffixOption.SEQ_NO:
                    {
                        suffix = Interlocked.Increment(ref _sequence).ToString();
                        break;
                    }
                    case LoggerSuffixOption.TIME_STAMP:
                    {
                        suffix = DateTime.UtcNow.ToString("yyyy-MM-ddThh-mm-ss");
                        break;
                    }
                    case LoggerSuffixOption.BOTH:
                    {
                        suffix = DateTime.UtcNow.ToString("yyyy-MM-ddThh-mm-ss") + "_" + (Interlocked.Increment(ref _sequence));
                        break;
                    }
                }
            }
            else
            {
                suffix += "_"+ Interlocked.Increment(ref _sequence);
            }
            return Path.Combine(Manager.Instance.GetDirectoryFor(_type, _userPath), string.Format("{0}_{1}{2}", _name, suffix, _extension));
        }
    }

    public class Logger : BaseLogger
    {
        private const string kDefaultFileName  = "log";
        private const string kDefaultExtension = ".txt";
        private const int    kDefaultMaxElapsedSeconds = 5;

        Unity.Simulation.ChunkedStream _producer;
        public int bufferSize { get; set; }
        
        /// <summary>
        /// Initializes a new instance of DataCapture logger with given params.
        /// </summary>
        /// <param name="logName">Name of the DataCapture Log file.</param>
        /// <param name="bufferSize">This corresponds to the file size (in KB). Default is set to 8192.</param>
        /// <param name="userPath">Location of the log file. Default is set to Application persistent path.</param>
        public Logger(string logName, int bufferSize = 8192, int maxElapsedSeconds = kDefaultMaxElapsedSeconds, 
            string userPath = "", 
            LoggerSuffixOption suffixOption = LoggerSuffixOption.SEQ_NO, Func<string> customSuffix = null)
        {
            this.bufferSize = bufferSize;

            var name = string.IsNullOrEmpty(logName) ? kDefaultFileName  : Path.GetFileNameWithoutExtension(logName);
            var ext  = string.IsNullOrEmpty(Path.GetExtension(logName)) ? kDefaultExtension : Path.GetExtension(logName);
            SetOutputPath(DataCapturePaths.Logs, name, ext, userPath, customSuffix, suffixOption);
        
            _producer = new Unity.Simulation.ChunkedStream(bufferSize:this.bufferSize, maxElapsedSeconds: maxElapsedSeconds, functor:(AsyncRequest<object> request) =>
            {
                Unity.Simulation.FileProducer.Write(GetPath(), request.data as Array);
                return AsyncRequest.Result.Completed;
            });
        }

        /// <summary>
        /// Logs the data to the file by appending it to the current buffer.
        /// </summary>
        /// <param name="parameters">Serializable struct containing data to be captured.</param>
        public void Log(object parameters)
        {
            _producer.Append(Encoding.ASCII.GetBytes(JsonUtility.ToJson(parameters) + "\n"));
        }

        /// <summary>
        /// Flush data in the buffer to the file system right away.
        /// Usually it waits till the size of the buffer reaches to the bufferSize set.
        /// </summary>
        public void Flushall(bool synchronous = false)
        {
            _producer.Flush(synchronous);
        }
    }

    public class ScreenCapture : BaseLogger
    {
        private const string kDefaultFileName  = "screencapture";
        private const string kDefaultExtension = ".raw";

        public ScreenCapture(string capturePath = "")
        {
            var name = string.IsNullOrEmpty(capturePath) ? kDefaultFileName  : Path.GetFileNameWithoutExtension(capturePath);
            var ext  = string.IsNullOrEmpty(capturePath) ? kDefaultExtension : Path.GetExtension(capturePath);
            SetOutputPath(DataCapturePaths.ScreenCapture, name, ext, capturePath);
        }

        /// <summary>
        /// Capture Screenshot asynchronously for a given source camera
        /// </summary>
        /// <param name="sourceCamera">Source camera for which the screen capture is to be performed</param>
        /// <param name="renderTextureFormat">Render Texture format for the screen capture</param>
        /// <param name="path">Path where the image is to be saved</param>
        /// <param name="format">Image format in which the file is to be saved. Default is set to RAW</param>
        public void ScreenCaptureAsync<T>(Camera sourceCamera, GraphicsFormat renderTextureFormat, string path, CaptureImageEncoder.ImageFormat format = CaptureImageEncoder.ImageFormat.Raw) where T : struct
        {
            Debug.Assert((sourceCamera != null),"Source Camera cannot be null");
            Debug.Assert(GraphicsUtilities.SupportsRenderTextureFormat(renderTextureFormat));

            Func<AsyncRequest<CaptureCamera.CaptureState>, AsyncRequest<CaptureCamera.CaptureState>.Result> functor = (AsyncRequest<CaptureCamera.CaptureState> r) =>
            {
                r.data.colorBuffer = CaptureImageEncoder.EncodeArray(r.data.colorBuffer as Array, sourceCamera.pixelWidth, sourceCamera.pixelHeight, GraphicsFormat.R8G8B8A8_UNorm, format);
                var result = Unity.Simulation.FileProducer.Write(GetPath(), r.data.colorBuffer as Array);
                return result ? AsyncRequest<CaptureCamera.CaptureState>.Result.Completed : AsyncRequest<CaptureCamera.CaptureState>.Result.Error;
            };
            CaptureCamera.Capture(sourceCamera, functor, forceFlip: ForceFlip.None);
        }
    }
}

