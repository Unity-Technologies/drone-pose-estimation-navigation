using System;
using System.Linq;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers.Utilities;
using UnityEngine.Perception.Randomization.Samplers;

namespace UnityEngine.Perception.Randomization.Randomizers.SampleRandomizers
{
    /// <summary>
    /// Creates a 2D layer of of evenly spaced GameObjects from a given list of prefabs
    /// </summary>
    [Serializable]
    [AddRandomizerMenu("Perception/Custom Foreground Object Placement Randomizer")]
    public class CustomForegroundObjectPlacementRandomizer : Randomizer
    {
        /// <summary>
        /// The Z offset component applied to the generated layer of GameObjects
        /// </summary>
        //public float depth;

        public float MinDepth = -5.0f;
        public float MaxDepth = 5.0f;
        public System.Random rng = new System.Random();

        /// <summary>
        /// The minimum distance between all placed objects
        /// </summary>
        public float separationDistance = 2f;

        /// <summary>
        /// The size of the 2D area designated for object placement
        /// </summary>
        public Vector2 placementArea;

        /// <summary>
        /// The list of prefabs sample and randomly place
        /// </summary>
        public GameObjectParameter prefabs;

        GameObject m_Container;
        //GameObjectOneWayCache m_GameObjectOneWayCache;
        CustomGameObjectOneWayCache m_GameObjectOneWayCache;

        public FloatParameter randomFloat = new FloatParameter { value = new UniformSampler(0, 1) };

        /// <inheritdoc/>
        protected override void OnAwake()
        {
            m_Container = new GameObject("Foreground Objects");
            m_Container.transform.parent = scenario.transform;
            m_GameObjectOneWayCache = new CustomGameObjectOneWayCache(
                m_Container.transform, prefabs.categories.Select(element => element.Item1).ToArray());
        }

        /// <summary>
        /// Generates a foreground layer of objects at the start of each scenario iteration
        /// </summary>
        protected override void OnIterationStart()
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
                //instance.transform.position = new Vector3(sample.x, sample.y, depth) + offset;
                instance.transform.position = new Vector3(sample.x, sample.y, placementDepth) + offset;
                //instance.transform.position = new Vector3(sample.x, 0f, placementDepth) + offset;

            }
            placementSamples.Dispose();
        }

        /// <summary>
        /// Deletes generated foreground objects after each scenario iteration is complete
        /// </summary>
        protected override void OnIterationEnd()
        {
            m_GameObjectOneWayCache.ResetAllObjects();
        }
    }
}
