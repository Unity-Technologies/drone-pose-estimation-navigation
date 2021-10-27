using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    // Start is called before the first frame update
    public PoseEstimationScenario scenario;

    private Canvas canvas;
    void Start()
    {
        canvas = GetComponent<Canvas>();
        if (scenario.trainingMode)
            canvas.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        canvas.enabled = !(scenario.trainingMode);
    }
}
