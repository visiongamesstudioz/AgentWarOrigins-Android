using UnityEngine;
using System.Collections;
using EndlessRunner;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    #region PRIVATE_VARIABLES
    private Vector2 positionCorrection = new Vector2(0, 100);
    #endregion
    #region PUBLIC_REFERENCES
    public RectTransform targetCanvas;
    public RectTransform healthBar;
    private Image healthBarSlider;
    public GameObject EnemyGameObject;
    public Transform objectToFollow;
    private DepthUIScript depthUIScript;
    private GameObject player;
    private Camera activeCamera;
    private void Start()
    {
        targetCanvas = GameObject.Find("HealthBarCanvas").GetComponent<RectTransform>();
        player = GameObject.FindGameObjectWithTag("Player");
        activeCamera = Camera.main;
        SetHealthBarData();
        
        targetCanvas.GetComponent<ScreenSpaceCanvasScript>().AddToCanvas(healthBar.GetComponent<DepthUIScript>());
    }
    #endregion
    #region PUBLIC_METHODS

    public void SetHealthBarData()
    {
        healthBar = GetComponent<RectTransform>();
        healthBarSlider = GetComponentInChildren<Image>();
        depthUIScript = healthBar.gameObject.GetComponent<DepthUIScript>();
        healthBar.SetParent(targetCanvas,false);
        RepositionHealthBar();
        healthBar.gameObject.SetActive(true);
    }
    #endregion
    #region UNITY_CALLBACKS
    void Update()
    {
        RepositionHealthBar();

    }
    #endregion
    #region PRIVATE_METHODS
    private void RepositionHealthBar()
    {
        if (UiManager.Instance)
        {
            activeCamera = UiManager.Instance.GetActiveCamera();
        }
        
        if (activeCamera == null)
        {
            activeCamera=Camera.main;
        }
        if (activeCamera)
        {
            Vector3 ViewportPosition = activeCamera.WorldToViewportPoint(objectToFollow.position);
            Vector3 WorldObject_ScreenPosition = new Vector3(
            ((ViewportPosition.x * targetCanvas.sizeDelta.x) - (targetCanvas.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * targetCanvas.sizeDelta.y) - (targetCanvas.sizeDelta.y * 0.5f)), ViewportPosition.z);
            //now you can set the position of the ui element
            healthBar.anchoredPosition = WorldObject_ScreenPosition;

            float distance = (WorldObject_ScreenPosition -activeCamera.transform.position).magnitude;
            depthUIScript.depth = -distance;

            bool isinFront = Util.IsObjectInFront(player, EnemyGameObject);
            float distanceToPlayer = Vector3.Distance(player.transform.position, EnemyGameObject.transform.position);
            if (isinFront && distanceToPlayer < 200 && EnemyGameObject.activeSelf)
            {
                healthBarSlider.enabled = true;
            }
            else
            {
                healthBarSlider.enabled = false;
            }
        }

    }
    #endregion
}