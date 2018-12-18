using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ObjectRotator : MonoBehaviour
{

    float rotSpeed = 20;
    private float speed = 25;
    #region ROTATE
    private float _sensitivity = 1f;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation = Vector3.zero;
    private RectTransform Objectpanel;

    public RectTransform Panel
    {
        get
        {
            return Objectpanel;
        }

        set
        {
            Objectpanel = value;
        }
    }

    public bool IsRotating { get; set; }

    #endregion

    void Update()
    {


        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(2,1,0), step);

        if (IsRotating)
        {

            // offset
            _mouseOffset = (Input.mousePosition - _mouseReference); // apply rotation
            _rotation.y = -(_mouseOffset.x + _mouseOffset.y) * _sensitivity; // rotate
            gameObject.transform.Rotate(_rotation); // store new mouse position
            _mouseReference = Input.mousePosition;
        }
    }

    //void OnMouseDown()
    //{
    //    // rotating flag
    //    _isRotating = true;

    //    // store mouse position
    //    _mouseReference = Input.mousePosition;
    //}

    //void OnMouseUp()
    //{
    //    // rotating flag
    //    _isRotating = false;
    //}

}
