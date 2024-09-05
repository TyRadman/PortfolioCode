using System.Collections.Generic;
using UnityEngine;

// a scriptableObject class that has values that change depending on the difficulty of the game (more attributes are to be added)
[CreateAssetMenu(fileName = "Value", menuName = "Difficulty values")]
public class DifficultyModifier : ScriptableObject
{
    public enum EnemyTag
    {
        TheWidow, Dog, Cat
    }

    public EnemyTag TheEnemyTag;
    public string Name;
    [Header("Hearing Distances:")]
    public HearingDistances TheHearingDistances;
    [Header("Other enemy stats:")]
    public float SightDistance;
    public float LosingPlayerTime;
    public float WalkingSpeed;
    public float ChasingSpeed;
    [Header("Player stats:")]
    public float BatteryMaxLife;
    public float MaxStamina;
    [Header("Sight Range")]
    [Range(0, 2)]
    public int SightRadius;
}

[System.Serializable]
public class HearingDistance
{
    public PlayerMovement.States State;
    public float Distance;
}

[System.Serializable]
public class HearingDistances
{
    public List<HearingDistance> Distances;
    public float DoorHearingDistance = 10f;
}