using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MuzzleFlash : MonoBehaviour
{
    [Tooltip("The alpha value to initialize the muzzle flash material to")]
    [SerializeField]
    private float m_StartAlpha = 0.5f;
    [SerializeField]
    [Tooltip("The minimum fade speed - the larger the value the quicker the muzzle flash will fade")]
    private float m_MinFadeSpeed = 1.25f;
    [Tooltip("The maximum fade speed - the larger the value the quicker the muzzle flash will fade")]
    [SerializeField]
    private float m_MaxFadeSpeed = 1.25f;

    [SerializeField] [Tooltip("Start Local Scale")] private Vector3 m_StartLocalScale;
    [SerializeField]
    [Tooltip("End Local Scale")]
    private Vector3 m_EndLocalScale;


    private const string TintColor = "_TintColor";
    private Color m_Color;
    private float m_StartLightIntensity;
    private float m_FadeSpeed;
    private GameObject m_GameObject;
    private Material m_Material;
    private Light m_Light;


    private void Awake()
    {

        this.m_Light = this.GetComponent<Light>();

        if (!this.m_Light)
            return;
        this.m_StartLightIntensity = this.m_Light.intensity;
    }


    private void Update()
    {
        if (this.m_Color.a > 0.0)
        {
         
            if (!this.m_Light)
                return;
            this.m_Light.intensity = this.m_StartLightIntensity * (this.m_Color.a / this.m_StartAlpha);
        }
        transform.localScale=Vector3.Lerp(m_StartLocalScale,m_EndLocalScale,Time.deltaTime *100);

    }
}
