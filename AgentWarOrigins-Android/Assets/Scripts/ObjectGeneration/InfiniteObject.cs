using UnityEngine;
using System.Collections.Generic;

namespace EndlessRunner
{
    /*
     * Place the List<int> of sections into a HashSet to make lookup really fast (O(1))
     */
    public class SectionList
    {
        public HashSet<int> sections;

        public SectionList(List<int> sectionArray)
        {
            sections = new HashSet<int>();
            for (int i = 0; i < sectionArray.Count; ++i)
            {
                sections.Add(sectionArray[i]);
            }
        }

        public bool ContainsSection(int section)
        {
            return sections.Contains(section);
        }

        public int Count()
        {
            return sections.Count;
        }
    }

    /*
     * Base class for any object created by the infinite object manager. These objects are used and reset multiple times after
     * being pushed and popped from the object pool. This is done to instantiate as little as possible
     */
    public enum ObjectType {Scene, Obstacle, Token, Last }
    public abstract class InfiniteObject : MonoBehaviour
    {
        [HideInInspector]
        public List<int> sections;
        private SectionList sectionList;

        protected ObjectType objectType;

        private Vector3 startPosition;
        private Quaternion startRotation;
        private Transform startParent;

        private int localIndex;
        private InfiniteObject infiniteObjectParent;
        protected Vector3 currentObjectSize;
        protected float currentObjectLength;
        // if enabled, only the object renderer and collider will be activated/deactivated
        public bool optimizeDeactivation;
        protected Renderer[] childRenderers;
        protected IOClod[] childIoClod;
        protected Renderer renderers;
        protected Collider[] childColliders;
        protected List<Collider> GameObjectsInCollisionLayer;
        protected List<Collider> GameObjectsInTriggerLayer;
        private bool destroyOnDeactivation;

        protected GameObject thisGameObject;
        protected Transform thisTransform;
        public virtual void Init()
        {
            
            sectionList = new SectionList(sections);
            startPosition = transform.position;
            startRotation = transform.rotation;
        }

        public virtual void Awake()
        {
            thisGameObject = gameObject;
            thisTransform = transform;


            renderers = GetComponent<Renderer>();
            childRenderers = GetComponentsInChildren<Renderer>();
            childColliders = GetComponentsInChildren<Collider>();
            startPosition = transform.position;
            startRotation = transform.rotation;

            destroyOnDeactivation = false;

        }

        public virtual void Start()
        {
            
        }
        public ObjectType GetObjectType()
        {
            return objectType;
        }

        public Transform GetTransform()
        {
            return thisTransform;
        }

        public void SetLocalIndex(int index)
        {
            localIndex = index;
        }

        public int GetLocalIndex()
        {
            return localIndex;
        }

        public Vector3 GetStartPosition()
        {
            return startPosition;
        }

        public virtual void SetParent(Transform parent)
        {

            thisTransform.parent = parent;
            SetStartParent(parent);
        }

        public void SetStartParent(Transform parent)
        {
            startParent = parent;
        }

        public void SetInfiniteObjectParent(InfiniteObject parentObject)
        {
            infiniteObjectParent = parentObject;
            thisTransform.parent = parentObject.GetTransform();
        }

        public InfiniteObject GetInfiniteObjectParent()
        {
            return infiniteObjectParent;
        }

        //// orient for platform and scene objects
        //public virtual void Orient(Vector3 position, Quaternion rotation)
        //{
        //    Vector3 pos = Vector3.zero;
        //    float yAngle = rotation.eulerAngles.y;
        //    //pos.Set(startPosition.x * Mathf.Cos(yAngle * Mathf.Deg2Rad), startPosition.y,
        //    //        -startPosition.x * Mathf.Sin(yAngle * Mathf.Deg2Rad));
        //    pos += position;
        //    Debug.Log("obstacle pos without y" + pos);
        //    Vector3 temp = new Vector3(pos.x, 0, pos.z);
        //    temp.y = Terrain.activeTerrain.SampleHeight(temp) + Terrain.activeTerrain.GetPosition().y;
        //    pos = temp;
        //    Debug.Log("obstacle position with y" + pos);

        //    thisTransform.position = pos;
        //    thisTransform.rotation = startRotation;
        //    thisTransform.Rotate(0, yAngle, 0, Space.World);
        //}

        // orient for collidables which have a platform as a parent
        //public virtual void Orient(PlatformObject parent, Vector3 position, Quaternion rotation)
        //{
        //    thisTransform.parent = parent.GetTransform();
        //    Vector3 pos = Vector3.zero;
        //    float yAngle = rotation.eulerAngles.y;
        //    pos.Set(startPosition.x * Mathf.Cos(yAngle * Mathf.Deg2Rad) + startPosition.z * Mathf.Sin(yAngle * Mathf.Deg2Rad), startPosition.y,
        //            -startPosition.x * Mathf.Sin(yAngle * Mathf.Deg2Rad) + startPosition.z * Mathf.Cos(yAngle * Mathf.Deg2Rad));
        //    pos += position;
        //    thisTransform.localPosition = parent.GetTransform().InverseTransformPoint(pos);
        //    thisTransform.rotation = startRotation;
        //    thisTransform.Rotate(0, rotation.eulerAngles.y, 0, Space.World);
        //}

        public virtual void Activate()
        {
            if (optimizeDeactivation)
            {
                for (int i = 0; i < childRenderers.Length; ++i)
                {
                    childRenderers[i].enabled = true;
                }


                if (renderers)
                {
                    renderers.enabled = true;
                }

                //SkinnedMeshRenderer[] skinnedMesh = GetComponentsInChildren<SkinnedMeshRenderer>();

                //if (skinnedMesh.Length > 0)
                //{
                //    skinnedMesh[0].gameObject.transform.parent.gameObject.SetActive(true);
                //}
                //for (int i = 0; i < childColliders.Length; ++i)
                //{
                //    childColliders[i].enabled = true;
                //    enabled = true;
                //}
            }
            else
            {
                thisGameObject.SetActive(true);
                //  ActiveRecursively(thisTransform, true);
            }
        }

    

        public void setInfiniteObjectPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetInfiniteObjectRandomRotation()
        {

            transform.rotation = Quaternion.Euler(0, (transform.rotation.y + Random.Range(-50f,  50f)),0);
        }
        // startup/tutorial objects will just be destroyed
        public void EnableDestroyOnDeactivation()
        {
            destroyOnDeactivation = true;
        }

        public virtual void Deactivate()
        {
            if (destroyOnDeactivation)
            {
                // don't destroy the collision particle effect
            //    ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
                Destroy(gameObject);
                return;
            }
            if (startParent)
            {
                thisTransform.parent = startParent;
            }
            infiniteObjectParent = null;
            if (optimizeDeactivation)
            {
                foreach (Renderer child in childRenderers)
                {
                    child.enabled = false;
                }

                if (renderers)
                {
                    renderers.enabled = false;
                }

            }
            else {

                thisGameObject.SetActive(false);
           //  ActiveRecursively(thisTransform, false);
            }
        }

        public static void ActiveRecursively(Transform obj, bool active)
        {
            foreach (Transform child in obj)
            {
                ActiveRecursively(child, active);
            }
            obj.gameObject.SetActive(active);
        }

        public bool IsActive()
        {
            if (optimizeDeactivation)
            {
                if (childRenderers != null)
                {
                    return childRenderers[0].enabled;
                }
                if (renderers != null)
                {
                    return renderers.enabled;
                }
            }
                
            return thisGameObject.activeSelf;
        }

        // the obejct can spawn if it contains the section or there are no sections
        public bool CanSpawnInSection(int section)
        {
            return sectionList.Count() == 0 || sectionList.ContainsSection(section);
        }

        public float GetCurrentSceneLength()
        {
            return currentObjectLength;
        }



    }
}
