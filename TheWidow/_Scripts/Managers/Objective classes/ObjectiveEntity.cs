using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectiveEntity
{
    // to determine whether the objective is a collection or a destination objective
    public ObjectiveTaskType TaskType;
    // describes what to do to finish the objective
    [TextArea(2, 5)] public string Description;
    // for objectives that are about collecting
    [Header("Collection Varibales")]
    // the tag of the object to be collected i.g. Battery, Key, etc..
    [Tooltip("The type of item that needs to be collected")]
    public ItemName ObjectiveTag;
    // how many items to collect
    [Tooltip("Number of items to collected in order to finish the objective. If the objective has no collections then this value will be ignored.")]
    public int MaxItemNumber;
    [Header("Destination Variables")]
    // the trigger that will mark the completion of the objective
    [Tooltip("Set a gameobject with an ObjectiveTrigger class if you want this object to be marked \"Completed\" once the trigger is reached.")]
    public ObjectiveTrigger TriggerClass;
    [Header("Triggering Objective")]
    // whether finishing this objective triggers the start of other objectives
    [Tooltip("Set to true if finishing this objective activates new objective/s.")]
    public bool TriggerObjective = false;
    // the other objectives
    public ObjectivesGenerator.ObjectivesLoad ObjectivesToTrigger;
    // to distiguish between objectives that for example require the player to collect the same type of items
    [HideInInspector] public int ID;
    // whether the objective is a main or a side objective
    [HideInInspector] public ObjectiveTypes ObjectiveType;
    // the current number of items collected if it's a collection objective
    [HideInInspector] public int NumberCollected = 0;

    public bool IsFinished()
    {
        return NumberCollected == MaxItemNumber;
    }
}
