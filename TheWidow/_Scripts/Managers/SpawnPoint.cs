using UnityEngine;
using System.Collections.Generic;

// a single spawning point class
public class SpawnPoint : MonoBehaviour
{
    private static readonly float MAX_LOOK_DOWN_DISTANCE = 5f;
    public ItemName Type;
    public List<ItemName> Types = new List<ItemName>();

    private void Awake()
    {
        // when the game starts, items are set flat on the surface below them. This saves some times as all I need to do is just put the object where I want it without having to worry about making it stand perfectly there
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, MAX_LOOK_DOWN_DISTANCE))
        {
            transform.position = hit.point;                         // sets the spawning point right on the surface of the object beneath it
            transform.rotation = Quaternion.AngleAxis(Random.Range(0f, 360f), transform.up);
        }
    }
}