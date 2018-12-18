using UnityEngine;

public class VehiclePathFollower : PathFollower
{

    public float MaxDistanceBetweenPlayerToStart;  
    private SimpleCarController m_SimpleCarController;
    // Use this for initialization
   void Start()
    {
        m_SimpleCarController = GetComponent<SimpleCarController>();
    }

    // Update is called once per frame
    public override void Update()
    {
        if (m_player == null)
        {
            m_player = GameObject.FindGameObjectWithTag("Player");
        }
        if (PathCreator)
        {
            if (m_player)
            {
                if (Vector3.Distance(transform.position, m_player.transform.position) < MaxDistanceBetweenPlayerToStart)
                {
                    m_SimpleCarController.enabled = true;
                    if (!IsLastWayPointReached())
                    {
                        HandleWaypointMovement();
                    }

                }
            }
        }
    }

    public Vector3 DifferenceInRotation()
    {
        return CurrentRotation.eulerAngles - PreviousRotation.eulerAngles;
    }

  
}
