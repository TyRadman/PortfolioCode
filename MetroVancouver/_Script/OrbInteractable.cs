using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OrbInteractable : XRBaseInteractable
{
    [Header("Orb Settings")]
    [SerializeField] private Orb _orb;

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);

        _orb.OnHovered();
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        _orb.OnUnhovered();
    }
}
