using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivesManager : MonoBehaviour
{
    public static ObjectivesManager Instance;
    private List<ObjectiveEntity> m_Objectives = new List<ObjectiveEntity>();
    private int IDNumber = 0;
    private ObjectivesUI m_hudElements;

    private void Awake()
    {
        Instance = this;
        m_hudElements = FindObjectOfType<ObjectivesUI>();
        m_hudElements.UpdateElements(m_Objectives);
    }

    public void AddObjectives(ObjectiveEntity[] newObjectives, ObjectiveTypes _type)
    {
        // displaying a notification based on the objective type
        if (_type == ObjectiveTypes.Main)
        {
            NotificationsPanel.Instance.ShowNotification(NotificationsPanel.NotificationType.NewObjective);
        }
        else
        {
            NotificationsPanel.Instance.ShowNotification(NotificationsPanel.NotificationType.NewSideObjective);
        }

        for (int i = 0; i < newObjectives.Length; i++)
        {
            m_Objectives.Add(newObjectives[i]);
            newObjectives[i].ID = IDNumber++;
        }

        triggerObjectives(newObjectives);

        // refresh UI
        m_hudElements.UpdateElements(m_Objectives);
    }

    private void triggerObjectives(ObjectiveEntity[] _objectives)
    {
        // triggering objectives that are collective
        var collectionObjs = System.Array.FindAll(_objectives, o => o.TaskType == ObjectiveTaskType.Collection);

        // for every objective that is a collection objective we want to turn its "IsObjective" bool to true
        for (int i = 0; i < collectionObjs.Length; i++)
        {
            ItemsManager.Instance.TriggerItemWithTag(collectionObjs[i].ObjectiveTag);
        }

        // triggering objectives that are about a destination
        var destinationObjs = System.Array.FindAll(_objectives, o => o.TaskType == ObjectiveTaskType.Destination);

        for (int i = 0; i < destinationObjs.Length; i++)
        {
            // giving the trigger the id so that it uses it when the objective is completed
            destinationObjs[i].TriggerClass.ID = destinationObjs[i].ID;
        }
    }

    // when an item is collected. Only for objectives that are about collecting an item.
    public void ItemCollected(ItemName _itemTag)
    {
        // gets all objectives that have the specified tag from main and side objectives
        var selectedObjs = m_Objectives.FindAll(o => o.ObjectiveTag == _itemTag && o.TaskType == ObjectiveTaskType.Collection);
        // updates the number in the HUD. This includes main and side objectives
        m_hudElements.GetTextElementsWithTag(_itemTag).ForEach(ou => ou.ItemCollected());
        // updates the values of the collected item
        selectedObjs.ForEach(o => o.NumberCollected++);

        // check if the player is finished with the objective that has this tag
        // we start with main objectives
        foreach (ObjectiveEntity obj in selectedObjs)
        {
            if (obj.IsFinished())
            {
                ObjectiveFinished(obj.ID);
            }
        }
    }

    public void ObjectiveFinished(int _id)
    {
        // notify the player that the objective is completed
        NotificationsPanel.Instance.ShowNotification(NotificationsPanel.NotificationType.ObjectiveFinished);
        // find the specified objective by its ID
        var obj = m_Objectives.Find(o => o.ID == _id);
        // remove finished objective from the list displayed
        m_hudElements.RemoveObjective(obj.ID, obj.ObjectiveType);
        // remove the objective from the list
        m_Objectives.Remove(obj);
        // update the panel
        m_hudElements.UpdateElements(m_Objectives);
    }
}

public interface i_ObjectiveItem
{
    bool IsObjective { get; set; }
}

public enum ObjectiveTaskType
{
    Collection, Destination
}

public enum ObjectiveTypes
{
    Main, Side
}

public enum CollectablesTypes
{
    None, Battery, SmallKey, MainKey
}

