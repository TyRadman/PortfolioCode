using UnityEngine;

// this class allows the enemy to open and close doors
public class EnemyDoorOpening : MonoBehaviour
{
    private Door m_Door;

    void OnTriggerEnter(Collider cinfo)
    { 
        changeDoorState(cinfo.gameObject);
    }

    void changeDoorState(GameObject theDoor)
    {
        // if the object collided with is a door
        if (theDoor.CompareTag("Door"))
        {
            m_Door = theDoor.GetComponent<Door>();

            // the enemy can't open a locked door
            if (m_Door.IsLocked)
            {
                return;
            }

            // if the door is closed then the enemy opens it to pass
            if (!m_Door.m_IsOpened)
            {
                m_Door.ChangeDoorState();
            }
            // if the door is opened, then there is a 50% chance the enemy will close it behind it. To add randomness to the game and confuse the player
            else if (SimpleFunctions.Instance.Chance(50f))
            {
                // the action of closing the door is delayed for half a second so that the door doesn't close with the enemy in the way
                Invoke(nameof(closeDoor), 0.5f);
            }
        }
    }

    void closeDoor()
    {
        m_Door.ChangeDoorState();
    }
}