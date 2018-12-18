using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public GameObject leftWheelTyre;
    public GameObject rightWheelTyre;
    public bool motor;
    public bool steering;
}
[Serializable]
public class SimpleCarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public float brakeTorque = 30000f;
    public VehiclePathFollower VehiclePathFollower;
    public bool IsLastWayPointReached;
    private VehicleHealth vehicleHealth;
    // finds the corresponding visual wheel
    // correctly applies the transform

    void Start()
    {
        vehicleHealth = GetComponent<VehicleHealth>();
        VehiclePathFollower = GetComponent<VehiclePathFollower>();
    }
    public void ApplyLocalPositionToVisuals(AxleInfo axleInfo)
    {

        Vector3 leftwheelPos;
        Quaternion leftwheelRotation;
        Vector3 rightwheelPos;
        Quaternion rightwheelRotation;
        axleInfo.leftWheel.GetWorldPose(out leftwheelPos, out leftwheelRotation);
        axleInfo.rightWheel.GetWorldPose(out rightwheelPos, out rightwheelRotation);

        axleInfo.leftWheelTyre.transform.position = leftwheelPos;
        axleInfo.leftWheelTyre.transform.rotation = leftwheelRotation;
        axleInfo.rightWheelTyre.transform.position = rightwheelPos;
        axleInfo.rightWheelTyre.transform.rotation = rightwheelRotation;

    }

    public void FixedUpdate()
    {
        float motor = VehiclePathFollower.Speed * maxMotorTorque;
        maxSteeringAngle = VehiclePathFollower.DifferenceInRotation().x;
        if (vehicleHealth)
        {
            foreach (AxleInfo axleInfo in axleInfos)
            {
                if ((IsLastWayPointReached= VehiclePathFollower.IsLastWayPointReached()) || vehicleHealth.GetCurrentHealth()==0f)
                {
                    axleInfo.leftWheel.brakeTorque = brakeTorque;
                    axleInfo.rightWheel.brakeTorque = brakeTorque;
                }
                else
                {
                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.brakeTorque = 0;
                }
                if (axleInfo.steering)
                {
                    axleInfo.leftWheel.steerAngle = maxSteeringAngle;
                    axleInfo.rightWheel.steerAngle = maxSteeringAngle;
                }
                if (axleInfo.motor)
                {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;
                }
                ApplyLocalPositionToVisuals(axleInfo);
            }
        }

    }
}