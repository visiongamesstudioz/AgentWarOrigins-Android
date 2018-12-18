using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleOnCutSceneComplete : MonoBehaviour {

    private void OnDisable()
    {
        Util.OnCutSceneComplete();
    }


}
