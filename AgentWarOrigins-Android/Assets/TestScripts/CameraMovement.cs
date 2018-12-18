using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float SmoothingFactor;
    public Transform DefaultPosition;
    public CharacterShowInput m_CharacterShowInput;


    public void MoveCamera(float changePosX)
    {
        float targetPosX = transform.position.x - changePosX;

        Vector3 targetPosition = new Vector3(targetPosX, transform.position.y, transform.position.z);
        ChangeGoTargetPosition(targetPosition);
    }

    private void ChangeGoTargetPosition(Vector3 target)
    {
        var timeToStart = Time.time;
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            m_CharacterShowInput.AcceptInput = false;
            //transform.position = Vector3.Lerp(transform.position, target,
            //        (Time.time - timeToStart) * SmoothingFactor);
            transform.position = target;
            //Here speed is the 1 or any number which decides the how fast it reach to one to other end.
            //leftButton.GetComponent<Button>().enabled = false;
            //rightButton.GetComponent<Button>().enabled = false;
        }
        m_CharacterShowInput.AcceptInput = true;
    }

    public void ResetCamera()
    {
        transform.position = DefaultPosition.position;
    }

}
