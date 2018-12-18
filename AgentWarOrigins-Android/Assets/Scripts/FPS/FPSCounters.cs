using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FPSCounters : MonoBehaviour
{
    public float fpsMeasurePeriod = 0.5f;
    public int maxFPS = 500;

    private int _accumulatedFPS;
    private float _nextFlushTime;
    private int _currentFPS;
    private string[] _fpsStrings;
    float deltaTime = 0.0f;
    private Text _textComponent;

    private void Start()
    {
        _nextFlushTime = Time.realtimeSinceStartup + fpsMeasurePeriod;
        _textComponent = GetComponent<Text>();

        _fpsStrings = new string[maxFPS + 1];
        for (int i = 0; i < maxFPS; i++)
        {
            _fpsStrings[i] = i.ToString() + " FPS";
        }
        _fpsStrings[maxFPS] = maxFPS.ToString() + "+ FPS";
    }

    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float msec = deltaTime * 1000.0f;
        float _currentFPS = 1.0f / deltaTime;
        if (_currentFPS <= maxFPS)
            {
                _textComponent.text = _fpsStrings[(int)_currentFPS];
            }
            else
            {
                _textComponent.text = _fpsStrings[maxFPS];
            }
        
    }
}
