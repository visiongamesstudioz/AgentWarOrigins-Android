using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;
using UnityEngine.UI;

public class TapToShoot : MonoBehaviour {

    public RectTransform targetCanvas;
    private Transform ObjectToFollow;
    private DepthUIScript depthUIScript;
    public Image TapToShootIcon;
    private GameObject player;

    // Use this for initialization
    void Start () {

        targetCanvas = GameObject.Find("TutorialCanvas").GetComponent<RectTransform>();
        player = GameObject.FindGameObjectWithTag("Player");

        SetTapToShootData();

        targetCanvas.GetComponent<ScreenSpaceCanvasScript>().AddToCanvas(TapToShootIcon.GetComponent<DepthUIScript>());
    }
	
	// Update is called once per frame
	void Update () {
        RepositionTapToShootIcon();
	}

    private void RepositionTapToShootIcon()
    {
        Camera activeCamera= UiManager.Instance.GetActiveCamera();
        if (activeCamera == null)
        {
            activeCamera=Camera.main;
        }
        if (ObjectToFollow)
        {

            Vector3 ViewportPosition = activeCamera.WorldToViewportPoint(ObjectToFollow.position);
            Vector3 WorldObject_ScreenPosition = new Vector3(
                ((ViewportPosition.x * targetCanvas.sizeDelta.x) - (targetCanvas.sizeDelta.x * 0.5f)),
                ((ViewportPosition.y * targetCanvas.sizeDelta.y) - (targetCanvas.sizeDelta.y * 0.5f)), ViewportPosition.z);
            //now you can set the position of the ui element
            TapToShootIcon.rectTransform.anchoredPosition = WorldObject_ScreenPosition;

            float distance = (WorldObject_ScreenPosition - activeCamera.transform.position).magnitude;
            depthUIScript.depth = -distance;
        }

    }

    public void SetObjectToFollow(GameObject objectToFollow)
    {
        ObjectToFollow = objectToFollow.transform;
    }

    #region PUBLIC_METHODS

    public void SetTapToShootData()
    {
        TapToShootIcon = GetComponent<Image>();
        depthUIScript = TapToShootIcon.gameObject.GetComponent<DepthUIScript>();
        TapToShootIcon.transform.SetParent(targetCanvas, false);
        RepositionTapToShootIcon();
      //  TapToShootIcon.gameObject.SetActive(true);
    }
    #endregion
}
