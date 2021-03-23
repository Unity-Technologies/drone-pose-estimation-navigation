using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Perception.Randomization.Scenarios;

public class CustomScenario : FixedLengthScenario
{
    protected override bool isIterationComplete => ProceedToNextScenario();
    
    private bool ProceedToNextScenario()
    {
        return false;
    }
}
