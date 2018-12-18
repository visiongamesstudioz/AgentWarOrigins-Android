
using EndlessRunner;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public GameObject CinemachineCameras;
    public GameObject CutSceneGameObject;
    public GameObject GamePlayGameObject;
    public GameObject InfiniteObjects;
    public GameObject TutorialCanvas;
    [HideInInspector]
    public GameObject GameManager;
    public PlayableDirector CutSceneTimeLinePlayableDirector;
    public PlayableDirector GmActivatorPlayable;

    public RuntimeAnimatorController PlayerAnimatorController;
    public Animator PlayerAnimator;
  //  public PlayerControl playerControl;
    public PlayerAnimation PlayerAnimation;
    public PlayerHealth PlayerHealth;
    public TutorialInput TutorialInput;
    // Use this for initialization
    void Start () {
        
	    if (!Util.IsTutorialComplete())
	    {

            CutSceneGameObject.SetActive(true);
	        CutSceneTimeLinePlayableDirector.Play();
	    }
	    else
	    {
            TutorialCanvas.gameObject.SetActive(false);
            CinemachineCameras.SetActive(false);
            CutSceneGameObject.SetActive(false);
            GmActivatorPlayable.Play();
            GamePlayGameObject.SetActive(true);
            GameManager= GameObject.FindGameObjectWithTag("GameController");

            Camera mainCamera=Camera.main;
            InfiniteObjects.SetActive(true);

	        GameObject endPosition = GameObject.FindGameObjectWithTag("Player");
            if (endPosition)
            {
                Vector3 targetPos = endPosition.transform.position + new Vector3(5, 2, 0);
                mainCamera.transform.position = targetPos;
            }

	        UiManager.Instance.EnableSceneCanvas();
            if (SceneLoadTracker.Instance.GetLastSceneNumber() == 2 || SceneLoadTracker.Instance.GetLastSceneNumber()==1)
            {
                return;
            }
	        UiManager.Instance.EnableMenuCanvas();

	    }
	}

    public void StopTimeline()
    {
        CutSceneTimeLinePlayableDirector.Stop();
        PlayerAnimator.runtimeAnimatorController = PlayerAnimatorController;
        TutorialInput.enabled = true;
       // playerControl.enabled = true;
        PlayerAnimation.enabled = true;
        PlayerHealth.enabled = true;

    }
}
