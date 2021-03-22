using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomModifyPostProcessVolume : MonoBehaviour
{
    private System.Random rng = new System.Random();

    public float minVignetteIntensity = 0.0f;
    public float maxVignetteIntensity = 0.5f;

    public float minWhiteBalanceTemperature = -100.0f;
    public float maxWhiteBalanceTemperature = 100.0f;

    public float minFilmGrainIntensity = 0.0f;
    public float maxFilmGrainIntensity = 1.0f;

    public float minLensDistortionIntensity = -0.2f;
    public float maxLensDistortionIntensity = 0.2f;

    public float minFocusDistance = 1.5f;
    public float maxFocusDistance = 3.0f;

    //public float minFocalLength = 1.0f;
    //public float maxFocalLength = 70.0f;

    //public float minAperture = 1.0f;
    //public float maxAperture = 16.0f;

    public float minPostExposure = -1.5f;
    public float maxPostExposure = 1.5f;

    public float minContrast = -25.0f;
    public float maxContrast = 25.0f;

    public float minSaturation = -25.0f;
    public float maxSaturation = 25.0f;


    protected float GenerateRandom(double maxVal, double minVal)
    {
        double range = (double)maxVal - (double)minVal;
        double smpl = rng.NextDouble();
        double scaled = (smpl * range) + (float)minVal;
        float randomVal = (float)scaled;

        return randomVal;
    }

    void Start()
    {
        Volume volumeProfile = GetComponent<Volume>();

        Vignette vignette;
        if (volumeProfile.profile.TryGet(out vignette))
        {
            float vignetteVal = 0.25f;
            vignette.intensity.value = vignetteVal;
        }

        WhiteBalance whiteBalance;
        if (volumeProfile.profile.TryGet(out whiteBalance))
        {
            float whiteBalanceTemperatureVal = 0.0f;
            whiteBalance.temperature.value = whiteBalanceTemperatureVal;
        }

        FilmGrain filmGrain;
        if (volumeProfile.profile.TryGet(out filmGrain))
        {
            float filmGrainVal = 0.0f;
            filmGrain.intensity.value = filmGrainVal;

        }

        LensDistortion lensDistortion;
        if (volumeProfile.profile.TryGet(out lensDistortion))
        {
            float lensDistortionVal = 0.0f;
            lensDistortion.intensity.value = lensDistortionVal;
        }

        DepthOfField depthOfField;
        if (volumeProfile.profile.TryGet(out depthOfField))
        {
            float focusDistance = 10.0f;
            depthOfField.focusDistance.value = focusDistance;

            //float focalLength = 50.0f;
            //depthOfField.focalLength.value = focalLength;

            //float aperture = 5.6f;
            //depthOfField.aperture.value = aperture;

        }

        ColorAdjustments colorAdjustments;
        if (volumeProfile.profile.TryGet(out colorAdjustments))
        {
            float postExposureVal = 0.0f;
            colorAdjustments.postExposure.value = postExposureVal;

            float contrastVal = 0.0f;
            colorAdjustments.contrast.value = contrastVal;

            float saturationVal = 0.0f;
            colorAdjustments.saturation.value = saturationVal;

        }

    }

    void Update()
    {
        // Remember to enable the tick for any component you want to vary
        Volume volumeProfile = GetComponent<Volume>();

        // Modify Vignette;
        ModVignette(volumeProfile);

        // Modify White Balance;
        ModWhiteBalance(volumeProfile);

        // Modify Film Grain
        ModFilmGrain(volumeProfile);

        // Modify Lens Distortion
        ModLensDistortion(volumeProfile);

        // Modify Depth of Field - remember to set Focus Mode to (Use Physical Camera) and Quality to (Custom) in inspector
        ModDepthOfField(volumeProfile);

        // Modify Color Adjustment
        ModColorAdjustments(volumeProfile);

    }

    public void ModVignette(Volume volumeProfile)
    {
        Vignette vignette;
        if (volumeProfile.profile.TryGet(out vignette))
        {
            float vignetteVal = GenerateRandom((double)maxVignetteIntensity, (double)minVignetteIntensity);
            vignette.intensity.value = vignetteVal;
        }
    }

    public void ModWhiteBalance(Volume volumeProfile)
    {
        WhiteBalance whiteBalance;
        if (volumeProfile.profile.TryGet(out whiteBalance))
        {
            float whiteBalanceTemperatureVal = GenerateRandom((double)maxWhiteBalanceTemperature, (double)minWhiteBalanceTemperature);
            whiteBalance.temperature.value = whiteBalanceTemperatureVal;
        }
    }

    public void ModFilmGrain(Volume volumeProfile)
    {
        FilmGrain filmGrain;
        if (volumeProfile.profile.TryGet(out filmGrain))
        {
            float filmGrainVal = GenerateRandom((double)maxFilmGrainIntensity, (double)minFilmGrainIntensity);
            filmGrain.intensity.value = filmGrainVal;

        }
    }

    public void ModLensDistortion(Volume volumeProfile)
    {
        LensDistortion lensDistortion;
        if (volumeProfile.profile.TryGet(out lensDistortion))
        {
            float lensDistortionVal = GenerateRandom((double)maxLensDistortionIntensity, (double)minLensDistortionIntensity);
            lensDistortion.intensity.value = lensDistortionVal;
        }
    }

    public void ModDepthOfField(Volume volumeProfile)
    {
        DepthOfField depthOfField;
        if (volumeProfile.profile.TryGet(out depthOfField))
        {
            float focusDistanceVal = GenerateRandom((double)maxFocusDistance, (double)minFocusDistance);
            depthOfField.focusDistance.value = focusDistanceVal;

            //float focalLengthVal = GenerateRandom((double)maxFocalLength, (double)minFocalLength);
            //depthOfField.focalLength.value = focalLengthVal;

            //float apertureVal = GenerateRandom((double)maxAperture, (double)minAperture);
            //depthOfField.aperture.value = apertureVal;

        }
    }

    public void ModColorAdjustments(Volume volumeProfile)
    {
        ColorAdjustments colorAdjustments;
        if (volumeProfile.profile.TryGet(out colorAdjustments))
        {
            float postExposureVal = GenerateRandom((double)maxPostExposure, (double)minPostExposure);
            colorAdjustments.postExposure.value = postExposureVal;

            float contrastVal = GenerateRandom((double)maxContrast, (double)minContrast);
            colorAdjustments.contrast.value = contrastVal;

            float saturationVal = GenerateRandom((double)maxSaturation, (double)minSaturation);
            colorAdjustments.saturation.value = saturationVal;

        }
    }
}
