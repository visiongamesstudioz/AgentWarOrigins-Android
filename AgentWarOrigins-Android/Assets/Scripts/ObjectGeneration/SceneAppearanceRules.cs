using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;

namespace EndlessRunner
{
    public class SceneAppearanceRules : AppearanceRules
    {
        public bool CannotSpawnAfterScene;
        public List<ScenePlacementRule> TargetSceneObjects;

        public override bool CanSpawnObject(float distance, ObjectSpawnData spawnData)
        {
            if (!base.CanSpawnObject(distance,spawnData))
            {
                return false;
            }

            if (CannotSpawnAfterScene || TargetSceneObjects.Count > 0)
            {
                InfiniteObject prevObject= infiniteObjectHistory.GetTopInfiniteObject(ObjectLocation.Center, true);
                if (prevObject != null)
                {
                    string prevObjectName = prevObject.name;


                    foreach (var targetObject in TargetSceneObjects)
                    {

                        if (prevObjectName.Contains(targetObject.scene.name))
                        {
                            return false;
                        }
                        //if (prevObjectName.Equals(targetObject.scene.name))
                        //{
                        //    return false;
                        //}

                    }
                }
            
            }
            return true;
        }
    }


}
