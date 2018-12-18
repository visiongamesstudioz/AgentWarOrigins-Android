using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RandomTipGenerator : MonoBehaviour
{

    public Text TipText;
    public string[] TipStrings;
	// Use this for initialization
	void Start ()
	{
	    int random = Random.Range(0, TipStrings.Length);
	    ShowRandomTip(random);
	}

    void ShowRandomTip(int i)
    {
        TipText.text = TipStrings[i];
    }
}
