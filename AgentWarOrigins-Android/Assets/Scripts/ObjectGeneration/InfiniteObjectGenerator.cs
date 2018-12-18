using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessRunner
{
    public enum ObjectLocation
    {
        Center,
        Left,
        Right,
        Last
    }

    /*
     * The InfiniteObjectGenerator is the controlling class of when different objects spawn.
     */

    public class InfiniteObjectGenerator : MonoBehaviour
    {
        public static InfiniteObjectGenerator instance;

        // How far out in the distance objects spawn (squared)
        public float sqrHorizon = 38000;
        // The distance behind the camera that the objects will be removed and added back to the object pool
        public float removeHorizon = -150;
        // the number of units between the slots in the track
        public float slotDistance = 25f;
        // Spawn the full length of objects, useful when creating a tutorial or startup objects
        public bool spawnFullLength;
        // Do we want to reposition on height changes?
        public bool heightReposition;
        // The amount of distance to move back when the player is reviving
        public float reviveMoveBackDistance = -20;

        public float SpawnSceneTime;
        public float objectDeactivationTime;
        public float SpawnCollidableTime;
        public Vector3 DistanceBetweenCollidables;

        public Vector3 ObstacleOffset = new Vector3(10, 0, 0);
        // the probability that no collidables will spawn on the platform
        [HideInInspector] public DistanceValueList noCollidableProbability;

        private Vector3 moveDirection;
        private Vector3 spawnDirection;
        private float wrongMoveDistance;


        private Vector3[] sceneSizes;
        private float largestSceneLength;
        private Vector3[] sceneStartPosition;
        private SectionSelection sectionSelection;


        private bool stopObjectSpawns;
        private ObjectSpawnData spawnData;

        private Transform playerTransform;
        private InfiniteObjectManager infiniteObjectManager;
        private InfiniteObjectHistory infiniteObjectHistory;
        private float currentTotalSceneLength;
        private Vector3 currentSceneSpawnPosition;
        private InfiniteObject prevScene;
        private GameObject infiniteObjectsParent;
        private GameObject obstacleObjectparent;
        private static bool isSessionStarted;
        private float spawnSceneUpdateTimer;
        private float objectDeactivationTimer;
        private float SpawnCollidableUpdateTimer;

        private float currentCollidableObjects;
        private Vector3 prevObstaclePosition;
        private Vector3 prevTokenPosition;
        private GameManager gameManager;

        private Queue<SceneObject> m_SceneObjectsQueue;
        private List<SceneObject> m_SceneObjectsList;
        private Vector3 DistanceBetweenSpawns = new Vector3(100, 0, 0);
        public void Awake()
        {
            instance = this;
            spawnDirection = Vector3.right;
            infiniteObjectsParent = GameObject.FindGameObjectWithTag("InfiniteObjectsParent");
            obstacleObjectparent = GameObject.FindGameObjectWithTag("ObstaclesParent");
            spawnSceneUpdateTimer = SpawnSceneTime;
            objectDeactivationTimer = objectDeactivationTime;

            m_SceneObjectsQueue = new Queue<SceneObject>();
            m_SceneObjectsList = new List<SceneObject>();
        }

        public void Start()
        {
            //get gamemanager Instance
            gameManager = GameManager.Instance;

            infiniteObjectManager = InfiniteObjectManager.instance;
            infiniteObjectHistory = InfiniteObjectHistory.instance;
            infiniteObjectManager.Init();
            if (InfiniteObjectHistory.instance != null)
                InfiniteObjectHistory.instance.Init(infiniteObjectManager.GetTotalObjectCount());

            sectionSelection = SectionSelection.instance;


            //infiniteObjectManager.GetObjectSizes(out sceneSizes, out largestSceneLength);
            //for (int i = 0; i < sceneSizes.Length; i++)
            //{
            //    Debug.Log("scene sizes[i]" + sceneSizes[i]);

            //}
            infiniteObjectManager.GetObjectStartPositions(out sceneStartPosition);

            stopObjectSpawns = false;
            spawnData = new ObjectSpawnData();
            spawnData.largestScene = largestSceneLength;
            spawnData.useWidthBuffer = true;

            noCollidableProbability.Init();

            //get previous collidable position
          //  prevObstaclePosition = new Vector3(430,0,0);
            prevTokenPosition = new Vector3(400, 0, 0);
            //  ShowStartupObjects(GameManager.Instance.showTutorial);

            //   SpawnObjectRun(true);
            //  InvokeRepeating("CheckForDeactivation", 0.01f, 5f);
            //    GameManager.Instance.OnStartGame += StartGame;

          //  InvokeRepeating("SpawnCollidable", 10, 5);
        }

        //private void SpawnCollidable()
        //{
        //    int random = Random.Range(1, 5);
        //    if(Vector3.Distance(prevTokenPosition,GameManager.m_InstantiatedPlayer.transform.position) < 500)
        //    {
        //       // Debug.Log("current token spawn pos < 500");
        //        for (int j = 0; j < random; j++)
        //        {

        //            SpawnCollidable(ObjectType.Token, prevTokenPosition, Vector3.right * 5, true, false);
        //        }

        //        prevTokenPosition += DistanceBetweenSpawns;
        //    }


        //}

        private void FixedUpdate()
        {
            spawnSceneUpdateTimer -= Time.fixedDeltaTime;
            objectDeactivationTimer -= Time.fixedDeltaTime;
            SpawnCollidableUpdateTimer -= Time.fixedDeltaTime;
            if (gameManager == null)
            {
                gameManager = GameManager.Instance;
            }
           
            if (spawnSceneUpdateTimer < 0 && GameManager.m_InstantiatedPlayer)
            {
                float sceneDistanceToPlayer = Vector3.Distance(currentSceneSpawnPosition, GameManager.m_InstantiatedPlayer.transform.position);

                if (gameManager)
                {
                    if (gameManager.IsGameActive() && sceneDistanceToPlayer < 750)
                    {
                        SpawnObjectRun(false);

                        spawnSceneUpdateTimer = SpawnSceneTime;
                    }
                }

            }

            if (SpawnCollidableUpdateTimer < 0 && GameManager.m_InstantiatedPlayer)
            {
                prevTokenPosition = new Vector3(GameManager.m_InstantiatedPlayer.transform.position.x,0,0) + DistanceBetweenSpawns;
                int random = Random.Range(1, 5);
                for(int i = 0; i < random; i++)
                {
                    SpawnCollidable(ObjectType.Token, prevTokenPosition, Vector3.right * 5, true, false);

                }
                SpawnCollidableUpdateTimer = SpawnCollidableTime;
            }



            ////time to spawn again
            //if (objectDeactivationTimer < 0)
            //{
            //    CheckForDeactivation();
            //    objectDeactivationTimer = objectDeactivationTime;
            //}
        }

        // creates any startup objects, returns null if no prefabs are assigned
        public bool ShowStartupObjects(bool tutorial)
        {
            var startupObjects = infiniteObjectManager.CreateStartupObjects(tutorial);
            if (startupObjects == null)
                return false;

            Transform objectTypeParent;
            Transform topObject;
            Transform transformParent;
            InfiniteObject parentInfiniteObject;
            bool isSceneObject;
            for (var i = 0; i < 2; ++i)
            {
                isSceneObject = i == 0;
                objectTypeParent = startupObjects.transform.Find(isSceneObject ? "Scene" : "Platforms");
                // iterate high to low because assignParent is going to set a new parent thus changing the number of children in startup objects
                for (var j = objectTypeParent.childCount - 1; j >= 0; --j)
                {
                    topObject = objectTypeParent.GetChild(j);
                    infiniteObjectManager.AssignParent(topObject.GetComponent<InfiniteObject>(), ObjectType.Scene);

                    InfiniteObject[] childObjects;
                    if (isSceneObject)
                        childObjects = topObject.GetComponentsInChildren<SceneObject>();
                    else childObjects = null;
                    for (var k = 0; k < childObjects.Length; ++k)
                    {
                        childObjects[k].EnableDestroyOnDeactivation();
                        transformParent = childObjects[k].GetTransform().parent;
                        if ((parentInfiniteObject = transformParent.GetComponent<InfiniteObject>()) != null)
                            childObjects[k].SetInfiniteObjectParent(parentInfiniteObject);
                    }
                }
            }

            // Get the persistent objects
            var persistence = startupObjects.GetComponent<InfiniteObjectPersistence>();
            infiniteObjectHistory.LoadInfiniteObjectPersistence(persistence);

            // All of the important objects have been taken out, destroy the game object
            Destroy(startupObjects.gameObject);

            return true;
        }

        //private void StartGame()
        //{
        //    playerController = PlayerController.Instance;
        //    playerTransform = playerController.transform;
        //}

        // An object run contains many platforms strung together with collidables: obstacles, power ups, and coins. If spawnObjectRun encounters a turn,
        // it will spawn the objects in the correct direction
        public void SpawnObjectRun(bool activateImmediately)
        {
            // spawn the center objects
            prevScene = infiniteObjectHistory.GetTopInfiniteObject(ObjectLocation.Center, true);
            //    Debug.Log("total scene spawned" + currentTotalSceneLength);
            var position = Vector3.zero;
            if (prevScene == null)
            {
                // currentTotalSceneLength = WarehouseMesh.GetComponent<BoxCollider>().bounds.size.x;

                position.x += 429.22f;
                currentSceneSpawnPosition += position;
            }
            else
            {
                currentTotalSceneLength += prevScene.GetCurrentSceneLength();
                var prevSceneIndex = infiniteObjectHistory.GetLastLocalIndex(ObjectLocation.Center, ObjectType.Scene);
                position.x += prevScene.GetCurrentSceneLength();
                currentSceneSpawnPosition += position;
            }
            var scene = SpawnObjects(ObjectLocation.Center, currentSceneSpawnPosition, spawnDirection,
                activateImmediately);

            if (scene == null)
                return;

            prevScene = (SceneObject) infiniteObjectHistory.GetTopInfiniteObject(ObjectLocation.Center, true);
            //if (currentTotalSceneLength < Camera.main.farClipPlane)
            //{
            //    SpawnObjectRun(false);

            //}
            //else
            //{
            //    StartCoroutine(ResetTotalLengthSpawned());

            //}
        }

        private IEnumerator ResetTotalLengthSpawned()
        {
            yield return new WaitForSeconds(30f);
            currentTotalSceneLength = 0;
        }

        private void CheckForDeactivation()
        {
            // InfiniteObject infiniteObject = null;
            // var activeInfiniteObjects = FindObjectsWithTag(infiniteObjectsParent, "Ground");
            //if (activeInfiniteObjects != null)
            //    foreach (var activeGameObject in activeInfiniteObjects)
            //    {
            //        if (activeGameObject != null)
            //            if ((activeGameObject.transform.position - playerControl.GetPlayerCurrentPosition()).x <
            //                removeHorizon)
            //                activeGameObject.Deactivate();
            //            else if ((activeGameObject.transform.position - playerControl.GetPlayerCurrentPosition()).x <
            //                     Camera.main.farClipPlane &&
            //                     activeGameObject.gameObject.layer != LayerMask.NameToLayer("Obstacle"))
            //                activeGameObject.Activate();
            //    }


            //var activeObstacleObjects = FindObjectsWithTag(obstacleObjectparent, "Obstacle");
            //if (activeObstacleObjects != null)
            //    foreach (var activeGameObject in activeObstacleObjects)
            //    {
            //        if (activeGameObject != null)
            //            if ((activeGameObject.transform.position - playerControl.GetPlayerCurrentPosition()).x <
            //                removeHorizon)
            //                activeGameObject.Deactivate();
            //            else if ((activeGameObject.transform.position - playerControl.GetPlayerCurrentPosition()).x <
            //                     Camera.main.farClipPlane &&
            //                     activeGameObject.gameObject.layer != LayerMask.NameToLayer("Obstacle"))
            //                activeGameObject.Activate();
            //    }

            //foreach (Transform child in transform)
            //{
            //    if ((child.transform.position - playerControl.GetPlayerCurrentPosition()).x < removeHorizon)
            //    {
            //        child.gameObject.SetActive(false);
            //    }
            //    else if ((child.transform.position - playerControl.GetPlayerCurrentPosition()).x <
            //             Camera.main.farClipPlane &&
            //             child.gameObject.layer != LayerMask.NameToLayer("Obstacle"))
            //    {
            //        child.gameObject.SetActive(true);

            //    }
            //}
            var player = GameObject.FindGameObjectWithTag("Player");
            if (GameManager.Instance.IsGameActive())
                if (m_SceneObjectsQueue.Count > 0)
                {
                    var bottomSceneObject = m_SceneObjectsQueue.Peek();
                    if (!bottomSceneObject) return;
                    //Debug.Log("bottom scene position" + bottomSceneObject.transform.position);
                    //Debug.Log("player current position " + playerControl.GetPlayerCurrentPosition());
                    if (
                        !((bottomSceneObject.transform.position -
                           player.GetComponent<PlayerControl>().GetPlayerCurrentPosition()).x <
                          removeHorizon)) return;
                    //Debug.Log("object if out of origin");
                    bottomSceneObject = m_SceneObjectsQueue.Dequeue();
                    bottomSceneObject.Deactivate();
                    //else if ((bottomSceneObject.transform.position - playerControl.GetPlayerCurrentPosition()).x <
                    //         Camera.main.farClipPlane &&
                    //         bottomSceneObject.gameObject.layer != LayerMask.NameToLayer("Obstacle"))
                    //{
                    //    Debug.Log("object if out of origin else");

                    //    m_SceneObjectsQueue.Dequeue();
                    //    bottomSceneObject.Activate();

                    //}
                }


            //if (m_SceneObjectsList.Count > 0)
            //{
            //    SceneObject firstSceneObject = m_SceneObjectsList[0];
            //    if (!firstSceneObject) return;
            //    if (!((firstSceneObject.transform.position - playerControl.GetPlayerCurrentPosition()).x <
            //          removeHorizon)) return;
            //    //Debug.Log("object if out of origin");
            //    m_SceneObjectsList.Remove(firstSceneObject);
            //    firstSceneObject.Deactivate();

            //}
        }

        public void DisableChildren()
        {
        }

        //private void CheckForObjectSpawn()
        //{
        //    if (currentTotalSceneLength == 0f)
        //        SpawnObjectRun(false);
        //}

        public static List<InfiniteObject> FindObjectsWithTag(GameObject parent, string tag)
        {
            var temp = new List<InfiniteObject>();
            var trs = parent.GetComponentsInChildren<Transform>(true);
            foreach (var t in trs)
                if (t.CompareTag(tag))
                    temp.Add(t.gameObject.GetComponent<InfiniteObject>());
            return temp;
        }

        public static List<InfiniteObject> FindObjectsWithTag(GameObject parent)
        {
            var temp = new List<InfiniteObject>();
            var trs = parent.GetComponentsInChildren<Transform>(true);
            foreach (var t in trs)
                temp.Add(t.gameObject.GetComponent<InfiniteObject>());
            return temp;
        }

        // before platforms are about to be spawned setup the section data to ensure the correct platforms are spawned
        private void SetupSection(ObjectLocation location, bool isSceneObject)
        {
            var prevSection = infiniteObjectHistory.GetPreviousSection(location, isSceneObject);
            spawnData.section = sectionSelection.GetSection(infiniteObjectHistory.GetTotalSceneDistance());
            if (sectionSelection.useSectionTransitions)
                if (spawnData.section != prevSection &&
                    !infiniteObjectHistory.HasSpawnedSectionTransition(location, isSceneObject))
                {
                    spawnData.sectionTransition = true;
                    spawnData.prevSection = prevSection;
                }
                else
                {
                    spawnData.sectionTransition = false;
                    if (spawnData.section != prevSection &&
                        infiniteObjectHistory.HasSpawnedSectionTransition(location, isSceneObject))
                        infiniteObjectHistory.SetPreviousSection(location, isSceneObject, spawnData.section);
                }
        }

        // spawn the platforms, obstacles, power ups, and coins
        private SceneObject SpawnObjects(ObjectLocation location, Vector3 position, Vector3 direction,
            bool activateImmediately)
        {
            SetupSection(location, true);

            var localIndex = infiniteObjectManager.GetNextObjectIndex(ObjectType.Scene, spawnData);
            if (localIndex == -1)
            {
                print("Unable to spawn scene. No scenes can be spawned based on the probability rules at distance " +
                      infiniteObjectHistory.GetTotalSceneDistance());
                return null;
            }
            var scene = SpawnScene(localIndex, location, position, direction, activateImmediately);


            return scene;
        }

        private void SpawnCollidables(ObjectType objectType, Vector3 position, bool activateImmediately)
        {
                SpawnCollidable(objectType, position, DistanceBetweenCollidables,activateImmediately,false);
        }

        // returns the length of the created platform
        private SceneObject SpawnScene(int localIndex, ObjectLocation location, Vector3 position, Vector3 direction,
            bool activateImmediately)
        {
            var scene
                = (SceneObject) infiniteObjectManager.ObjectFromPool(localIndex, ObjectType.Scene);
            if (scene)
            {
                scene.SceneLocalIndex = localIndex;
                //  EnableStaticBatching.Instance.StaticCombineSceneObjects();
            //    var lookRotation = Quaternion.LookRotation(direction);

                var offset = new Vector3(-0.3f, scene.transform.position.y, scene.transform.position.z);
                //offset to remove lines between scenes
                currentSceneSpawnPosition += offset;
                scene.setInfiniteObjectPosition(currentSceneSpawnPosition);

                //reset y and z position of currentscene spawn position
                currentSceneSpawnPosition.y = 0;
                currentSceneSpawnPosition.z = 0;


                //
                //  PlayerControl.Instance.SetPlayerPosition(scene);
                if (activateImmediately)
                    scene.Activate();

                var objectIndex = infiniteObjectManager.LocalIndexToObjectIndex(localIndex, ObjectType.Scene);
                var prevTopScene = infiniteObjectHistory.SceneObjectSpawned(objectIndex, location,
                    ObjectType.Scene, scene);
                // the current platform now becames the parent of the previous top platform
                //if (prevTopScene != null)
                //{
                //    prevTopScene.SetInfiniteObjectParent(scene);
                //}
                //else
                //{
                //    infiniteObjectHistory.SetBottomInfiniteObject(location, true, scene);
                //}

                if (prevTopScene != null)
                {
                    //add latest scene to queue
                    m_SceneObjectsQueue.Enqueue(scene);
                    //    m_SceneObjectsList.Add(scene);
                    infiniteObjectHistory.AddTotalDistance(prevTopScene.GetCurrentSceneLength(), location, true);
                }

                //spawn collidables in entire scene
                int currentSceneLength = (int)scene.GetCurrentSceneLength();
                int noOfCollidables = currentSceneLength / (int)DistanceBetweenCollidables.x;
                scene.CollidablePositions = noOfCollidables;
                spawnData.slotPositions = scene.GetSlotsAvailable();              
                prevObstaclePosition = scene.transform.position + ObstacleOffset;
                for (int i = 0; i < noOfCollidables; i++)
                {
                    SpawnCollidables(ObjectType.Obstacle, prevObstaclePosition, true);
                }
                //while (currentCollidablesSpawned < 10)
                //{
                //    int random = Random.Range(1, 5);
                //    for (int j = 0; j < random; j++)
                //    {
                //        SpawnCollidable(ObjectType.Token, prevTokenPosition, Vector3.right * 5, true, false);
                //        currentCollidablesSpawned++;
                //    }
                //    prevTokenPosition += new Vector3(100, 0, 0);
                //}

                //SpawnCollidableUpdateTimer = SpawnCollidableTime;



            }
    
         //   Debug.Log("total distance spawned till now " + scene.name + "is" + infiniteObjectHistory.GetTotalSceneDistance());

            return scene;
        }

        // returns true if there is still space on the platform for a collidable object to spawn
        private CollidableObject SpawnCollidable(ObjectType objectType, Vector3 position,Vector3 distanceBetweenCollidables,
            bool activateImmediately)
        {
            //var collidablePositions = (int) noOfCollidableObjectsAtOnce;
            CollidableObject collidable = null;
            //// can't do anything if the scene doesn't accept any collidable object spawns
            //if (collidablePositions == 0)
            //    return;

            //    Debug.Log("offset value" + offset);
            //      float zDelta = sceneSizes[sceneLocalIndex].z * .8f / (1 + collidablePositions);

            Vector3 collidableSpawnPosition = position + distanceBetweenCollidables;
            var localIndex = infiniteObjectManager.GetNextObjectIndex(objectType, spawnData, collidableSpawnPosition.x);

            if (localIndex != -1)
            {
                collidable = infiniteObjectManager.ObjectFromPool(localIndex, objectType) as CollidableObject;

                if (collidable != null)
                {
                    var spawnSlot = collidable.GetSpawnSlot(Vector3.forward * slotDistance,spawnData.slotPositions);
                    //Debug.Log(spawnSlot);
                    collidableSpawnPosition = position + distanceBetweenCollidables + new Vector3(0,collidable.transform.position.y,0) + 
                                              //new Vector3(0,
                                              //    collidable.GetComponent<Renderer>().bounds.size.y / 2, 0) +
                                              spawnSlot;

                    if (collidable.CanSpwanCollidableAtPosition(collidableSpawnPosition))
                    {
                        collidable.setInfiniteObjectPosition(collidableSpawnPosition);

                       
                        if (activateImmediately)
                            collidable.Activate();

                        if (objectType == ObjectType.Obstacle)
                        {
                            if (collidable.RandomRotation)
                            {
                                collidable.SetInfiniteObjectRandomRotation();
                            }
                        }

                    }

                }
                //collidable.setInfiniteObjectPosition(collidableSpawnPosition);


                //if (activateImmediately)
                //    collidable.Activate();

                //  CheckForCollidableObjectInRange();
                //collidable.setInfiniteObjectPosition((position + (offset.z + ((i + 1) * zDelta)) * direction + spawnSlot));
                //  collidable.Orient(position + (offset.z + ((i + 1) * zDelta)) * direction + spawnSlot, lookRotation);

                var objectIndex = infiniteObjectManager.LocalIndexToObjectIndex(localIndex, objectType);
                //          scene.CollidableSpawned(i);

                // don't allow any more of the same collidable type if we are forcing a different collidable
                //     if (scene.forceDifferentCollidableTypes)
                //          break;
                var prevCollidableSpawned = infiniteObjectHistory.CollidableObjectSpawned(objectIndex,
                    ObjectLocation.Center, objectType, collidable, collidableSpawnPosition);
                //     ObjectType.Obstacle, collidable);
                //          infiniteObjectHistory.AddTotalDistance(collidableSpawnPosition.x, ObjectLocation.Center, false);
            }
            else
            {
              //  Debug.Log("no collidable spaened");
            }
            if (objectType == ObjectType.Obstacle)
            {
                prevObstaclePosition = new Vector3(collidableSpawnPosition.x, 0, 0);

            }
            else if (objectType == ObjectType.Token)
            {
                prevTokenPosition = new Vector3(collidableSpawnPosition.x, 0, 0);
            }

            return collidable;
        }


        private CollidableObject SpawnCollidable(ObjectType objectType, Vector3 position, Vector3 distanceBetweenCollidables,
    bool activateImmediately,bool canInstantiate)
        {
            //var collidablePositions = (int) noOfCollidableObjectsAtOnce;
            CollidableObject collidable = null;
            //// can't do anything if the scene doesn't accept any collidable object spawns
            //if (collidablePositions == 0)
            //    return;

            //    Debug.Log("offset value" + offset);
            //      float zDelta = sceneSizes[sceneLocalIndex].z * .8f / (1 + collidablePositions);

            Vector3 collidableSpawnPosition = position + distanceBetweenCollidables;
            var localIndex = infiniteObjectManager.GetNextObjectIndex(objectType, spawnData, collidableSpawnPosition.x);

            if (localIndex != -1)
            {
                collidable = infiniteObjectManager.ObjectFromPool(localIndex, objectType, canInstantiate) as CollidableObject;

                if (collidable != null)
                {
                    var spawnSlot = collidable.GetSpawnSlot(Vector3.forward * slotDistance, spawnData.slotPositions);
                    //Debug.Log(spawnSlot);
                    collidableSpawnPosition = position + distanceBetweenCollidables + new Vector3(0, collidable.transform.position.y, 0) +
                                              //new Vector3(0,
                                              //    collidable.GetComponent<Renderer>().bounds.size.y / 2, 0) +
                                              spawnSlot;

                    if (collidable.CanSpwanCollidableAtPosition(collidableSpawnPosition))
                    {
                        collidable.setInfiniteObjectPosition(collidableSpawnPosition);


                        if (activateImmediately)
                            collidable.Activate();

                        if (objectType == ObjectType.Obstacle)
                        {
                            if (collidable.RandomRotation)
                            {
                                collidable.SetInfiniteObjectRandomRotation();
                            }
                        }

                    }

                }
                //collidable.setInfiniteObjectPosition(collidableSpawnPosition);


                //if (activateImmediately)
                //    collidable.Activate();

                //  CheckForCollidableObjectInRange();
                //collidable.setInfiniteObjectPosition((position + (offset.z + ((i + 1) * zDelta)) * direction + spawnSlot));
                //  collidable.Orient(position + (offset.z + ((i + 1) * zDelta)) * direction + spawnSlot, lookRotation);

                var objectIndex = infiniteObjectManager.LocalIndexToObjectIndex(localIndex, objectType);
                //          scene.CollidableSpawned(i);

                // don't allow any more of the same collidable type if we are forcing a different collidable
                //     if (scene.forceDifferentCollidableTypes)
                //          break;
                var prevCollidableSpawned = infiniteObjectHistory.CollidableObjectSpawned(objectIndex,
                    ObjectLocation.Center, objectType, collidable, collidableSpawnPosition);
                //     ObjectType.Obstacle, collidable);
                //          infiniteObjectHistory.AddTotalDistance(collidableSpawnPosition.x, ObjectLocation.Center, false);
            }
            else
            {
                //  Debug.Log("no collidable spaened");
            }
            if (objectType == ObjectType.Obstacle)
            {
                prevObstaclePosition = new Vector3(collidableSpawnPosition.x, 0, 0);

            }
            else if (objectType == ObjectType.Token)
            {
                prevTokenPosition = new Vector3(collidableSpawnPosition.x, 0, 0);
            }

            return collidable;
        }

        private bool CheckForCollidableObjectInRange(Vector3 position)
        {
            return false;
        }

        // gradually turn the player for a curve
        public void SetMoveDirection(Vector3 newDirection)
        {
            moveDirection = newDirection;
        }

        public Vector3 GetMoveDirection()
        {
            return moveDirection;
        }

        // clear everything out and reset the generator back to the beginning, keeping the current set of objects activated before new objects are generated
        public void ResetValues()
        {
            moveDirection = Vector3.right;
            spawnDirection = Vector3.right;
            wrongMoveDistance = 0;
            stopObjectSpawns = false;
            spawnData.largestScene = largestSceneLength;
            spawnData.useWidthBuffer = true;

            infiniteObjectHistory.SaveObjectsReset();
        }

        // remove the saved infinite objects and activate the set of objects for the next game
        public void ReadyFromReset()
        {
            // deactivate the saved infinite objects from the previous game
            var infiniteObject = infiniteObjectHistory.GetSavedInfiniteObjects();
            InfiniteObject[] childObjects = null;
            for (var i = 0; i < 2; ++i)
            {
                // loop through the platform and scenes
                if (i == 0)
                    childObjects = infiniteObject.GetComponentsInChildren<SceneObject>(true);

                for (var j = 0; j < childObjects.Length; ++j)
                    childObjects[j].Deactivate();
            }

            // activate the objects for the current game
            for (var i = 0; i < 2; ++i)
            for (var j = 0; j < (int) ObjectLocation.Last; ++j)
            {
                infiniteObject = infiniteObjectHistory.GetTopInfiniteObject((ObjectLocation) j, i == 0);
                if (infiniteObject != null)
                {
                    if (i == 0)
                        childObjects = infiniteObject.GetComponentsInChildren<SceneObject>(true);
                    for (var k = 0; k < childObjects.Length; ++k)
                        childObjects[k].Activate();
                }
            }
        }
    }
}