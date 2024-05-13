using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds additional values that the tank would need for other components. 
/// I imagined it would be useful because a value like the shield's scale would differ from tank to tank, so we need a value that would accurately do the job, and I couldn't think of a better approach than having a script that holds such values.
/// </summary>
namespace TankLike.UnitControllers
{
    public class TankAdditionalInfo : MonoBehaviour
    {
        [field: SerializeField] public float ShieldScale { get; private set; }
        [field: SerializeField] public Transform[] BrockShootingPoints { get; private set; }
    }
}