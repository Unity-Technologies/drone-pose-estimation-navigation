using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class DroneMovement : MonoBehaviour
{

    public bool m_ManualControl;

    private Rigidbody _drone;
    private float _upForce;
    private float tiltVelocity;
    private float tiltAmount = 0;
    public float m_DroneMovementSpeed = 500f;
    
    private float _wantedYRotation;
    private float _currentYRotation;
    private float _rotateAmountByKeys = 2.5f;
    private float _rotationYVelocity;

    private void Awake()
    {
        _drone = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        
        //GetNextPose() -> python process
        
        //Apply the pose (Quaternion
        
        MoveDroneUpAndDown();
        MoveDroneForward();
        DroneRotate();
        
        _drone.AddRelativeForce(Vector3.up * _upForce);
        _drone.rotation = Quaternion.Euler(new Vector3(tiltAmount, _currentYRotation, _drone.rotation.z));
    }


    void MoveDroneUpAndDown()
    {
        if (m_ManualControl)
        {
            if (Input.GetKey(KeyCode.E))
            {
                _upForce = 450;
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                _upForce = -200;
            }
            else if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                _upForce = 98.1f;
            }
        }
    }

    void MoveDroneForward()
    {
        if (m_ManualControl)
        {
            if (Input.GetAxis("Vertical") != 0)
            {
                _drone.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * m_DroneMovementSpeed);
                tiltAmount = Mathf.SmoothDamp(tiltAmount, 20 * Input.GetAxis("Vertical"), ref tiltVelocity, 0.1f);
            }
        }
    }

    void DroneRotate()
    {
        if (m_ManualControl)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _wantedYRotation -= _rotateAmountByKeys;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                _wantedYRotation += _rotateAmountByKeys;
            }

            _currentYRotation = Mathf.SmoothDamp(_currentYRotation, _wantedYRotation, ref _rotationYVelocity, 0.25f);   
        }
    }
}
