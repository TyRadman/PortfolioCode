using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalPowerUp : PowerUp
{
    [SerializeField] private float pushForce = 10f;
    [SerializeField] private float explosionRaduis = 3f;
    [SerializeField] [Range(1.2f, 2.5f)] private float sizeMultiplier = 1.5f;
    [SerializeField] [Range(0.5f, 0.9f)] private float turningSpeedChange = 0.7f;
    [SerializeField] [Range(0.5f, 0.9f)] private float movementSpeedChange = 0.8f;
    [SerializeField] [Range(0.1f, 2f)] private float transitionSpeed = 1f;
    public static float ExplosionRaduis = 3f;
    public static float PushForce;

    private void Awake()
    {
        PushForce = pushForce;
        ExplosionRaduis = explosionRaduis;
    }

    public override void Effect()
    {
        base.Effect();

        EffectsPoolingManager.instance.SpawnEffect("Metal Transformation", playerMovement.Instance.transform, transitionSpeed);
        EffectsPoolingManager.instance.SpawnEffect("Walls Hit", playerMovement.Instance.transform.position);
        PlayerComponents.Instance.ChangeSize(sizeMultiplier, transitionSpeed);
        Obstacle.CurrentState = Obstacle.ObstacleState.FlyingAway;
        playerMovement.Instance.ChangeTurnSpeed(turningSpeedChange);
        playerMovement.Instance.ChangeMovementSpeed(movementSpeedChange);
    }

    public override void Deeffect()
    {
        base.Deeffect();

        PlayerComponents.Instance.ChangeSize(1f / sizeMultiplier, transitionSpeed / 4f);
        Obstacle.CurrentState = Obstacle.ObstacleState.Normal;
        playerMovement.Instance.ChangeTurnSpeed(1f);
        playerMovement.Instance.ChangeMovementSpeed(1f);
    }
}
