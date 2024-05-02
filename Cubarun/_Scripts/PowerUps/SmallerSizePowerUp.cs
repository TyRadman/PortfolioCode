using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallerSizePowerUp : PowerUp
{
    [SerializeField] [Range(0.3f, 0.9f)] private float smalledSize = 0.5f;
    [SerializeField] [Range(0.5f, 2f)] private float transitionSpeed = 1f;

    public override void Effect()
    {
        base.Effect();

        EffectsPoolingManager.instance.SpawnEffect("Purple Transformation", playerMovement.Instance.transform, transitionSpeed);
        PlayerComponents.Instance.ChangeSize(smalledSize, transitionSpeed);
    }

    public override void Deeffect()
    {
        base.Deeffect();

        PlayerComponents.Instance.ChangeSize(1f / smalledSize, transitionSpeed);
    }

    protected override void InstantDeactiviation()
    {
        base.InstantDeactiviation();

        PlayerComponents.Instance.ResetSize();
    }
}
