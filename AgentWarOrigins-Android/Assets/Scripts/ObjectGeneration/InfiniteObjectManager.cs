using UnityEngine;
using System.Collections.Generic;

namespace EndlessRunner
{
    public struct ObjectSpawnData
    {
        public float largestScene;
        public bool useWidthBuffer;
        public int slotPositions;
        public int section;
        public int prevSection;
        public bool sectionTransition;
    }

    /*
     * Used in conjuction with the infinite object generator, the manager keeps track of all of the objects. The infinite object generator requests a new
     * object through getNextObjectIndex/objectFromPool and the object manager will return the object from the object pool based on the appearance rules/
     * probability.
     */

    public class InfiniteObjectManager : MonoBehaviour
    {

        static public InfiniteObjectManager instance;

        // the number of objects to cache in the object pool before the game starts.
        public int SceneprespawnCache = 0;
        public int ObstaclespreSpwanCache=0;
        public int TokenspreSpawnCache;
        // Scene prefabs:
        public SceneObject[] scenes;
        public Transform sceneParent;

        // Obstacles:
        public CollidableObject[] obstacles;
        public Transform obstacleParent;

        // Coins:
        public CollidableObject[] tokens;
        public Transform tokensParent;
        
        //// Power ups:
        //public PowerUpObject[] powerUps;
        //public Transform powerUpParent;

        // Tutorial:
        public InfiniteObjectPersistence tutorialObjects;

        // Startup:
        public InfiniteObjectPersistence startupObjects;

        // Save all of the instantiated platforms in a pool to prevent instantiating and destroying objects
        private List<List<InfiniteObject>> objectsPool;
        private List<int> objectPoolIndex;

        private List<AppearanceRules> appearanceRules;
        private List<AppearanceProbability> appearanceProbability;
        private List<float> probabilityCache;
        private List<bool> objectCanSpawnCache;

        private InfiniteObjectHistory infiniteObjectHistory;

        public void Awake()
        {
            instance = this;
        }

        //private void Start()
        //{
        //    Init();
        //}

        public void Init()
        {
            infiniteObjectHistory = InfiniteObjectHistory.instance;

            objectsPool = new List<List<InfiniteObject>>();
            objectPoolIndex = new List<int>();

            appearanceRules = new List<AppearanceRules>();
            appearanceProbability = new List<AppearanceProbability>();
            probabilityCache = new List<float>();
            objectCanSpawnCache = new List<bool>();
            //add powerups if needed
            int totalObjs = scenes.Length + obstacles.Length + tokens.Length;
            InfiniteObject infiniteObject;
            for (int i = 0; i < totalObjs; ++i)
            {
                objectsPool.Add(new List<InfiniteObject>());
                objectPoolIndex.Add(0);

                probabilityCache.Add(0);
                objectCanSpawnCache.Add(false);

                infiniteObject = ObjectIndexToObject(i);
                infiniteObject.Init();
                appearanceRules.Add(infiniteObject.GetComponent<AppearanceRules>());
                appearanceRules[i].Init();
                appearanceProbability.Add(infiniteObject.GetComponent<AppearanceProbability>());
                appearanceProbability[i].Init();
            }
            // wait until all of the appearance rules have been initialized before the object index is assigned
            for (int i = 0; i < totalObjs; ++i)
            {
                infiniteObject = ObjectIndexToObject(i);
                for (int j = 0; j < totalObjs; ++j)
                {
                    ObjectIndexToObject(j).GetComponent<AppearanceRules>().AssignIndexToObject(infiniteObject, i);
                }
            }
            // cache a fixed amount of each type of object before the game starts
            List<InfiniteObject> infiniteObjects = new List<InfiniteObject>();
            for (int i = 0; i < SceneprespawnCache; ++i)
            {
                for (int j = 0; j < scenes.Length; ++j)
                {
                    infiniteObjects.Add(ObjectFromPool(j, ObjectType.Scene));
                }
                
            }
            for (int i = 0; i < ObstaclespreSpwanCache; i++)
            {
                for (int j = 0; j < obstacles.Length; j++)
                {
                    infiniteObjects.Add(ObjectFromPool(j, ObjectType.Obstacle));
                }
            }
            for (int i = 0; i < TokenspreSpawnCache; i++)
            {
                for (int j = 0; j < tokens.Length; j++)
                {
                    infiniteObjects.Add(ObjectFromPool(j, ObjectType.Token));
                }
            }

            for (int i = 0; i < infiniteObjects.Count; ++i)
            {

                infiniteObjects[i].Deactivate();
            }
        }

        // Measure the size of the platforms and scenes
        public void GetObjectSizes(out Vector3[] sceneSizes, out float largestSceneLength)
        {
            // the parent scene object must represent the children's size
            sceneSizes = new Vector3[scenes.Length];
            largestSceneLength = 0;
            for (int i = 0; i < scenes.Length; ++i)
            {
                Renderer sceneRenderer = scenes[i].GetComponent<Renderer>();
                if (sceneRenderer != null)
                {

                    sceneSizes[i] = sceneRenderer.bounds.size;
                  //  Debug.Log("renderer collider size" + sceneSizes[i]);

                }
                else
                {
                    sceneSizes[i] = new Vector3(scenes[i].GetCurrentSceneLength(), 0, 0);

                    //     Debug.Log("box collider size" + sceneSizes[i]);

                }
                //    sceneSizes[i] = scenes[i].GetComponent<BoxCollider>().terrainData.size;

                if (largestSceneLength < sceneSizes[i].x)
                {
                    largestSceneLength = sceneSizes[i].x;
                }
            }     
        }

        public void GetObjectStartPositions(out Vector3[] sceneStartPosition)
        {
            sceneStartPosition = new Vector3[scenes.Length];
            for (int i = 0; i < scenes.Length; ++i)
            {
                sceneStartPosition[i] = scenes[i].GetStartPosition();
            }
        }

        // Returns the specified object from the pool
        public InfiniteObject ObjectFromPool(int localIndex, ObjectType objectType)
        {
            InfiniteObject obj = null;
            int objectIndex = LocalIndexToObjectIndex(localIndex, objectType);
            List<InfiniteObject> objectPool = objectsPool[objectIndex];
            int poolIndex = objectPoolIndex[objectIndex];
            if (GameManager.m_InstantiatedPlayer)
            {
                PlayerControl playerControl = GameManager.m_InstantiatedPlayer.GetComponent<PlayerControl>();
                Vector3 targetPosition = Vector3.zero;
                if (playerControl)
                {
                    targetPosition = GameManager.m_InstantiatedPlayer.GetComponent<PlayerControl>().GetPlayerCurrentPosition();
                }

                // keep a start index to prevent the constant pushing and popping from the list	

                if (objectType == ObjectType.Scene)
                {
                    if (objectPool.Count > 0 && objectPool[poolIndex].IsActive() == false &&
                   (objectPool[poolIndex].transform.position - targetPosition).x +
                   objectPool[poolIndex].GetCurrentSceneLength() < 0)
                    {
                        obj = objectPool[poolIndex];
                        objectPoolIndex[objectIndex] = (poolIndex + 1) % objectPool.Count;
                        return obj;
                    }
                }
                else
                {
                    if (objectPool.Count > 0 && objectPool[poolIndex] == null)
                    {
                        objectPool.RemoveAt(poolIndex);
                    }
                    if (objectPool.Count > 0 && (objectPool[poolIndex].IsActive() == false))
                    {

                        obj = objectPool[poolIndex];
                        objectPoolIndex[objectIndex] = (poolIndex + 1) % objectPool.Count;
                        return obj;
                    }
                }

            }

            // No inactive objects, need to instantiate a new one
            InfiniteObject[] objects = null;
            switch (objectType)
            {
                case ObjectType.Scene:
                    objects = (SceneObject[])scenes;
                    break;
                case ObjectType.Obstacle:
                    objects = (CollidableObject[])obstacles;
                    break;
                case ObjectType.Token:
                    objects = (CollidableObject[])tokens;
                    break;
                    //add powerups later
                //case ObjectType.PowerUp:
                //    objects = powerUps;
                //    break;
            }

            obj = (Instantiate(objects[localIndex].gameObject,new Vector3(-1000f * (objectPool.Count + 1 ),objects[localIndex].gameObject.transform.position.y ,0),objects[localIndex].gameObject.transform.rotation) as GameObject).GetComponent<InfiniteObject>();

            AssignParent(obj, objectType);
            obj.SetLocalIndex(localIndex);

            objectPool.Insert(poolIndex, obj);
            objectPoolIndex[objectIndex] = (poolIndex + 1) % objectPool.Count;

            return obj;
        }
        public InfiniteObject ObjectFromPool(int localIndex, ObjectType objectType,bool canInstantiate)
        {
            InfiniteObject obj = null;
            int objectIndex = LocalIndexToObjectIndex(localIndex, objectType);
            List<InfiniteObject> objectPool = objectsPool[objectIndex];
            int poolIndex = objectPoolIndex[objectIndex];
            if (GameManager.m_InstantiatedPlayer)
            {
                PlayerControl playerControl = GameManager.m_InstantiatedPlayer.GetComponent<PlayerControl>();
                Vector3 targetPosition = Vector3.zero;
                if (playerControl)
                {
                    targetPosition = GameManager.m_InstantiatedPlayer.GetComponent<PlayerControl>().GetPlayerCurrentPosition();
                }
                // keep a start index to prevent the constant pushing and popping from the list	
                if (objectType == ObjectType.Scene)
                {
                    if (objectPool.Count > 0 && objectPool[poolIndex].IsActive() == false &&
                   (objectPool[poolIndex].transform.position - targetPosition).x +
                   objectPool[poolIndex].GetCurrentSceneLength() < 0)
                    {
                        obj = objectPool[poolIndex];
                        objectPoolIndex[objectIndex] = (poolIndex + 1) % objectPool.Count;
                        return obj;
                    }
                }
                else
                {
                    if (objectPool.Count > 0 && objectPool[poolIndex] == null)
                    {
                        objectPool.RemoveAt(poolIndex);
                    }
                    if (objectPool.Count > 0 && (objectPool[poolIndex].IsActive() == false))
                    {

                        obj = objectPool[poolIndex];
                        objectPoolIndex[objectIndex] = (poolIndex + 1) % objectPool.Count;
                        return obj;
                    }
                }

            }
            if (canInstantiate)
            {

                // No inactive objects, need to instantiate a new one
                InfiniteObject[] objects = null;
                switch (objectType)
                {
                    case ObjectType.Scene:
                        objects = (SceneObject[])scenes;
                        break;
                    case ObjectType.Obstacle:
                        objects = (CollidableObject[])obstacles;
                        break;
                    case ObjectType.Token:
                        objects = (CollidableObject[])tokens;
                        break;
                        //add powerups later
                        //case ObjectType.PowerUp:
                        //    objects = powerUps;
                        //    break;
                }
                obj = (Instantiate(objects[localIndex].gameObject, new Vector3(-1000f * (objectPool.Count + 1), objects[localIndex].gameObject.transform.position.y, 0), objects[localIndex].gameObject.transform.rotation) as GameObject).GetComponent<InfiniteObject>();

                AssignParent(obj, objectType);
                obj.SetLocalIndex(localIndex);

                objectPool.Insert(poolIndex, obj);
                objectPoolIndex[objectIndex] = (poolIndex + 1) % objectPool.Count;

            }

            return obj;

        }
        public void AssignParent(InfiniteObject infiniteObject, ObjectType objectType)
        {
            switch (objectType)
            { 

                case ObjectType.Scene:
                    infiniteObject.SetParent(sceneParent);
                    break;
                case ObjectType.Obstacle:
                    infiniteObject.SetParent(obstacleParent);
                    
                    break;
                case ObjectType.Token:
                    infiniteObject.SetParent(tokensParent);
                    break;
                    //add power up if needed
                //case ObjectType.PowerUp:
                //    infiniteObject.SetParent(powerUpParent);
                //    break;
            }
        }


        // Converts local index to object index
        public int LocalIndexToObjectIndex(int localIndex, ObjectType objectType)
        {
            switch (objectType) { 

                case ObjectType.Scene:
                    return localIndex;
                case ObjectType.Obstacle:
                    return scenes.Length + localIndex;
                case ObjectType.Token:
                    return scenes.Length + obstacles.Length + localIndex;
                //case ObjectType.PowerUp:
                //    return scenes.Length + obstacles.Length + tokens.Length + localIndex;
            }
            return -1; // error
        }
        // Converts object index to local index
        public int ObjectIndexToLocalIndex(int objectIndex, ObjectType objectType)
        {
            switch (objectType)
            {
                case ObjectType.Scene:
                    return objectIndex;
                case ObjectType.Obstacle:
                    return objectIndex - scenes.Length;
                case ObjectType.Token:
                    return objectIndex - scenes.Length - obstacles.Length;
                //case ObjectType.PowerUp:
                //    return objectIndex - scenes.Length - obstacles.Length - tokens.Length;
            }
            return -1; // error	
        }

        public InfiniteObject LocalIndexToInfiniteObject(int localIndex, ObjectType objectType)
        {
            switch (objectType)
            {
                case ObjectType.Scene:
                    return scenes[localIndex];
                case ObjectType.Obstacle:
                    return obstacles[localIndex];
                case ObjectType.Token:
                    return tokens[localIndex];
                //case ObjectType.PowerUp:
                //    return powerUps[localIndex];
            }
            return null; // error	
        }

        // Returns the number of total objects
        public int GetTotalObjectCount()
        {
            //add powerups if needed

            return scenes.Length + obstacles.Length + tokens.Length;
        }

        // Converts the object index to an infinite object
        private InfiniteObject ObjectIndexToObject(int objectIndex)
        {
            if (objectIndex < scenes.Length)
            {
                return scenes[objectIndex];
            }
            else if (objectIndex <  scenes.Length + obstacles.Length)
            {
                return obstacles[objectIndex - scenes.Length];
            }
            else if (objectIndex < scenes.Length + obstacles.Length + tokens.Length)
            {
                return tokens[objectIndex - scenes.Length - obstacles.Length];
            }
            //else if (objectIndex < scenes.Length + obstacles.Length + coins.Length + powerUps.Length)
            //{
            //    return powerUps[objectIndex - platforms.Length - scenes.Length - obstacles.Length - coins.Length];
            //}
            return null;
        }

        /**
         * The next platform is determined by probabilities as well as object rules.
         * spawnData contains any extra data that is needed to make a decision if the object can be spawned
         */
        public int GetNextObjectIndex(ObjectType objectType, ObjectSpawnData spawnData)
        {
            return GetNextObjectIndex(objectType, spawnData, 0);
        }

        public int GetNextObjectIndex(ObjectType objectType, ObjectSpawnData spawnData, float value)
        {
            InfiniteObject[] objects = null;
            switch (objectType)
            {
                case ObjectType.Scene:
                    objects = scenes;
                    break;
                case ObjectType.Obstacle:
                    objects = obstacles;
                    break;
                case ObjectType.Token:
                    objects = tokens;
                    break;
                    //case ObjectType.PowerUp:
                    //    objects = powerUps;
                    //    break;
            }
            float totalProbability = 0;
            float probabilityAdjustment = 0;
            float distance = 0f;
            distance = objectType == ObjectType.Scene ? infiniteObjectHistory.GetTotalSceneDistance() : value;
            int objectIndex;

            for (int localIndex = 0; localIndex < objects.Length; ++localIndex)
            {
                objectIndex = LocalIndexToObjectIndex(localIndex, objectType);
                // cache the result
                if (appearanceRules[objectIndex])
                {
                    objectCanSpawnCache[objectIndex] = appearanceRules[objectIndex].CanSpawnObject(distance, spawnData);
                    if (!objectCanSpawnCache[objectIndex])
                    {
                        //    Debug.Log( objectType + " of " + localIndex + "at " + distance + " cannot be spawned");
                        continue;
                    }
                    probabilityAdjustment = appearanceRules[objectIndex].ProbabilityAdjustment(distance);
                }
             
                // If the probability adjustment has a value of the float's max value then spawn this object no matter hwat
                if (probabilityAdjustment == float.MaxValue)
                {
                    probabilityCache[objectIndex] = probabilityAdjustment;
                    totalProbability = float.MaxValue;
                    break;
                }
                probabilityCache[objectIndex] = appearanceProbability[objectIndex].GetProbability(distance) * probabilityAdjustment;
           //     Debug.Log("probabiliy of " + objectType +" of" + localIndex +"at" + distance +"is" + probabilityCache[objectIndex]);
                totalProbability += probabilityCache[objectIndex];
            }

            // chance of spawning nothing (especially in the case of collidable objects)
            if (totalProbability == 0)
            {
                return -1;
            }

            float randomValue = Random.value;
            float prevObjProbability = 0;
            float objProbability = 0;
            // with the total probability we can determine a platform
            // minor optimization: don't check the last platform. If we get that far into the loop then regardless we are selecting that platform
            for (int localIndex = 0; localIndex < objects.Length - 1; ++localIndex)
            {
                objectIndex = LocalIndexToObjectIndex(localIndex, objectType);
                if (!objectCanSpawnCache[objectIndex])
                {
                    continue;
                }

                objProbability = probabilityCache[objectIndex];
                if (objProbability == float.MaxValue || randomValue <= (prevObjProbability + objProbability) / totalProbability)
                {
                    return localIndex;
                }
                prevObjProbability += objProbability;
            }
            return objects.Length - 1;
        }
        public GameObject CreateStartupObjects(bool tutorial)
        {
            InfiniteObjectPersistence prefab = (tutorial ? tutorialObjects : startupObjects);
            if (prefab != null)
            {
                return GameObject.Instantiate(prefab.gameObject) as GameObject;
            }
            return null;
        }
    }
}