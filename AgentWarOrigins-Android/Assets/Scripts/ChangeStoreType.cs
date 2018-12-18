using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeStoreType : MonoBehaviour
{
    public RectTransform[] Panels;

    void Awake()
    {
        int storeType= PlayerData.CurrentGameStats.CurrentStoreTypeClicked;
        switch (storeType)
        {
            case 1:
                ChangeStore(1);
                break;
            case 2:
                ChangeStore(2);
                break;
            default:
                ChangeStore(0);
                break;
        }

        PlayerData.CurrentGameStats.CurrentStoreTypeClicked = 0;  //reset store type
    }
    public void ChangeStore(int storeId)
    {
        for (int i = 0; i < Panels.Length; i++)
        {
            Panels[i].gameObject.SetActive(i == storeId);
        }
    }
 
}
