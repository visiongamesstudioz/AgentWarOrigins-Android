using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;
    private GUIStyle style;
    private Rect rect;
    private int w;
    private int h;
    private void Start()
    {
        w = Screen.width;
        h = Screen.height;

        style = new GUIStyle();

        rect = new Rect(0, 0, w, h * 2 / 100);
    }




    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {

        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = fps.ToString();

        if (fps < 15)
        {
            style.normal.textColor = Color.red;

        }
        else
        {
            style.normal.textColor = Color.green;
        }
        GUI.Label(rect, text, style);

    }
}
