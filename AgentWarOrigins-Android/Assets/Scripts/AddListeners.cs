using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddListeners : MonoBehaviour
{

    public Button MainMissionsDisplayButton;
    public Button CompletedMissionsDisplayButton;
	// Use this for initialization
	void Start () {
		
        MainMissionsDisplayButton.onClick.AddListener(MissionManager.Instance.DisplayActiveMissions);
        CompletedMissionsDisplayButton.onClick.AddListener(MissionManager.Instance.DisplayCompletedMissions);
	}
}
