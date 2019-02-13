
using System.Collections.Generic;
using Cinemachine;
using EndlessRunner;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public List<CinemachineVirtualCamera> FollowPlayerVirtualCameras=new List<CinemachineVirtualCamera>();
    public List<CinemachineVirtualCamera> LookAtPlayerVirtualCameras=new List<CinemachineVirtualCamera>();
    public GameObject CinemachineCameras;
    public GameObject CutSceneGameObject;
    public GameObject GamePlayGameObject;
    public Camera TutorialCamera;
    public GameObject InfiniteObjects;
    public GameObject TutorialCanvas;
    [HideInInspector]
    public GameObject GameManager;
    public PlayableDirector CutSceneTimeLinePlayableDirector;
    public PlayableDirector GmActivatorPlayable;
    public Animator PlayerAnimator;
  //  public PlayerControl playerControl;
    public PlayerAnimation PlayerAnimation;
    //public PlayerHealth PlayerHealth;
    public InputControl TutorialInput;
    // Use this for initialization
    void Start () {


        if (!Util.IsTutorialComplete())
	    {
            CutSceneGameObject.SetActive(true);
        }
	    else
	    {
            TutorialCanvas.gameObject.SetActive(false);
            CinemachineCameras.SetActive(false);
            CutSceneGameObject.SetActive(false);
            GmActivatorPlayable.Play();
            GamePlayGameObject.SetActive(true);
            GameManager= GameObject.FindGameObjectWithTag("GameController");

            InfiniteObjects.SetActive(true);

            //GameObject endPosition = GameObject.FindGameObjectWithTag("Player");
            //   if (endPosition)
            //   {
            //       Vector3 targetPos = endPosition.transform.position + new Vector3(5, 2, 0);
            //      TutorialCamera.transform.position = targetPos;  
	        //   

	        TutorialCamera.gameObject.SetActive(true);

	        TutorialCamera.GetComponent<IntroSceneCameraMovement>().enabled = true;

            GameObject player = EndlessRunner.GameManager.m_InstantiatedPlayer;
	        PlayerAnimator = player.GetComponent<Animator>();
	        TutorialInput = player.GetComponent<InputControl>();

	        PlayerAnimation = player.GetComponent<PlayerAnimation>();
	        if (PlayerAnimator.runtimeAnimatorController == null)
	        {
	            PlayerAnimator.runtimeAnimatorController = player.GetComponent<Player>().PlayerAnimatorController;

            }
            //   TutorialCamera.gameObject.SetActive(true);
            //   TutorialCamera.gameObject.SetActive(true);
            TutorialInput.enabled = true;
	        PlayerAnimation.enabled = true;

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
        GameObject player= EndlessRunner.GameManager.m_InstantiatedPlayer;
        PlayerAnimator = player.GetComponent<Animator>();
        TutorialInput = player.GetComponent<InputControl>();

        PlayerAnimation = player.GetComponent<PlayerAnimation>();

        if (PlayerAnimator.runtimeAnimatorController == null)
        {
            PlayerAnimator.runtimeAnimatorController = player.GetComponent<Player>().PlayerAnimatorController;

        }
        TutorialCamera.gameObject.SetActive(true);
        if (!Util.IsTutorialComplete())
        {
            TutorialCamera.GetComponent<CameraController>().enabled = true;
        }
     //   TutorialCamera.gameObject.SetActive(true);
        TutorialInput.enabled = true;
        PlayerAnimation.enabled = true;
        //PlayerHealth.enabled = true;

    }


    public void SetVirtualCameraFollowObject(Transform followObject)
    {
        for (int i = 0; i < FollowPlayerVirtualCameras.Count; i++)
        {
            FollowPlayerVirtualCameras[i].Follow = followObject;
        }
    }

    public void SetLookUpCameraObject(Transform lookUpTransform)
    {
        for (int i = 0; i < LookAtPlayerVirtualCameras.Count; i++)
        {
            LookAtPlayerVirtualCameras[i].LookAt = lookUpTransform;
        }
    }

    public void PlayCutScene()
    {
        CutSceneTimeLinePlayableDirector.RebuildGraph();
        CutSceneTimeLinePlayableDirector.Play();
    }


}
