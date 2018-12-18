using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
namespace EndlessRunner
{

    public class ScenePlacementRulesInspector : Editor
    {

        public static bool showScenes(ref List<ScenePlacementRule> scenePlacementRules, bool avoidedAfterScenes)
        {
            GUILayout.Label(string.Format(" {0} Scenes", (avoidedAfterScenes ? "Dont Spawn After" : "Avoided")), "BoldLabel");
            if (scenePlacementRules == null || scenePlacementRules.Count == 0)
            {
                GUILayout.Label(string.Format("No {0} Scenes", (avoidedAfterScenes ? "Dont Spawn After" : "avoided")));
                return false;
            }

            ScenePlacementRule scenePlacementRule;
            for (int i = 0; i < scenePlacementRules.Count; ++i)
            {
                scenePlacementRule = scenePlacementRules[i];

                // quick cleanup if the platform has gone null
                if (scenePlacementRule.scene == null)
                {
                    scenePlacementRules.RemoveAt(i);
                    return true;
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(string.Format("  {0}", scenePlacementRule.scene.name));
                if (GUILayout.Button("Remove"))
                {
                    scenePlacementRules.RemoveAt(i);
                    return true;
                }
                GUILayout.EndHorizontal();
            }

            return false;
        }

        public static int AddScene(ref List<ScenePlacementRule> scenePlacementRules, InfiniteObject scene, bool linkedscene)
        {
            if (scenePlacementRules == null)
            {
                scenePlacementRules = new List<ScenePlacementRule>();
            }
            if (scene == null)
            {
                return 1;
            }
            // Make sure there aren't any duplicates
            for (int i = 0; i < scenePlacementRules.Count; ++i)
            {
                if (scenePlacementRules[i].scene.GetInstanceID() == scene.GetInstanceID())
                    return 2;
            }

            scenePlacementRules.Add(new ScenePlacementRule(scene, linkedscene));
            return 0;
        }
    }
}