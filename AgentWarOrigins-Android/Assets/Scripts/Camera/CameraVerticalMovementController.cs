﻿using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;

public class CameraVerticalMovementController : MonoBehaviour
{

    public enum RotationAxis
    {
        MouseX = 1,
        MouseY = 2
    }
    public RotationAxis axes = RotationAxis.MouseY;

    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;

    public float sensHorizontal = 10.0f;
    public float sensVertical = 2;

    public float _rotationX = 0;

    public Joystick Joystick;



    // Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE

        //if (axes == RotationAxis.MouseX)
        //{
        //    transform.Rotate(0, Input.GetAxis("Mouse X") * sensHorizontal, 0);
        //}
        //else if (axes == RotationAxis.MouseY)
        //{
        //    _rotationX -= Input.GetAxis("Mouse Y") * sensVertical;
        //    _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert); //Clamps the vertical angle within the min and max limits (45 degrees)
            
        //    float rotationY = transform.localEulerAngles.y;

        //    transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
        //}

#elif UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
        
        if (axes == RotationAxis.MouseX)
        {
            transform.Rotate(0, Joystick.Horizontal * sensHorizontal, 0);
        }
        else if (axes == RotationAxis.MouseY)
        {
            _rotationX -= Joystick.Vertical * sensVertical;
            _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert); //Clamps the vertical angle within the min and max limits (45 degrees)

            float rotationY = transform.localEulerAngles.y;

            transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
        }
#endif

    }

    public void ResetCameraRotation()
    {
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    //private void OnDisable()
    //{
    //    ResetCameraRotation();
    //}


}