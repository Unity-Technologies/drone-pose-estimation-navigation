using System;
using UnityEngine.Perception.Randomization.Scenarios;
[Serializable]
public class PoseEstimationScenarioConstants : ScenarioConstants
{
    public int totalFrames = 1000;
}
public class PoseEstimationScenario : PerceptionScenario<PoseEstimationScenarioConstants>
{
    public bool trainingMode = false;
    private bool shouldIterate;
    public void Move()
    {
        shouldIterate = true;
    }
    protected override void IncrementIteration()
    {
        base.IncrementIteration();
        shouldIterate = false;
    }

    /*
    protected override bool isScenarioReadyToStart => true;
    protected override bool isIterationComplete => trainingMode || shouldIterate;
    protected override bool isScenarioComplete => currentIteration >= constants.totalFrames;
    */
    protected override bool isIterationComplete => shouldIterate || trainingMode && currentIterationFrame >= 1;
    
    protected override bool isScenarioComplete => currentIteration >= constants.totalFrames;

    protected override void OnComplete()
    {
        if (trainingMode)
            base.OnComplete();
    }
}