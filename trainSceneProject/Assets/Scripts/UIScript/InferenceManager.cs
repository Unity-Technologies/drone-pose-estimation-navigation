using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Perception.Randomization.Scenarios;

public class InferenceManager : MonoBehaviour
{

    public PoseEstimationScenario scenario;

    public void StartPoseEstimation()
    {
        // Call to python process..
    }

    public void GenerateNewEnvironment()
    {
        scenario.Move();
    }
}
