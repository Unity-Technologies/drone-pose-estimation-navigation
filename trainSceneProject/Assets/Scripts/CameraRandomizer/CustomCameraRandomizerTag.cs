using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Parameters;

[AddComponentMenu("Perception/RandomizerTags/Custom Camera Randomizer Tag")]
public class CustomCameraRandomizerTag : RandomizerTag
{
    [HideInInspector]
    public Vector3 rootPosePosition;
    
    [HideInInspector]
    public Vector3 rootPoseRotation;

    private void Start()
    {
        rootPosePosition = transform.position;
        rootPoseRotation = transform.eulerAngles;
    }

}
