using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoadTracker : MonoBehaviour {

    public static SceneLoadTracker Instance;
    private static int lastSceneIndex = -1;

    private void Awake()
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

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += SceneUnloadedMethod;

    }
    private void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneUnloaded -= SceneUnloadedMethod;
    }

    // looks a bit funky but the method signature must match the scenemanager delegate signature
    void SceneUnloadedMethod(Scene sceneNumber)
    {
        int sceneIndex = sceneNumber.buildIndex;
        // only want to update last scene unloaded if were not just reloading the current scene
        if (lastSceneIndex != sceneIndex)
        {
            lastSceneIndex = sceneIndex;
            //Debug.Log("unloaded scene is : " + lastSceneIndex);
        }
    }
    public int GetLastSceneNumber()
    {
        return lastSceneIndex;
    }
}
