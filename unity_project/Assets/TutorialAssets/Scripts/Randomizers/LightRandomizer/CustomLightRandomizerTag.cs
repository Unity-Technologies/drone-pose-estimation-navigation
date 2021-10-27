using UnityEngine;
using UnityEngine.Perception.Randomization.Randomizers;

[AddComponentMenu("Perception/RandomizerTags/CustomLightRandomizerTag")]
public class CustomLightRandomizerTag : RandomizerTag
{
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;

    public void SetIntensity(float rawIntensity)
    {
        var light = gameObject.GetComponent<Light>();
        if (light)
        {
            var scaledIntensity = rawIntensity * (maxIntensity - minIntensity) + minIntensity;
            light.intensity = scaledIntensity;
        }
    }
}