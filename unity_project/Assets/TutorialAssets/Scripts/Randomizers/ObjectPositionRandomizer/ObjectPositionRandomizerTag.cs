using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Perception.Randomization.Randomizers;

[AddComponentMenu("Perception/RandomizerTags/ObjectPositionRandomizerTag")]
[RequireComponent(typeof(Renderer))]
public class ObjectPositionRandomizerTag : RandomizerTag
{
    public bool downObject;
    public bool upObject = true;
}
