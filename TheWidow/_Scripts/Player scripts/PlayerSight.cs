using UnityEngine;

public class PlayerSight : MonoBehaviour
{
    public static float DistanceFromPlayer;
    public static GameObject ObjectTag;
    public LayerMask HittableMasks;

    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 30f, HittableMasks))// checking what the raycast has hit and how far it is then storing the data in static variables
        {
            // caching the distance to the object the player is currently looking at. This helps the interaction class determine whether the object can be interacted with
            DistanceFromPlayer = hit.distance;
            ObjectTag = hit.collider.gameObject;
        }
    }
}