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
    public float CrouchingDistance;
    public float WalkingDistance;
    public float RunningDistance;
    public float DoorOpeningDistance;
    public float SmallInteractionsDistance;
    [Header("Other enemy stats:")]
    public MovementSpeeds TheMovementSpeeds;
    public float SightDistance;
    public float LosingPlayerTime;
    public float WalkingSpeed;
    public float RunningSpeed;
    [Header("Player stats:")]
    public float BatteryMaxLife;
    public float MaxStamina;
    [Header("Sight Range")]
    [Range(0, 2)]
    public int SightRadius;
}

[System.Serializable]
public struct HearingDistances
{
    public float CrouchingDistance;
    public float WalkingDistance;
    public float RunningDistance;
    public float DoorOpeningDistance;
    public float SmallInteractionsDistance;
}
[System.Serializable]
public struct MovementSpeeds
{
    public float PatrolSpeed;
    public float CheckingSpeed; // related to hearing
    public float ChasingSpeed;
}