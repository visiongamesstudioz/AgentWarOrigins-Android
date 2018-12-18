using UnityEngine;
using System.Collections.Generic;

namespace EndlessRunner
{

    /*
     * Infinite Object History keeps a record of the objects spawned which is mostly used by the appearance rules
     */
    public class InfiniteObjectHistory : MonoBehaviour
    {

         public static InfiniteObjectHistory instance;

        // local index is the index of the object within its own array
        // object index is the index of the object unique to all of the other objects (array independent)
        // spawn index is the index of the object spawned within its own object type.
        // 
        // For example, the following objects would have the corresponding local, object and spawn indexes:
        //
        // Name			Local Index		Object Index	Spawn Index     Notes
        // PlatformA	0				0 				0               
        // ObstacleA	0				3				0               ObstacleA has an object index of 3 because it is the third object in the complete object array:
        // PlatformB	1				1				1                   PlatformA, PlatformB, PlatformC, ObstacleA, ...
        // ObstacleA	0				3				1
        // PlatformC	2				2				2
        // ObstacleB	1				4				2
        // ObstacleC	2				5				3
        // PlatformA	0				0				3
        // ObstacleA	0				3				4
        // ObstacleC	2				5				5
        // PlatformC	2				2				4

        // The relative location of the objects being spawned: Center, Right, Left
        private ObjectLocation activeObjectLocation;

        // Spawn index for the given object index
        private List<int>[] objectSpawnIndex;
        // Spawn index for the given object type
        private int[][] objectTypeSpawnIndex;

        // local index for the given object type
        private int[][] lastLocalIndex;
        // spawn location (distance) for the given object index
        private List<float>[] lastObjectSpawnDistance;


        // distance of the last spawned object for the given object type
        private float[][] latestObjectTypeSpawnDistance;

        // angle for each object location
        private float[] objectLocationAngle;

        // The total distance spawned for both platforms and scenes
        private float[] totalDistance;
        private float[] totalSceneDistance;
        private Vector3 latestCollidableSpawnPosition;
        // Keep track of the top-most and bottom-most objects in the scene hierarchy. When a new object is spawned, it is placed as the parent of the respective previous
        // objects. When the generator moves the platforms and scenes, it will only need to move the top-most object. It will also only need to check the bottom-most object
        // to see if it needs to be removed
        private SceneObject[] topSceneObjectSpawned;
        private InfiniteObject[] topCollidableSpawned;
        private InfiniteObject[] bottomSceneObjectSpawned;

        private InfiniteObject savedInfiniteObjects;

        private int[] previousSceneSection;
        private bool[] spawnedSceneSectionTransition;

        private InfiniteObjectManager infiniteObjectManager;

        public void Awake()
        {
            instance = this;

        }

        public void OnEnable()
        {

        }
        public void Start()
        {

        }
        public void Init(int objectCount)
        {
            activeObjectLocation = ObjectLocation.Center;
            objectSpawnIndex = new List<int>[(int)ObjectLocation.Last];
            objectTypeSpawnIndex = new int[(int)ObjectLocation.Last][];
            lastLocalIndex = new int[(int)ObjectLocation.Last][];
            latestObjectTypeSpawnDistance = new float[(int)ObjectLocation.Last][];
            lastObjectSpawnDistance = new List<float>[(int)ObjectLocation.Last];

            objectLocationAngle = new float[(int)ObjectLocation.Last];

            totalDistance = new float[(int)ObjectLocation.Last];
            totalSceneDistance = new float[(int)ObjectLocation.Last];

         
            topSceneObjectSpawned = new SceneObject[(int)ObjectLocation.Last];
            bottomSceneObjectSpawned = new InfiniteObject[(int)ObjectLocation.Last];
            topCollidableSpawned=new InfiniteObject[(int)ObjectLocation.Last];
            previousSceneSection = new int[(int)ObjectLocation.Last];
            spawnedSceneSectionTransition = new bool[(int)ObjectLocation.Last];

            for (int i = 0; i < (int)ObjectLocation.Last; ++i)
            {
                objectSpawnIndex[i] = new List<int>();
                objectTypeSpawnIndex[i] = new int[(int)ObjectType.Last];
                lastLocalIndex[i] = new int[(int)ObjectType.Last];
                latestObjectTypeSpawnDistance[i] = new float[(int)ObjectType.Last];

                lastObjectSpawnDistance[i] = new List<float>();


                for (int j = 0; j < objectCount; ++j)
                {
                    objectSpawnIndex[i].Add(-1);
                    lastObjectSpawnDistance[i].Add(0);
                }
                for (int j = 0; j < (int)ObjectType.Last; ++j)
                {
                    objectTypeSpawnIndex[i][j] = -1;
                    lastLocalIndex[i][j] = -1;
                    latestObjectTypeSpawnDistance[i][j] = -1;
                }
            }

            infiniteObjectManager = InfiniteObjectManager.instance;
        }

        // set the new active location
        public void SetActiveLocation(ObjectLocation location)
        {
            activeObjectLocation = location;
        }

        public InfiniteObject SceneObjectSpawned(int index, float locationOffset, ObjectLocation location, float angle, ObjectType objectType)
        {
            return SceneObjectSpawned(index, location, angle, objectType, null);
        }


        // Keep track of the object spawned. Returns the previous object at the top position
        public SceneObject SceneObjectSpawned(int index, ObjectLocation location, float angle, ObjectType objectType, SceneObject infiniteObject)
        {
            lastObjectSpawnDistance[(int)location][index] = totalSceneDistance[(int)location];
            objectTypeSpawnIndex[(int)location][(int)objectType] += 1;
            objectSpawnIndex[(int)location][index] = objectTypeSpawnIndex[(int)location][(int)objectType];
            latestObjectTypeSpawnDistance[(int)location][(int)objectType] = lastObjectSpawnDistance[(int)location][index];
            lastLocalIndex[(int)location][(int)objectType] = infiniteObjectManager.ObjectIndexToLocalIndex(index, objectType);

            SceneObject prevTopObject = null;

            if (objectType == ObjectType.Scene)
            {
                prevTopObject = topSceneObjectSpawned[(int)location];
                topSceneObjectSpawned[(int)location] = infiniteObject;
            }

            return prevTopObject;
        }
        //overload
        public SceneObject SceneObjectSpawned(int index, ObjectLocation location, ObjectType objectType, SceneObject infiniteObject)
        {
            return SceneObjectSpawned(index, location, 0, objectType, infiniteObject);
        }

        public CollidableObject CollidableObjectSpawned(int index, ObjectLocation location, ObjectType objectType,
            CollidableObject infiniteObject ,Vector3 position)
        {
            latestCollidableSpawnPosition = position;
            lastObjectSpawnDistance[(int)location][index] = position.x;
            objectTypeSpawnIndex[(int)location][(int)objectType] += 1;
            objectSpawnIndex[(int)location][index] = objectTypeSpawnIndex[(int)location][(int)objectType];
            latestObjectTypeSpawnDistance[(int)location][(int)objectType] = lastObjectSpawnDistance[(int)location][index];
            lastLocalIndex[(int)location][(int)objectType] = infiniteObjectManager.ObjectIndexToLocalIndex(index, objectType);

            CollidableObject prevTopCollidableObject = null;

            if (objectType == ObjectType.Scene)
            {
                prevTopCollidableObject = (CollidableObject) topCollidableSpawned[(int)location];
                topCollidableSpawned[(int)location] = infiniteObject;
            }
            return prevTopCollidableObject;
        }
        // the bottom infinite object only needs to be set for the very first object at the object location.. objectRemoved will otherwise take care of making sure the
        // bottom object is correct
        public void SetBottomInfiniteObject(ObjectLocation location, bool isSceneObject, InfiniteObject infiniteObject)
        {
            if (isSceneObject)
            {
                bottomSceneObjectSpawned[(int)location] = infiniteObject;
            }
        }

        public void ObjectRemoved(ObjectLocation location, bool isSceneObject)
        {
            if (isSceneObject)
            {
                bottomSceneObjectSpawned[(int)location] = bottomSceneObjectSpawned[(int)location].GetInfiniteObjectParent();
                if (bottomSceneObjectSpawned[(int)location] == null)
                {
                    topSceneObjectSpawned[(int)location] = null;
                }
            }
        }

        // Increase the distance travelled by the specified amount
        public void AddTotalDistance(float amount, ObjectLocation location, bool isSceneObject)
        {
            if (isSceneObject)
            {
                totalSceneDistance[(int)location] += amount;

                // truncate to prevent precision errors
                totalSceneDistance[(int)location] = ((int)(totalSceneDistance[(int)location] * 1000f)) / 1000f;
            //    Debug.Log("total scene distance" + totalSceneDistance[(int)location]);
                //// as time goes on totalDistance and totalSceneDistance become more separated because of the minor differences in sizes of the platforms/scenes.
                //// prevent the two distances from becoming too out of sync by setting the platform distance to the scene distance if the distance between
                //// the two is small
                //if (Mathf.Abs(totalSceneDistance[(int)location] - totalDistance[(int)location]) < 0.1f)
                //{
                //    totalSceneDistance[(int)location] = totalDistance[(int)location];
                //}
            }   
        }
        // returns the spawn index for the given object type
        public int GetObjectTypeSpawnIndex(ObjectType objectType)
        {
            return objectTypeSpawnIndex[(int)activeObjectLocation][(int)objectType];
        }

        // returns the spawn index for the given object index
        public int GetObjectSpawnIndex(int index)
        {
            return objectSpawnIndex[(int)activeObjectLocation][index];
        }

        // returns the local index for the given object type
        public int GetLastLocalIndex(ObjectType objectType)
        {
            return GetLastLocalIndex(activeObjectLocation, objectType);
        }

        // returns the local index for the given object type at the object location
        public int GetLastLocalIndex(ObjectLocation location, ObjectType objectType)
        {
            return lastLocalIndex[(int)location][(int)objectType];
        }

        // returns the spawn location (distance) for the given object index
        public float GetSpawnDistance(int index)
        {
            return lastObjectSpawnDistance[(int)activeObjectLocation][index];
        }

        // returns the distance of the last spawned object for the given object type
        public float GetLastObjectTypeSpawnDistance(ObjectType objectType)
        {
            return latestObjectTypeSpawnDistance[(int)activeObjectLocation][(int)objectType];
        }

        // returns the angle of location for a scene object or platform object
        public float GetObjectLocationAngle(ObjectLocation location)
        {
            return objectLocationAngle[(int)location];
        }

        // returns the total distance for a scene object or platform object
        public float GetTotalSceneDistance()
        {
            return totalSceneDistance[(int) activeObjectLocation];
        }

        // returns the top-most platform or scene object
        public InfiniteObject GetTopInfiniteObject(ObjectLocation location, bool isSceneObject)
        {
            return (isSceneObject ? topSceneObjectSpawned[(int)location] : null);
        }

        // returns the bottom-most platform or scene object
        public InfiniteObject GetBottomInfiniteObject(ObjectLocation location, bool isSceneObject)
        {
            return (isSceneObject ? bottomSceneObjectSpawned[(int)location]:null);
        }

        // set everything back to 0 for a new game
        public void SaveObjectsReset()
        {
                for (int i = 0; i < (int)ObjectLocation.Last; ++i)
                {

                    if (topSceneObjectSpawned[i] != null)
                        topSceneObjectSpawned[i].SetInfiniteObjectParent(savedInfiniteObjects);
                    if (bottomSceneObjectSpawned[i] != null)
                        bottomSceneObjectSpawned[i].SetInfiniteObjectParent(savedInfiniteObjects);
                }

            activeObjectLocation = ObjectLocation.Center;
            for (int i = 0; i < (int)ObjectLocation.Last; ++i)
            {
                totalDistance[i] = 0;
                totalSceneDistance[i] = 0;
                objectLocationAngle[i] = 0;

                topSceneObjectSpawned[i] = (SceneObject) (bottomSceneObjectSpawned[i] = null);

                previousSceneSection[i] = 0;
                spawnedSceneSectionTransition[i] = false;

                for (int j = 0; j < objectSpawnIndex[i].Count; ++j)
                {
                    objectSpawnIndex[i][j] = -1;
                    lastObjectSpawnDistance[i][j] = 0;
                }
                for (int j = 0; j < (int)ObjectType.Last; ++j)
                {
                    objectTypeSpawnIndex[i][j] = -1;
                    lastLocalIndex[i][j] = -1;
                    latestObjectTypeSpawnDistance[i][j] = -1;
                }
            }

        }

        public InfiniteObject GetSavedInfiniteObjects()
        {
            return savedInfiniteObjects;
        }

        public void SetPreviousSection(ObjectLocation location, bool isSceneObject, int section)
        {
            if (isSceneObject)
            {
                previousSceneSection[(int)location] = section;
                spawnedSceneSectionTransition[(int)location] = false;
            }
        }

        public int GetPreviousSection(ObjectLocation location, bool isSceneObject)
        {
            return (isSceneObject ? previousSceneSection[(int)location] : -1);
        }

        public void DidSpawnSectionTranition(ObjectLocation location, bool isSceneObject)
        {
            if (isSceneObject)
            {
                spawnedSceneSectionTransition[(int)location] = true;
            }
        }

        public bool HasSpawnedSectionTransition(ObjectLocation location, bool isSceneObject)
        {
            return (isSceneObject ? spawnedSceneSectionTransition[(int)location] : false);
        }

        // For persisting the data:
        public void SaveInfiniteObjectPersistence(ref InfiniteObjectPersistence persistence)
        {
            persistence.totalDistance = totalDistance;
            persistence.totalSceneDistance = totalSceneDistance;
            persistence.objectLocationAngle = objectLocationAngle;
            persistence.topSceneObjectSpawned = topSceneObjectSpawned;
            persistence.bottomSceneObjectSpawned = bottomSceneObjectSpawned;
            persistence.previousSceneSection = previousSceneSection;
            persistence.spawnedSceneSectionTransition = spawnedSceneSectionTransition;

            int objectCount = objectSpawnIndex[0].Count;
            persistence.objectSpawnIndex = new int[(int)ObjectLocation.Last * objectCount];
            persistence.lastObjectSpawnDistance = new float[(int)ObjectLocation.Last * objectCount];
            persistence.objectTypeSpawnIndex = new int[(int)ObjectLocation.Last * (int)ObjectType.Last];
            persistence.lastLocalIndex = new int[(int)ObjectLocation.Last * (int)ObjectType.Last];
            persistence.latestObjectTypeSpawnDistance = new float[(int)ObjectLocation.Last * (int)ObjectType.Last];

            int width = (int)ObjectLocation.Last;
            for (int i = 0; i < (int)ObjectLocation.Last; ++i)
            {
                for (int j = 0; j < objectCount; ++j)
                {
                    persistence.objectSpawnIndex[i * width + j] = objectSpawnIndex[i][j];
                    persistence.lastObjectSpawnDistance[i * width + j] = lastObjectSpawnDistance[i][j];
                }
                for (int j = 0; j < (int)ObjectType.Last; ++j)
                {
                    persistence.objectTypeSpawnIndex[i * width + j] = objectTypeSpawnIndex[i][j];
                    persistence.lastLocalIndex[i * width + j] = lastLocalIndex[i][j];
                    persistence.latestObjectTypeSpawnDistance[i * width + j] = latestObjectTypeSpawnDistance[i][j];
                }
            }
        }

        public void LoadInfiniteObjectPersistence(InfiniteObjectPersistence persistence)
        {
            totalDistance = persistence.totalDistance;
            totalSceneDistance = persistence.totalSceneDistance;
            objectLocationAngle = persistence.objectLocationAngle;

            topSceneObjectSpawned = (SceneObject[]) persistence.topSceneObjectSpawned;
            bottomSceneObjectSpawned = persistence.bottomSceneObjectSpawned;
            previousSceneSection = persistence.previousSceneSection;
            spawnedSceneSectionTransition = persistence.spawnedSceneSectionTransition;

            int objectCount = objectSpawnIndex[0].Count;
            int width = (int)ObjectLocation.Last;
            for (int i = 0; i < (int)ObjectLocation.Last; ++i)
            {
                for (int j = 0; j < objectCount; ++j)
                {
                    objectSpawnIndex[i][j] = persistence.objectSpawnIndex[i * width + j];
                    lastObjectSpawnDistance[i][j] = persistence.lastObjectSpawnDistance[i * width + j];
                }
                for (int j = 0; j < (int)ObjectType.Last; ++j)
                {
                    objectTypeSpawnIndex[i][j] = persistence.objectTypeSpawnIndex[i * width + j];
                    lastLocalIndex[i][j] = persistence.lastLocalIndex[i * width + j];
                    latestObjectTypeSpawnDistance[i][j] = persistence.latestObjectTypeSpawnDistance[i * width + j];
                }
            }
        }
    }

    /**
     * Maps the platform distance/section to a local platform index. Used by the scenes and sections to be able to determine which platform they are spawning near
     */
    [System.Serializable]
    public class PlatformDistanceDataMap
    {
        public List<float> distances;
        public List<int> localIndexes;
        public List<int> sections;

        public PlatformDistanceDataMap()
        {
            distances = new List<float>();
            localIndexes = new List<int>();
            sections = new List<int>();
        }

        // a new platform has been spawned, add the distance and section
        public void AddIndex(float distance, int index, int section)
        {
            distances.Add(distance);
            localIndexes.Add(index);
            sections.Add(section);
        }

        // remove the reference if the scene distance is greater than the earliest platform distance
        public void CheckForRemoval(float distance)
        {
            if (distances.Count > 0)
            {
                // add 0.1f to prevent rounding errors
                if (distances[0] <= distance + 0.1f)
                {
                    distances.RemoveAt(0);
                    localIndexes.RemoveAt(0);
                    sections.RemoveAt(0);
                }
            }
        }

        // returns the first platform index who doesnt have a scene spawned near it
        public int FirstIndex()
        {
            if (localIndexes.Count > 0)
            {
                return localIndexes[0];
            }
            return -1;
        }

        public int FirstSection()
        {
            if (sections.Count > 0)
            {
                return sections[0];
            }
            return -1;
        }

        public void ResetValues()
        {
            distances.Clear();
            localIndexes.Clear();
            sections.Clear();
        }

        public void CopyFrom(PlatformDistanceDataMap other)
        {
            distances = other.distances.GetRange(0, other.distances.Count);
            localIndexes = other.localIndexes.GetRange(0, other.localIndexes.Count);
            sections = other.sections.GetRange(0, other.sections.Count);
        }
    }
}