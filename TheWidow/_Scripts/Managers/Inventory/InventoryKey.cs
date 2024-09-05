using System.Collections.Generic;

public class InventoryKey : InventoryInteractable
{
    public Key.KeyType Key_Type;
    public static List<Door> Doors = new List<Door>();
    public List<Door> test = new List<Door>();

    public void Start()
    {
        Doors.Clear();
        
        if (Doors.Count == 0)
        {
            // we find all doors in the scene. If the door is locked and requires a key, then it's cached in the Doors list
            System.Array.ForEach(FindObjectsOfType<Door>(), d => { if (d.InventoryVars.m_InventoryUsage) Doors.Add(d); });
        }

        // we make the checkKeys function listen to any OnObjectInteraction event which is triggered when the player is close enough to the door
        Doors.ForEach(d => d.OnInteraction += CheckKeys);
        test = Doors;   // debugging purposes
    }

    public override void OnUsed()
    {
        var door = Doors.Find(d => d.IsActivated);
        door.UnlockDoor();

        base.OnUsed();
    }

    public void CheckKeys(Key.KeyType _unlockingKey)
    {
    }

    public void Unsubscribe(Door _doorToRemove)
    {
        Doors.ForEach(d => d.OnInteraction -= CheckKeys);
        Doors.Remove(_doorToRemove);
    }

    public void AddDoor(Door _door)
    {
        Doors.Add(_door);
        test = Doors;
    }
}