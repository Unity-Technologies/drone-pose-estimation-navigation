using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotors : MonoBehaviour
{

    public GameObject[] rotors;
    
    public float zDegreesPerSecond = 500;

    // Update is called once per frame
    void Update()
    {
        foreach (var rotor in rotors)
        {
            var center = rotor.transform.GetComponent<Renderer>().bounds.center;
            rotor.transform.RotateAround(center, Vector3.up, 500 * Time.deltaTime);
            //rotor.transform.localRotation *= Quaternion.Euler(0, zDegreesPerSecond * Time.deltaTime, 0);
        }
    }
}
