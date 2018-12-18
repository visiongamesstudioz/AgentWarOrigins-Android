using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableStaticBatching : MonoBehaviour
{
    public static EnableStaticBatching Instance;
    public GameObject SceneParent;

    public void Awake()
    {
        Instance = this;
    }
    public void StaticCombineSceneObjects()
    {
        StaticBatchingUtility.Combine(SceneParent);
    }
}
