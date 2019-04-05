using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTable : MonoBehaviour
{

    [System.Serializable]
    struct Entry
    {
        public string m_Key;
        public UnityEvent m_Event;
    }

    [SerializeField] List<Entry> m_Entries = new List<Entry>();

    public UnityEvent GetEvent(string key, bool addIfNotFound = false)
    {
        for (int i = 0; i < m_Entries.Count; i++)
        {
            if (m_Entries[i].m_Key == key)
                return m_Entries[i].m_Event;
        }
        if (addIfNotFound)
        {
            m_Entries.Add(new Entry() {m_Key = key, m_Event = new UnityEvent()});
            return m_Entries[m_Entries.Count - 1].m_Event;
        }
        return null;
    }

    public int IndexOf(UnityEvent evt)
    {
        for (int i = 0; i < m_Entries.Count; i++)
        {
            if (m_Entries[i].m_Event == evt)
                return i;
        }
        return -1;
    }

}
