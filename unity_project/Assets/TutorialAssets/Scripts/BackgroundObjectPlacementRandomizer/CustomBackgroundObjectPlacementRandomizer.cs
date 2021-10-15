using System;
using System.Linq;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers.Utilities;
using UnityEngine.Perception.Randomization.Samplers;

namespace UnityEngine.Perception.Randomization.Randomizers.SampleRandomizers
{
    /// <summary>
    /// Creates multiple layers of evenly distributed but randomly placed objects
    /// </summary>
    [Serializable]
    [AddRandomizerMenu("Perception/Custom Background Object Placement Randomizer")]
    public class CustomBackgroundObjectPlacementRandomizer : Randomizer
    {
        /// <summary>
        /// The Z offset component applied to all generated background layers
        /// </summary>
        //public float depth;

        public float MinDepth = -5.0f;
        public float MaxDepth = 5.0f;
        public System.Random rng = new System.Random();

        /// <summary>
        /// The number of background layers to generate
        /// </summary>
        public int layerCount = 4;

        /// <summary>
        /// The minimum distance between placed background objects
        /// </summary>
        public float separationDistance = 2f;

        /// <summary>
        /// The 2D size of the generated background layers
        /// </summary>
        public Vector2 placementArea;

        /// <summary>
        /// A categorical parameter for sampling random prefabs to place
        /// </summary>
        public GameObjectParameter prefabs;

        GameObject m_Container;
        CustomGameObjectOneWayCache m_GameObjectOneWayCache;

        public FloatParameter randomFloat = new FloatParameter { value = new UniformSampler(0, 1) };

        /// <inheritdoc/>
        protected override void OnAwake()
        {
            m_Container = new GameObject("BackgroundContainer");
            m_Container.transform.parent = scenario.transform;
            m_GameObjectOneWayCache = new CustomGameObjectOneWayCache(
                m_Container.transform, prefabs.categories.Select((element) => element.Item1).ToArray());
        }

        /// <summary>
        /// Generates background layers of objects at the start of each scenario iteration
        /// </summary>
        protected override void OnIterationStart()
        {
            for (var i = 0; i < layerCount; i++)
            {
                var seed = SamplerState.NextRandomState();
                var placementSamples = CustomPoissonDiskSampling.GenerateSamples(
                    placementArea.x, placementArea.y, separationDistance, seed);
                var offset = new Vector3(placementArea.x, placementArea.y, 0f) * -0.5f;
                //var offset = new Vector3(placementArea.x, 0f, 0f) * -0.5f;
                foreach (var sample in placementSamples)
                {
                    double range = (double)MaxDepth - (double)MinDepth;
                    double smpl = randomFloat.Sample();
                    double scaled = (smpl * range) + (float)MinDepth;
                    float placementDepth = (float)scaled;

                    var instance = m_GameObjectOneWayCache.GetOrInstantiate(prefabs.Sample());
                    //instance.transform.position = new Vector3(sample.x, sample.y, separationDistance * i + depth) + offset;
                    instance.transform.position = new Vector3(sample.x, sample.y, placementDepth) + offset;
                }
                placementSamples.Dispose();
            }
        }

        /// <summary>
        /// Deletes generated background objects after each scenario iteration is complete
        /// </summary>
        protected override void OnIterationEnd()
        {
            m_GameObjectOneWayCache.ResetAllObjects();
        }
    }
}
