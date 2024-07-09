using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbsManager : MonoBehaviour
{
    public static OrbsManager Instance;
    [SerializeField] private List<OrbsGroup> _orbGroups;
    [field: SerializeField] public OrbsGroup SelectedOrbsGroup { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public void EnableOrbs(bool enable)
    {
        if(SelectedOrbsGroup == null)
        {
            return;
        }

        // enable or disable all the orbs
        SelectedOrbsGroup.Orbs.ForEach(o => o.gameObject.SetActive(enable));
    }
}
