
using EndlessRunner;
using UnityEngine;

public class IntroSceneCameraMovement : MonoBehaviour {

    public Transform startPosition;
    public Transform endPosition;
  //  public GameObject PostProcessGo;
    public float speed = 1.0F;
    private float startTime;
    private float journeyLength;
    private Vector3 targetPos;
    void Start()
    {
       // Screen.SetResolution(1280, 720, true);

        startTime = Time.time;
        endPosition = GameObject.FindGameObjectWithTag("Player").transform ;
        targetPos = endPosition.position + new Vector3(5, 2, 0);

        journeyLength = Vector3.Distance(startPosition.position, targetPos);
   
    }
    void Update()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        float remainingDistance = Vector3.Distance(transform.position, targetPos);
        if (remainingDistance > 0.05f)
        {
            //move camera towards target
            transform.position = Vector3.Lerp(startPosition.position, targetPos, fracJourney);

        }
        else
        {
            //enable scene canvas and menu canvas
            UiManager.Instance.EnableSceneCanvas();
            UiManager.Instance.EnableMenuCanvas();
            enabled = false;

            //transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
            //GetComponent<CameraController>().enabled = true;

        }


    }
}
