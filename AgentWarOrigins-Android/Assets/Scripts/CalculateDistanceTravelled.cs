using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;

public class CalculateDistanceTravelled : MonoBehaviour
{
    private float distanceTravelled;
    private Vector3 lastPosition;
    //private CurrentGameStats m_CurrentGameStats;
    private List<Mission> m_travelDistanceMissions;
    private float m_MissionCheckTimer = 5f;
    private float m_CurrentPassedTimeToCheckMissions;
    private MissionManager m_MissionManager;
    private float previousDistanceTravelled;
    void Start()
    {
        m_MissionManager=MissionManager.Instance;
        lastPosition = transform.position;
       // m_CurrentGameStats = SaveLoadManager.Instance.CurrentGameStats;
        m_CurrentPassedTimeToCheckMissions = m_MissionCheckTimer;
        m_travelDistanceMissions=new List<Mission>();
    }


    void Update()
    {
        if (GameManager.Instance.IsGameActive())
        {
            previousDistanceTravelled = distanceTravelled;
            distanceTravelled += Vector3.Distance(transform.position, lastPosition);
            lastPosition = transform.position;
            PlayerData.CurrentGameStats.CurrentDistanceTravelled = Mathf.Floor(distanceTravelled);
            PlayerData.PlayerProfile.TotalDistanceTravelled += (distanceTravelled - previousDistanceTravelled);
            m_CurrentPassedTimeToCheckMissions -= Time.deltaTime;
            if (m_CurrentPassedTimeToCheckMissions < 0)
            {
                CheckForTravelDistanceMissions();
                m_CurrentPassedTimeToCheckMissions = m_MissionCheckTimer;
            }
        }
    
    }

    private void CheckForTravelDistanceMissions()
    {
       // m_PlayerProfile = SaveLoadManager.Instance.Load();
        m_travelDistanceMissions.Clear();
        foreach (Mission mission in m_MissionManager.m_ActiveMissions)
        {
            if (mission.MissionType == MissionType.TravelDistance)
            {
                m_travelDistanceMissions.Add(mission);
            }
        }
        foreach (Mission mission in m_travelDistanceMissions)
        {
            if (m_travelDistanceMissions.Count > 0)
            {
                if (!mission.IsMissionForOneRun)
                {
                    if ((int)PlayerData.PlayerProfile.TotalDistanceTravelled >= mission.AmountOrObjectIdToComplete)
                    {

                        EventManager.TriggerEvent(mission.MissionTitle);
                    }
                }
                else
                {
                    //check for current game stats
                 //   Debug.Log(m_CurrentGameStats.CurrentDistanceTravelled);
                    if ((int)PlayerData.CurrentGameStats.CurrentDistanceTravelled >= mission.AmountOrObjectIdToComplete)
                    {
                        EventManager.TriggerEvent(mission.MissionTitle);
                    }
                }
            }
        }
    }
}
