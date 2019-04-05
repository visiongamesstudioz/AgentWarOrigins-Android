using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleBackPress : MonoBehaviour
{

    public static HandleBackPress Instance;
    //A list to keep track of all the scenes you've loaded so far
    private List<string> previousScenes = new List<string>();

 
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().buildIndex==2)
            {
                GameObject canvas=GameObject.Find("Canvas");
                if (canvas != null)
                {
                    GameObject exitApp = Util.FindGameObjectWithName(canvas, "ExitApp");
                    exitApp.SetActive(true);
                }
            
            }
            else
            {
                CurrentGameStats currentGameStats = PlayerData.CurrentGameStats;
                Util.ResetCurrentGameStats(currentGameStats);
                GameObject sceneLoader = new GameObject { name = "SceneLoader" };
                sceneLoader.AddComponent<SceneLoader>();
                sceneLoader.GetComponent<SceneLoader>().LoadSceneAsync(2);
            }
        }

    }
}
