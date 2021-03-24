using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Perception.Randomization.Randomizers;

[AddComponentMenu("Perception/RandomizerTags/DroneObjectPositionRandomizerTag")]
[RequireComponent(typeof(Renderer))]
public class DroneObjectPositionRandomizerTag : RandomizerTag
{
    public bool downObject;
    public bool upObject;
}
