using System.Collections;
using UnityEngine;

public class FingerObject : MonoBehaviour
{
    public static FingerObject i;
    public GameObject player;
    private WaypointInfo _waypoint;
    [SerializeField] private float _teleportationDuration = 1;
    [SerializeField] private AnimationCurve _teleporationCurve;
    [SerializeField] private AnimationCurve _heightCurve;
    [SerializeField] private float _teleportationHeight = 3;
    private bool _canInteractWithPinPoints = true;

    // Initialize singleton 
    private void Awake()
    {
        i = this;
    }

    // When finger enters waypoint collider, display its canvas
    private void OnTriggerEnter(Collider waypointCollider)
    {
        if (!_canInteractWithPinPoints)
        {
            return;
        }

        // Check that the finger has entered the waypoint object 
        if (waypointCollider.CompareTag("Waypoint"))
        {
            // Get a reference to the waypoint object 
            _waypoint = waypointCollider.GetComponent<WaypointInfo>();

            // Display the waypoint's canvas 
            if (_waypoint != null && _waypoint.AtWaypoint == false)
            {
                CountdownBar.i.DisplayElementsAtWaypoint(_waypoint);
            }
        }
    }

    // When finger exits waypoint collider, remove its canvas
    private void OnTriggerExit(Collider waypointCollider)
    {
        if (!_canInteractWithPinPoints)
        {
            return;
        }

        if (waypointCollider.CompareTag("Waypoint"))
        {
            if (_waypoint != null && _waypoint.AtWaypoint == false)
            {
                CountdownBar.i.RemoveElementsAtWaypoint(_waypoint);
            }
        }
    }

    // Transform the player object's location 
    public void TransformPlayer()
    {
        if (_waypoint != null)
        {
            _canInteractWithPinPoints = false;
            StartCoroutine(TeleportationProcess());
            WaypointManager.i.ActivateWaypoint(_waypoint);
        }
    }

    private IEnumerator TeleportationProcess()
    {
        Vector3 playerStartPosition = player.transform.position;
        Vector3 waypointPosition = _waypoint._destinationPoint.position + Vector3.up * _teleportationHeight;
        float timer = 0;

        while (timer <= _teleportationDuration)
        {
            timer += Time.deltaTime;
            float t = _teleporationCurve.Evaluate(timer / _teleportationDuration);

            Vector3 newPosition = Vector3.Lerp(playerStartPosition, waypointPosition, t);
            newPosition.y += _heightCurve.Evaluate(timer / _teleportationDuration) * _teleportationHeight;

            player.transform.position = newPosition;
            yield return null;
        }

        // disable all the orbs
        WaypointManager.i.DisableWaypointOrbs();

        // after the player reaches the destination, enable the orbs of the destination if the destination has orbs
        if (_waypoint.OrbsGroup != null)
        {
            _waypoint.OrbsGroup?.gameObject.SetActive(true);
            OrbsManager.Instance.SelectedOrbsGroup = _waypoint.OrbsGroup;
        }

        // enable interaction with the minimap
        _canInteractWithPinPoints = true;
        // make sure the player is positioned at the end point
        player.transform.position = waypointPosition;
    }
}
