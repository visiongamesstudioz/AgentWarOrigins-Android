using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using Mono.CSharp;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(SceneAppearanceRules))]
public class SceneAppearanceRulesInspector : AppearanceRulesInspector
{
    private SceneObject targetScene = null;
    private bool addnewCannotSpawnAfterScene = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10);
        SceneAppearanceRules sceneAppearanceRules = (SceneAppearanceRules) target;
        List<ScenePlacementRule> targetScenes = sceneAppearanceRules.TargetSceneObjects;
      //  addnewCannotSpawnAfterScene = sceneAppearanceRules.CannotSpawnAfterScene;

        if (ScenePlacementRulesInspector.showScenes(ref targetScenes, true))
        {
            sceneAppearanceRules.TargetSceneObjects = targetScenes;

            EditorUtility.SetDirty(target);
        }
     //   addnewCannotSpawnAfterScene = EditorGUILayout.Toggle("has Cannot Spawn After Scenes", addnewCannotSpawnAfterScene);
        if (addnewCannotSpawnAfterScene)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Target Scene");
            targetScene = EditorGUILayout.ObjectField(targetScene, typeof(SceneObject), false) as SceneObject;
            GUILayout.EndHorizontal();

            if (addError.Length > 0)
            {
                GUI.contentColor = Color.red;
                GUILayout.Label(addError);
                GUI.contentColor = Color.white;
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                int error;
                if ((error = ScenePlacementRulesInspector.AddScene(ref targetScenes, targetScene, false)) == 0)
                {
                    addnewCannotSpawnAfterScene = false;
                    sceneAppearanceRules.TargetSceneObjects = targetScenes;
                    EditorUtility.SetDirty(target);
                }
                else
                {
                    switch (error)
                    {
                        case 1:
                            addError = "Error: Target Scene is not set";
                            break;
                        case 2:
                            addError = "Error: Target Scene has already been added";
                            break;
                        default:
                            addError = "Unknown Error";
                            break;
                    }
                }
            }

            if (GUILayout.Button("Cancel"))
            {
                addnewCannotSpawnAfterScene = false;
            }
            GUILayout.EndHorizontal();
        }

        if (!addnewCannotSpawnAfterScene && GUILayout.Button("Add Dont Spawn After Scene"))
        {
            addError = "";
            addnewCannotSpawnAfterScene = true;
        }


    }
}
