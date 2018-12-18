using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;

public class VehicleMovement : ObstacleObject {

    public float MaxDistanceBetweenPlayerToStart;
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public float brakeTorque = 30000f;
    private GameObject m_Player;
    public override void Awake()
    {
        base.Awake();
        m_RigidBody.useGravity = true;
    }

    private void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");

    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            m_RigidBody.useGravity = true;
            m_RigidBody.isKinematic = false;
            childColliders[0].isTrigger = false;
            //get parent scene object
            SceneObject sceneObject = other.gameObject.GetComponentInParent<SceneObject>();
            //check for scene spawn
            if (sceneObject)
            {
                for (int i = 0; i < m_CollidableAppearanceRules.avoidScenes.Count; i++)
                {
                    //Debug.Log("scene index hit is "+ name + sceneObject.SceneLocalIndex);
                    //Debug.Log("avoid scene index of " + name + "is" + m_CollidableAppearanceRules.avoidScenes[i].sceneIndex);
                    if (!m_CollidableAppearanceRules.avoidScenes[i].CanSpawnObject(sceneObject.SceneLocalIndex))
                    {
                        Deactivate();
                    }

                }
            }

        }
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

        if (m_Player)
        {
            if (Vector3.Distance(transform.position, m_Player.transform.position) < MaxDistanceBetweenPlayerToStart)
            {
                foreach (AxleInfo axleInfo in axleInfos)
                {
                    if (transform.position.x -m_Player.transform.position.x < 2f)
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
                        axleInfo.leftWheel.motorTorque = maxMotorTorque;
                        axleInfo.rightWheel.motorTorque = maxMotorTorque;
                    }
                    ApplyLocalPositionToVisuals(axleInfo);
                }
            }
          
        }

    }
}
