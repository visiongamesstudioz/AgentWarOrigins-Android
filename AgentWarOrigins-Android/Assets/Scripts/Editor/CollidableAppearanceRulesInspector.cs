using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
namespace EndlessRunner { 
/*
     * Collidable Appearance Rules need everything the Appearance Rules do, plus an option to select the platforms to avoid
     */
[CustomEditor(typeof(CollidableAppearanceRules))]
public class CollidableAppearanceRulesInspector : AppearanceRulesInspector
{
    private SceneObject targetScene=null;
    private bool addNewAvoidScene = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        CollidableAppearanceRules collidableAppearanceRules = (CollidableAppearanceRules)target;
        List<ScenePlacementRule> scenePlacementRules = collidableAppearanceRules.avoidScenes;
            
        if (ScenePlacementRulesInspector.showScenes(ref scenePlacementRules, false))
        {
            collidableAppearanceRules.avoidScenes = scenePlacementRules;
           
            EditorUtility.SetDirty(target);
        }

        if (addNewAvoidScene)
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
                if ((error = ScenePlacementRulesInspector.AddScene(ref scenePlacementRules, targetScene, false)) == 0)
                {
                     addNewAvoidScene = false;
                    collidableAppearanceRules.avoidScenes = scenePlacementRules;
                    EditorUtility.SetDirty(target);
                }
                else {
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
                    addNewAvoidScene = false;
            }
            GUILayout.EndHorizontal();
        }

        if (!addNewAvoidScene && GUILayout.Button("Add Avoid Scene"))
        {
            addError = "";
                addNewAvoidScene = true;
        }
    }
}
    }