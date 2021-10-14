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

[AddRandomizerMenu("Perception/Custom Light Position Rotation Randomizer")]
public class CustomLightPositionRotationRandomizer : Randomizer
{
    public bool useMovingLight = false;
    public float changeLightPosition = 0.25f; //10.0f  0.005f; in editor 0.25
    public Vector3 initialLightPosition;

    public bool useRotatingLight = false;
    public float changeLightRotation = 10.0f; //10.0f  1.0f; in editor 10
    public Vector3 initialLightRotation;

    public FloatParameter randomFloat = new FloatParameter { value = new UniformSampler(0, 1) };

    protected override void OnIterationStart()
    {

        var taggedObjects = tagManager.Query<CustomLightPositionRotationRandomizerTag>();
        foreach (var taggedObject in taggedObjects)
        {
            var volume = taggedObject.GetComponent<Light>();

            // Deterministic

            // move the light
            if (useMovingLight)
            {
                if (randomFloat.Sample() > 0.5)
                {
                    //float x_value = initialLightPosition[0] - changeLightPosition - 2 * changeLightPosition * randomFloat.Sample();
                    //float y_value = initialLightPosition[1] - changeLightPosition - 2 * changeLightPosition * randomFloat.Sample();
                    //float z_value = initialLightPosition[2] - changeLightPosition - 2 * changeLightPosition * randomFloat.Sample();

                    Vector3 lightPosition = volume.transform.position;
                    float x_value = lightPosition[0] - changeLightPosition - 2 * changeLightPosition * randomFloat.Sample();
                    float y_value = lightPosition[1] - changeLightPosition - 2 * changeLightPosition * randomFloat.Sample();
                    float z_value = lightPosition[2] - changeLightPosition - 2 * changeLightPosition * randomFloat.Sample();
                    volume.transform.position = new Vector3(x_value, y_value, z_value);
                }
                else
                {
                    //float x_value = initialLightPosition[0] + changeLightPosition + 2 * changeLightPosition * randomFloat.Sample();
                    //float y_value = initialLightPosition[1] + changeLightPosition + 2 * changeLightPosition * randomFloat.Sample();
                    //float z_value = initialLightPosition[2] + changeLightPosition + 2 * changeLightPosition * randomFloat.Sample();

                    Vector3 lightPosition = volume.transform.position;
                    float x_value = lightPosition[0] + changeLightPosition + 2 * changeLightPosition * randomFloat.Sample();
                    float y_value = lightPosition[1] + changeLightPosition + 2 * changeLightPosition * randomFloat.Sample();
                    float z_value = lightPosition[2] + changeLightPosition + 2 * changeLightPosition * randomFloat.Sample();
                    volume.transform.position = new Vector3(x_value, y_value, z_value);
                }
            }

            // rotate the Light
            if (useRotatingLight)
            {
                if (randomFloat.Sample() > 0.5)
                {
                    //volume.transform.rotation = Quaternion.Euler(initialLightRotation[0] - changeLightRotation + 2 * changeLightRotation * randomFloat.Sample(),
                    //                                             initialLightRotation[1] - changeLightRotation + 2 * changeLightRotation * randomFloat.Sample(),
                    //                                             initialLightRotation[2] - changeLightRotation + 2 * changeLightRotation * randomFloat.Sample());

                    Vector3 lightRotation = volume.transform.rotation.eulerAngles;
                    volume.transform.rotation = Quaternion.Euler(lightRotation[0] - changeLightRotation + 2 * changeLightRotation * randomFloat.Sample(),
                                                                 lightRotation[1] - changeLightRotation + 2 * changeLightRotation * randomFloat.Sample(),
                                                                 lightRotation[2] - changeLightRotation + 2 * changeLightRotation * randomFloat.Sample());
                }
                else
                {
                    //volume.transform.rotation = Quaternion.Euler(initialLightRotation[0] + changeLightRotation - 2 * changeLightRotation * randomFloat.Sample(),
                    //                                             initialLightRotation[1] + changeLightRotation - 2 * changeLightRotation * randomFloat.Sample(),
                    //                                             initialLightRotation[2] + changeLightRotation - 2 * changeLightRotation * randomFloat.Sample());

                    Vector3 lightRotation = volume.transform.rotation.eulerAngles;
                    volume.transform.rotation = Quaternion.Euler(lightRotation[0] + changeLightRotation - 2 * changeLightRotation * randomFloat.Sample(),
                                                                 lightRotation[1] + changeLightRotation - 2 * changeLightRotation * randomFloat.Sample(),
                                                                 lightRotation[2] + changeLightRotation - 2 * changeLightRotation * randomFloat.Sample());
                }
            }
        }
    }
}