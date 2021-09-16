using System;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers.SampleRandomizers.Tags;
using UnityEngine.Perception.Randomization.Samplers;

namespace UnityEngine.Perception.Randomization.Randomizers.SampleRandomizers
{
    /// <summary>
    /// Randomizes the rotation of objects tagged with a RotationRandomizerTag
    /// </summary>
    [Serializable]
    [AddRandomizerMenu("Perception/Y Rotation Randomizer")]
    public class YRotationRandomizer : Randomizer
    {
        /// <summary>
        /// Defines the range of random rotations that can be assigned to tagged objects
        /// </summary>
        public FloatParameter rotationRange = new FloatParameter { value = new UniformSampler(0f, 360f)}; // in range (0, 1)

        /// <summary>
        /// Randomizes the rotation of tagged objects at the start of each scenario iteration
        /// </summary>
        protected override void OnIterationStart()
        {
            var taggedObjects = tagManager.Query<YRotationRandomizerTag>();
            foreach (var taggedObject in taggedObjects){
                float yRotation = rotationRange.Sample();
                // sets rotation
                taggedObject.SetYRotation(yRotation);
            }
        }
    }
}
