using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Randomizers.SampleRandomizers.Tags;

namespace UnityEngine.Perception.Randomization.Parameters
{
    [Serializable]
    public class GameObjectParameter : CategoricalParameter<GameObject> { }
}

[Serializable]
[AddRandomizerMenu("Perception/Distractor Object Randomizer")]
public class DistractorObjectRandomizer : Randomizer
{
    private GameObject distractorObjectsParent;
    public IntegerParameter randomNumObjects;
    public GameObjectParameter randomPrefab;

    private Dictionary<string, List<GameObject>> cache = new Dictionary<string, List<GameObject>>();

    protected override void OnAwake()
    {
        distractorObjectsParent = new GameObject("DistractorObjects");
    }

    protected override void OnIterationStart()
    {
        DeactivateAllObjects();

        int numObjects = randomNumObjects.Sample();
        for (int i = 0; i < numObjects; i++)
        {
            GameObject prefab = randomPrefab.Sample(); // randomly picks a cylinder or a sphere
            GameObject distractorObject = GetDistractorObject(prefab);
            distractorObject.SetActive(true);
        }
    }


    // HELPERS

    private GameObject GetDistractorObject(GameObject prefab)
    {
        /* Gets deactivated object of specified type from cache if one is available.
         * If not, lazily creates one and adds it to the cache.
         */

        // look for cached object
        List<GameObject> objects = cache.ContainsKey(prefab.name) ? cache[prefab.name] : new List<GameObject>();
        foreach (GameObject obj in objects)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        // nothing found in cache, generate new object and cache it
        GameObject newObj = GenerateObject(prefab);
        objects.Add(newObj);
        cache[prefab.name] = objects;
        return newObj;
    }


    private void DeactivateAllObjects()
    {
        foreach (List<GameObject> objectList in cache.Values)
        {
            foreach (GameObject obj in objectList)
            {
                obj.SetActive(false);
            }
        }
    }
   
    private GameObject GenerateObject(GameObject prefab)
    {
        GameObject obj = UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        obj.transform.parent = distractorObjectsParent.transform;
        return obj;
    }
    
}
