using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using FloatParameter = UnityEngine.Perception.Randomization.Parameters.FloatParameter;
using UnityEngine.Perception.Randomization.Samplers;

[Serializable]

[AddRandomizerMenu("Perception/Camera Randomizer")]
public class CameraRandomizer : Randomizer
{

    public float positionRange = 0.005f;
    public float rotationRangeDegrees = 1.0f; 

    public FloatParameter random; //(-1, 1)

    protected override void OnIterationStart()
    {

        var taggedObjects = tagManager.Query<CameraRandomizerTag>();
        foreach (var taggedObject in taggedObjects)
        {
            CameraRandomizerTag tag = taggedObject.GetComponent<CameraRandomizerTag>();
            Vector3 adjustedPosition = AdjustedVector(tag.rootPosePosition, positionRange);
            Vector3 adjustedRotation = AdjustedVector(tag.rootPoseRotation, rotationRangeDegrees);

            taggedObject.transform.position = adjustedPosition;
            taggedObject.transform.eulerAngles = adjustedRotation;
            
        }
    }
    
    private Vector3 AdjustedVector(Vector3 rootVector, float range)
    {
        float x = AdjustedValue(rootVector.x, range);
        float y = AdjustedValue(rootVector.y, range);
        Vector3 adjustedVector = new Vector3(x, y, rootVector.z);
        return adjustedVector;
    }

    private float AdjustedValue(float rootValue, float range)
    {
        // adjust the rootvalue to be in the following segment: [rootvalue - range; rootvalue + range]
        float change = -range + 2 * range * random.Sample();
        float adjustedVal = rootValue + change;
        return adjustedVal;
    }
}