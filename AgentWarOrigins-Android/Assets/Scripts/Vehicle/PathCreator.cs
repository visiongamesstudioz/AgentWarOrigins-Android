using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour {

    public Color PathColor=Color.white;
    public LayerMask Mask;

    public List<Transform> PathWayPoints= new List<Transform>();
    private Transform[] _wayPoints;

    private void OnDrawGizmos()
    {
        Gizmos.color = PathColor;
        _wayPoints = GetComponentsInChildren<Transform>();
        PathWayPoints.Clear();

        foreach (Transform wayPoint in _wayPoints)
        {
            if (wayPoint != this.transform)
            {
                PathWayPoints.Add(wayPoint);
            }
        }
        for (int i = 0; i < PathWayPoints.Count; i++)
        {
            Vector3 position = PathWayPoints[i].position;
            if (i > 0)
            {
                Vector3 previous = PathWayPoints[i - 1].position;
                Gizmos.DrawLine(previous,position);
            }

            Gizmos.DrawSphere(position, 0.2f);

        }

    }



}
