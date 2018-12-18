using UnityEngine;
using System.Collections;
using EndlessRunner;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class Test : MonoBehaviour
{
    //public GameObject cube;
    private CapsuleCollider m_CapsuleCollider;
    private PlayerAnimation playerAnimation;

#if UNITY_EDITOR
    [MenuItem("Custom/Find Sprite")]
    public static void FindSprite()
    {
        var selected = Selection.activeGameObject;
        if (selected == null) return;
        var renderer = selected.GetComponent<SpriteRenderer>();
        if (renderer == null) return;
        Debug.Log(AssetDatabase.GetAssetPath(renderer.sprite));
    }

#endif


    private void Start()
    {
        m_CapsuleCollider = GetComponent<CapsuleCollider>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    private void Update()
    {
        //  Debug.Log(Util.IsObjectInFront(GameObject.FindGameObjectWithTag("Enemy"),cube));
        //update collider center based on animation curves
        if (playerAnimation.GetCurrentAnimationStateHashInLayer(0).IsName("Run_Jump")|| playerAnimation.GetCurrentTransitionInfoInLayer(0).IsName("Base Layer.Running -> Base Layer.Run_Jump"))
        {
            m_CapsuleCollider.center = new Vector3(0, playerAnimation.GetColliderCenter(), 0);

        }
        else
        {

            m_CapsuleCollider.center = new Vector3(0, playerAnimation.DefaultColliderCenter, 0);

        }

    }

}
