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

[AddRandomizerMenu("Perception/Custom Camera Randomizer")]
public class CustomCameraRandomizer : Randomizer
{
    // private System.Random rng = new System.Random();

    public bool useVariableFieldOfView = false;
    public float cameraMinFieldOfView = 10.0f; //5
    public float cameraMaxFieldOfView = 90.0f; //40

    public bool usePhysicalCamera = false;
    public float cameraMinFocalLength = 1.0f; // 1
    public float cameraMaxFocalLength = 100.0f; // 100

    public bool useMovingCamera = false;
    public float changeCameraPosition = 0.25f; //10.0f  0.005f; in editor 0.25
    public Vector3 initialCameraPosition;

    public bool useRotatingCamera = false;
    public float changeCameraRotation = 10.0f; //10.0f  1.0f; in editor 10
    public Vector3 initialCameraRotation;

    public FloatParameter randomFloat = new FloatParameter { value = new UniformSampler(0, 1) };

    protected float GenerateRandom(double maxVal, double minVal)
    {
        double range = (double)maxVal - (double)minVal;
        //double smpl = rng.NextDouble();
        double smpl = randomFloat.Sample();
        double scaled = (smpl * range) + (float)minVal;
        float randomVal = (float)scaled;

        return randomVal;
    }

    protected override void OnIterationStart()
    {

        var taggedObjects = tagManager.Query<CustomCameraRandomizerTag>();
        foreach (var taggedObject in taggedObjects)
        {
            var volume = taggedObject.GetComponent<Camera>();

            // change Field of View
            if (useVariableFieldOfView)
            {                
                float newFOV = GenerateRandom((double)cameraMaxFieldOfView, (double)cameraMinFieldOfView);

                volume.fieldOfView = newFOV;
            }

            // change Focal Length
            if (usePhysicalCamera)
            {
                float newFL = GenerateRandom((double)cameraMaxFocalLength, (double)cameraMinFocalLength);

                volume.focalLength = newFL;
            }

            
            // Deterministic

            // transform the camera
            if (useMovingCamera)
            {
                if (randomFloat.Sample() > 0.5)
                {
                    //float x_value = initialCameraPosition[0] - changeCameraPosition - 2 * changeCameraPosition * randomFloat.Sample();
                    //float y_value = initialCameraPosition[1] - changeCameraPosition - 2 * changeCameraPosition * randomFloat.Sample();
                    //float z_value = initialCameraPosition[2] - changeCameraPosition - 2 * changeCameraPosition * randomFloat.Sample();

                    Vector3 cameraPosition = volume.transform.position;
                    float x_value = cameraPosition[0] - changeCameraPosition - 2 * changeCameraPosition * randomFloat.Sample();
                    float y_value = cameraPosition[1] - changeCameraPosition - 2 * changeCameraPosition * randomFloat.Sample();
                    float z_value = cameraPosition[2] - changeCameraPosition - 2 * changeCameraPosition * randomFloat.Sample();
                    volume.transform.position = new Vector3(x_value, y_value, z_value);
                }
                else
                {
                    //float x_value = initialCameraPosition[0] + changeCameraPosition + 2 * changeCameraPosition * randomFloat.Sample();
                    //float y_value = initialCameraPosition[1] + changeCameraPosition + 2 * changeCameraPosition * randomFloat.Sample();
                    //float z_value = initialCameraPosition[2] + changeCameraPosition + 2 * changeCameraPosition * randomFloat.Sample();

                    Vector3 cameraPosition = volume.transform.position;
                    float x_value = cameraPosition[0] + changeCameraPosition + 2 * changeCameraPosition * randomFloat.Sample();
                    float y_value = cameraPosition[1] + changeCameraPosition + 2 * changeCameraPosition * randomFloat.Sample();
                    float z_value = cameraPosition[2] + changeCameraPosition + 2 * changeCameraPosition * randomFloat.Sample();
                    volume.transform.position = new Vector3(x_value, y_value, z_value);
                }
            }

            // rotate the camera
            if (useRotatingCamera)
            {
                if (randomFloat.Sample() > 0.5)
                {
                    //volume.transform.rotation = Quaternion.Euler(initialCameraRotation[0] - changeCameraRotation + 2 * changeCameraRotation * randomFloat.Sample(),
                    //                                             initialCameraRotation[1] - changeCameraRotation + 2 * changeCameraRotation * randomFloat.Sample(),
                    //                                             initialCameraRotation[2] - changeCameraRotation + 2 * changeCameraRotation * randomFloat.Sample());

                    Vector3 cameraRotation = volume.transform.rotation.eulerAngles;
                    volume.transform.rotation = Quaternion.Euler(cameraRotation[0] - changeCameraRotation + 2 * changeCameraRotation * randomFloat.Sample(),
                                                                 cameraRotation[1] - changeCameraRotation + 2 * changeCameraRotation * randomFloat.Sample(),
                                                                 cameraRotation[2] - changeCameraRotation + 2 * changeCameraRotation * randomFloat.Sample());
                }
                else
                {
                    //volume.transform.rotation = Quaternion.Euler(initialCameraRotation[0] + changeCameraRotation - 2 * changeCameraRotation * randomFloat.Sample(),
                    //                                             initialCameraRotation[1] + changeCameraRotation - 2 * changeCameraRotation * randomFloat.Sample(),
                    //                                             initialCameraRotation[2] + changeCameraRotation - 2 * changeCameraRotation * randomFloat.Sample());

                    Vector3 cameraRotation = volume.transform.rotation.eulerAngles;
                    volume.transform.rotation = Quaternion.Euler(initialCameraRotation[0] + changeCameraRotation - 2 * changeCameraRotation * randomFloat.Sample(),
                                                                 initialCameraRotation[1] + changeCameraRotation - 2 * changeCameraRotation * randomFloat.Sample(),
                                                                 initialCameraRotation[2] + changeCameraRotation - 2 * changeCameraRotation * randomFloat.Sample());
                }
            }
        }
    }
}