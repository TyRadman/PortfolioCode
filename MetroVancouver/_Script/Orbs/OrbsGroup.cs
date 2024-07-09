using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbsGroup : MonoBehaviour
{
    public List<Orb> Orbs;
    public bool EditorMode = false;
    [Header("Orbs Display")]
    [Range(0.1f, 1000f)] public float SpeadRadius;
    [Range(0.1f, 100f)] public float OrbsSize = 1f;
}
