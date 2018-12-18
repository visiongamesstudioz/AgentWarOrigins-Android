using UnityEngine;
using System.Collections;
namespace EndlessRunner
{
    public class CameraController : MonoBehaviour
    {
        public float EnemySceneFOV = 20f;

        public float NormalSceneFOV = 50f;
        public float SceneFOVInterPolationSpeed = 15f;

        public Vector3 NormalSceneDistanceFromTarget;
        public Vector3 EnemySceneDistanceFromTarget;
        public float damping = 1f;
        private GameObject target;
        private Transform camTransform;
        private Vector3 offset;
        private Vector3 m_DistanceFromTarget;
        // Use this for initialization
        void Start()
        {
            m_DistanceFromTarget = NormalSceneDistanceFromTarget;
            target = GameObject.FindGameObjectWithTag("Player");
            if (target != null)
            {
                transform.position = target.transform.position - m_DistanceFromTarget;

                offset = transform.position - target.transform.position;
            }
       
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = target.transform.position - m_DistanceFromTarget;
            offset = transform.position - target.transform.position;
        }

        void LateUpdate()
        {
            FollowPlayer();
        }

        void FollowPlayer()
        {
         
            Vector3 desiredPosition = target.transform.position + offset;
            Vector3 position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * damping);
            transform.position = desiredPosition;

        }



        //public void SetDistanceFromTarget(float distanceToTargetX)
        //{
        //    m_DistanceFromTarget = new Vector3(distanceToTargetX, m_DistanceFromTarget.y, m_DistanceFromTarget.z);
        //}

        public void SetNormalDistanceFromTarget()
        {
            m_DistanceFromTarget = NormalSceneDistanceFromTarget;
        }

        public void SetEnemyDistanceFromTarget()
        {
            m_DistanceFromTarget = EnemySceneDistanceFromTarget;

        }
    }
}