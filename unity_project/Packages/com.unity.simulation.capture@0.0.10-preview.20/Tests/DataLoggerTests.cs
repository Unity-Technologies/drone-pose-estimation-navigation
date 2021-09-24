using System;
using System.Collections;
using System.Globalization;
using System.IO;
using UnityEngine;

using Unity.Simulation;

using NUnit.Framework;
using UnityEngine.TestTools;

using Logger = Unity.Simulation.Logger;

public class DataLoggerTests
{
    struct TestLog
    {
        public string msg;
    }

    [UnitySetUp]
    public IEnumerator CleanupExistingFiles()
    {
        var path = Path.Combine(Configuration.Instance.GetStoragePath(), "Logs");
        if (Directory.Exists(path))
        {
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        yield return null;
    }

    [UnityTest]
    [Timeout(10000)]
    public IEnumerator ProducerBuffer_TrimsEmptySpaces_IfPresentBeforeFlush()
    {
        string path = Path.Combine(Configuration.Instance.GetStoragePath(), "Logs", "log_0.txt");
        var inputLog = new TestLog() {msg = "Test"};
        var logger = new Logger("log.txt", 20);
        logger.Log(new TestLog() { msg = "Test"});
        logger.Log(new TestLog() { msg = "UnityTest"});
        while (!System.IO.File.Exists(path))
            yield return null;
        var fileInfo = new FileInfo(path);
        Assert.AreEqual(JsonUtility.ToJson(inputLog).Length + 1, fileInfo.Length);
    }

    [UnityTest]
    public IEnumerator DataLogger_FlushesToFileSystem_WithElapsedTimeSet()
    {
        var logger = new Logger("TestLog.txt", maxElapsedSeconds: 2);
        logger.Log(new TestLog() { msg = "Test 123"});
        yield return new WaitForSeconds(3);
        var path = Path.Combine(Configuration.Instance.GetStoragePath(), "Logs", "TestLog_0.txt");
        Assert.IsTrue(File.Exists(path));
    }

    [UnityTest]
    public IEnumerator DataLogger_FlushesToFileSystemOnlyWithMaxBufferSize()
    {
        var logger = new Logger("SimLog", bufferSize: 65536, maxElapsedSeconds:-1);
        logger.Log(new TestLog() { msg = "Test Simulation Log!"});
        yield return new WaitForSeconds(5);
        var path = Path.Combine(Configuration.Instance.GetStoragePath(), "Logs", "SimLog_0.txt");
        Assert.IsTrue(!File.Exists(path));
        for (int i = 0; i < 100; i++)
        {
            logger.Log(new TestLog(){ msg = "Test Simulation Log"});
        }
        logger.Flushall(true);
        Assert.IsTrue(File.Exists(path));

        var fileContents = File.ReadAllLines(path);
        Assert.AreEqual(fileContents.Length, 101);
    }

    [UnityTest]
    public IEnumerator DataLogger_TimestampSuffix()
    {
        var logger = new Logger("testLog", maxElapsedSeconds:-1, suffixOption: LoggerSuffixOption.TIME_STAMP);
        var path = Path.Combine(Configuration.Instance.GetStoragePath(), "Logs");
        logger.Log(new TestLog() { msg = "This is a test log with timestamp" });
        logger.Flushall(true);
        var files = Directory.GetFiles(path);
        Assert.IsTrue(files.Length > 0, "No Logs created");
        foreach (var file in files)
        {
            var fn = Path.GetFileName(file).Split('_')[1];
            var timestamp = fn.Split('.')[0];
            Assert.IsTrue(timestamp.Length > 0, "Invalid file name: " + file);
            DateTime dt;
            Assert.IsTrue(DateTime.TryParseExact(timestamp, "yyyy-MM-ddThh-mm-ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dt), "Not a valid Data: " + timestamp);
        }
        yield return null;

    }

    [UnityTest]
    public IEnumerator DataLogger_TimestampAndSeqSuffix()
    {
        var logger = new Logger("testLog", bufferSize: 10, maxElapsedSeconds:-1, suffixOption: LoggerSuffixOption.BOTH);
        var path = Path.Combine(Configuration.Instance.GetStoragePath(), "Logs");
        logger.Log(new TestLog() { msg = "This is a test log with timestamp and seq" });
        logger.Log(new TestLog() { msg = "This is a another test log" });
        logger.Flushall(true);
        var files = Directory.GetFiles(path);
        Assert.IsTrue(files.Length > 0, "No Logs created");
        int start = 0;
        foreach (var file in files)
        {
            var fn = Path.GetFileName(file).Split('_');
            Assert.IsTrue(fn.Length == 3 , "Invalid file name: " + file);
            var timestamp = fn[1];
            var seq = fn[2].Split('.')[0];
            DateTime dt;
            Assert.IsTrue(DateTime.TryParseExact(timestamp, "yyyy-MM-ddThh-mm-ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dt), "Not a valid Data: " + timestamp);
            Assert.IsTrue(start++ == Int32.Parse(seq));
        }
        yield return null;
    }

    [UnityTest]
    public IEnumerator DataLogger_CustomFilenameSuffix()
    {
        var logger = new Logger("testLog", bufferSize: 10, maxElapsedSeconds:-1, customSuffix: () => DateTime.Now.ToString("yy-MM-dd"));
        var path = Path.Combine(Configuration.Instance.GetStoragePath(), "Logs");
        logger.Log(new TestLog() { msg = "This is a test log with timestamp and seq" });
        logger.Log(new TestLog() { msg = "This is a another test log" });
        logger.Flushall(true);
        var files = Directory.GetFiles(path);
        Assert.IsTrue(files.Length > 0, "No Logs created");
        int start = 0;
        foreach (var file in files)
        {
            var fn = Path.GetFileName(file).Split('_');
            Assert.IsTrue(fn.Length == 3 , "Invalid file name: " + file);
            var timestamp = fn[1];
            var seq = fn[2].Split('.')[0];
            DateTime dt;
            Assert.IsTrue(DateTime.TryParseExact(timestamp, "yy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dt), "Not a valid Data: " + timestamp);
            Assert.IsTrue(start++ == Int32.Parse(seq));
        }
        yield return null;
    }
    
}