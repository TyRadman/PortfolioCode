using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using Misc;

    [CreateAssetMenu(fileName = "Poolable_NAME", menuName = Directories.MAIN + "Poolable Reference")]

    public class PoolableParticlesReference : ScriptableObject
    {
        [field: SerializeField] public ParticleSystemHandler Poolable { get; private set; }
    }
}
