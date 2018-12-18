using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    [SerializeField] protected PathCreator PathCreator;
    public float Speed;
    public float RotationSpeed;
    protected int CurrentWayPointId = 0;
    [HideInInspector] public Quaternion PreviousRotation;
    [HideInInspector] public Quaternion CurrentRotation;
    protected readonly float _reachDistance = 1;
    protected GameObject m_player;
    // Use this for initialization

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
    }
    // Update is called once per frame
    public virtual void Update()
    {
        if (PathCreator)
        {
            if (!IsLastWayPointReached())
            {
                HandleWaypointMovement();
            }
            else
            {
                FacePlayer(false);
            }
        }
    }


    public void HandleWaypointMovement()
    {
        var distance = Vector3.Distance(transform.position,
            PathCreator.PathWayPoints[CurrentWayPointId].transform.position);
        Vector3 targetRotation = PathCreator.PathWayPoints[CurrentWayPointId].transform.position -
                                 transform.position;
        if (targetRotation != Vector3.zero)
        {
            var lookRotation =
                Quaternion.LookRotation(PathCreator.PathWayPoints[CurrentWayPointId].transform.position -
                                        transform.position);
            PreviousRotation = transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
            CurrentRotation = transform.rotation;
            transform.position = Vector3.MoveTowards(transform.position,
                PathCreator.PathWayPoints[CurrentWayPointId].transform.position, Time.deltaTime * Speed);
        }

        if (distance < _reachDistance)
            CurrentWayPointId++;
        if (CurrentWayPointId >= PathCreator.PathWayPoints.Count)
        {
            CurrentWayPointId = PathCreator.PathWayPoints.Count - 1;

            // enabled = false;
        }
    }

    public void HandleWaypointMovement(PathCreator pathCreator)
    {
        var distance = Vector3.Distance(transform.position,
            pathCreator.PathWayPoints[CurrentWayPointId].transform.position);
        Vector3 targetRotation = pathCreator.PathWayPoints[CurrentWayPointId].transform.position -
                                 transform.position;

        if (targetRotation != Vector3.zero)
        {
            var lookRotation =
                Quaternion.LookRotation(pathCreator.PathWayPoints[CurrentWayPointId].transform.position -
                                        transform.position);
            PreviousRotation = transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
            CurrentRotation = transform.rotation;
        }

        transform.position = Vector3.MoveTowards(transform.position,
            pathCreator.PathWayPoints[CurrentWayPointId].transform.position, Time.deltaTime * Speed);
        if (distance < _reachDistance)
            CurrentWayPointId++;
        if (CurrentWayPointId > pathCreator.PathWayPoints.Count)
        {
            CurrentWayPointId = pathCreator.PathWayPoints.Count;

            // enabled = false;
        }
    }

    public bool IsLastWayPointReached()
    {
        return CurrentWayPointId == PathCreator.PathWayPoints.Count - 1;
    }

    public bool IsLastWayPointReached(PathCreator pathCreator)
    {
        return CurrentWayPointId == pathCreator.PathWayPoints.Count;
    }

    public void ResetCurrentWayPoint()
    {
        CurrentWayPointId = 0;
    }

    public void FacePlayer(bool immediate)
    {
        //    transform.LookAt(m_player.transform);

        Vector3 relativePos = m_player.transform.position - transform.position +
                              new Vector3(5, Random.Range(-1.0f, -2.0f), 2);
        if (relativePos != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            if (immediate)
            {
                transform.rotation = rotation;
            }
            else
            {
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
            }
        }
   
    }
}
