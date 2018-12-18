using UnityEngine;
using System.Collections.Generic;
namespace EndlessRunner
{
    public class CollidableAppearanceRules : AppearanceRules
    {

        public List<ScenePlacementRule> avoidScenes;
        public bool forceAppearanceInScenes;
        public List<SceneObject> ForceAppearScenes;
        public override void AssignIndexToObject(InfiniteObject infiniteObject, int index)
        {
            base.AssignIndexToObject(infiniteObject, index);

            for (int i = 0; i < avoidScenes.Count; ++i)
            {
                if (avoidScenes[i].AssignIndexToObject(infiniteObject, index))
                    break;
            }
        }

        public override bool CanSpawnObject(float distance, ObjectSpawnData spawnData)
        {
            if (!base.CanSpawnObject(distance, spawnData))
                return false;

            //for (int i = 0; i < avoidScenes.Count; ++i)
            //{
            //    Debug.Log("last local index spawned " + infiniteObjectHistory.GetLastLocalIndex(ObjectType.Scene));
            //    if (!avoidScenes[i].CanSpawnObject(infiniteObjectHistory.GetLastLocalIndex(ObjectType.Scene)))
            //        return false;
            //}

            // may not be able to spawn if the slots don't line up

            return (spawnData.slotPositions & ((thisInfiniteObject as CollidableObject).GetSlotPositionsMask())) != 0;
        }
    }
}
