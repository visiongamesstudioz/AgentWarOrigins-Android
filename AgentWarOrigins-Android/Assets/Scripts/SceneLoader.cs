using System.Collections;
using EndlessRunner;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    //should add loading bar and progress
    private AsyncOperation asyncLoad;

    // Use this for initialization
    public void LoadSceneAsync(int sceneIndex)
    {
        LoadSceneAsync(sceneIndex, true);
    }

    public void LoadSceneAsync(int sceneIndex, bool sceneTransition)
    {
        //SceneManager.LoadSceneAsync(sceneIndex)

        if (UiManager.Instance)
        {
            UiManager.Instance.DisableSceneCanavas();
        }

        StartCoroutine(sceneTransition ? LoadLevelWithSceneTransition(sceneIndex) : LoadLevel(sceneIndex));
    }

    public void LoadSceneAsync(int sceneIndex, bool sceneTransition, Canvas canvas)
    {
        //SceneManager.LoadSceneAsync(sceneIndex)

        if (UiManager.Instance)
        {
            UiManager.Instance.DisableSceneCanavas();
        }

        StartCoroutine(sceneTransition ? LoadLevelWithSceneTransition(canvas, sceneIndex) : LoadLevel(sceneIndex));
    }

    private IEnumerator LoadLevelWithSceneTransition(int levelIndex)
    {
        asyncLoad = SceneManager.LoadSceneAsync(levelIndex);
        asyncLoad.allowSceneActivation = false;
        var canvas = GameObject.Find("Canvas");
        if (canvas.GetComponent<Canvas>())
        {
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        }

        GameObject loadingScreen = Util.FindGameObjectWithName(canvas, "Loading Screen");
        if (loadingScreen)
        {
            loadingScreen.gameObject.SetActive(true);
            var slider = loadingScreen.GetComponentInChildren<Slider>(true);
            slider.gameObject.SetActive(true);
            //show banner ad
            //     AdmobAdManager.Instance.RequestBanner();
            while (!asyncLoad.isDone)
            {
                if (slider)
                {
                    slider.value = asyncLoad.progress;
                    if (asyncLoad.progress == 0.9f)
                    {
                        slider.value = 1f;
                        asyncLoad.allowSceneActivation = true;
                    }
                }

                //destroy bannr ad
                yield return null;
            }
        }

    }

    private IEnumerator LoadLevelWithSceneTransition(Canvas canvas, int levelIndex)
    {
        asyncLoad = SceneManager.LoadSceneAsync(levelIndex);
        asyncLoad.allowSceneActivation = false;

        if (canvas)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        GameObject loadingScreen = Util.FindGameObjectWithName(canvas.gameObject, "Loading Screen");
        if (loadingScreen)
        {
            loadingScreen.gameObject.SetActive(true);
            var slider = loadingScreen.GetComponentInChildren<Slider>(true);
            slider.gameObject.SetActive(true);
            //show banner ad
            //     AdmobAdManager.Instance.RequestBanner();
            while (!asyncLoad.isDone)
            {
                if (slider)
                {
                    slider.value = asyncLoad.progress;
                    if (asyncLoad.progress == 0.9f)
                    {
                        slider.value = 1f;
                        asyncLoad.allowSceneActivation = true;
                    }
                }

                //destroy banner ad
                yield return null;
            }
        }

    }

    private IEnumerator LoadLevel(int levelIndex)
    {
        asyncLoad = SceneManager.LoadSceneAsync(levelIndex);
        asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress == 0.9f)
                asyncLoad.allowSceneActivation = true;
            yield return null;
        }

    }
}
 

