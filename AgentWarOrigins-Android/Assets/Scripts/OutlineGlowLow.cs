using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineGlowLow : MonoBehaviour
{
    private Renderer _renderers;
    private readonly float _startWidth = 1;
    private readonly float _endWidth = 2;
    // Use this for initialization
    private void Start()
    {
        _renderers = GetComponentInChildren<Renderer>(true);
    }

    // Update is called once per frame
    private void Update()
    {
        if (_renderers.material.HasProperty("_Outline"))
        {
            Debug.Log("has outline parameter");
            if (_renderers.material.GetFloat("_Outline") < _endWidth)
                _renderers.material.SetFloat("_Outline", Mathf.Lerp(_startWidth, _endWidth, 50*Time.deltaTime));
            else
                _renderers.material.SetFloat("_Outline", _startWidth);
        }
    }
}
