using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadLevel : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        //temporarily set tutotial completed
        PlayerPrefs.SetInt("IsTutorialComplete", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }
}
