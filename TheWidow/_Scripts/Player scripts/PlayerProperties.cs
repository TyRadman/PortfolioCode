using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperties : MonoBehaviour
{
    public static PlayerProperties Instance;
    [SerializeField] private bool m_CanSprint = true;
    [SerializeField] private bool m_CanCrough = true;
    [SerializeField] private bool m_HasDepthOfField = true;
    [SerializeField] private bool m_CanZoom = true;

    private void Awake()
    {
        Instance = this;
    }
}
