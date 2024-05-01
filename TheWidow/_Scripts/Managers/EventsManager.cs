using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// a class that triggers events once the player enters a specified zone
public class EventsManager : MonoBehaviour
{
    public static EventsManager Instance;
    private List<HidingAndShowingTriggers> m_Events = new List<HidingAndShowingTriggers>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddEvent(HidingAndShowingTriggers _event)
    {
        m_Events.Add(_event);
    }

    public void TriggerEvent(string _eventName)
    {
        // try and catch are used for debugging purposes
        try
        {
            m_Events.Find(e => e.GetEventName() == _eventName).Activate();
        }
        catch
        {
            Debug.LogError("Event name doesn't exist.");
        }
    }
}
