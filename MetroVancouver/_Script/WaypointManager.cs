using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    [SerializeField] private List<WaypointInfo> Waypoints;
    public static WaypointManager i; 

    // Initialize singleton
    private void Awake()
    {
        i = this;
    }

    // "Activate" the current waypoint by triggering the bounce animation
    public void ActivateWaypoint(WaypointInfo selectedWaypoint)
    {
        if (Waypoints != null && selectedWaypoint != null)
        {
            Waypoints.ForEach(w => w.Deactivate());
            selectedWaypoint.Activate();
        }
        else
        {
            Debug.Log("What is wrong");
        }
    }

    public void DisableWaypointOrbs()
    {
        Waypoints.ForEach(o => o.OrbsGroup?.gameObject.SetActive(false));
    }
}
