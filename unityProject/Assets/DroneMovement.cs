using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class DroneMovement : MonoBehaviour
{

    public bool m_ManualControl;

    private Rigidbody drone;
    private float _upForce;

    private void Awake()
    {
        drone = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        MoveDrone();
        
        drone.AddRelativeForce(Vector3.up * _upForce);
    }


    void MoveDrone()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _upForce = 450;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
