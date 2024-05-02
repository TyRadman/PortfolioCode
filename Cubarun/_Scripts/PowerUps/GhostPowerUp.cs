using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPowerUp : PowerUp
{
    [SerializeField] private Material transformationMaterial;


    public override void Effect()
    {
        base.Effect();

        PlayerInfoManager.Instance.SetMaterial(transformationMaterial, powerUpEffectDuration);
        Obstacle.CurrentState = Obstacle.ObstacleState.Transparent;
    }

    public override void Deeffect()
    {
        base.Deeffect();


    }

    protected override void InstantDeactiviation()
    {
        base.InstantDeactiviation();

        PlayerInfoManager.Instance.RestoreOriginalMaterial();
    }
}
