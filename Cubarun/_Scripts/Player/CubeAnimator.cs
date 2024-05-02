using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAnimator : MonoBehaviour
{
    public static CubeAnimator Instance;
    [SerializeField] private Transform mesh;
    [SerializeField] private float shrinkingSizeSpeed = 0.2f;
    private float originalShrinkingSpeed;
    private float growingSizeSpeed;

    [SerializeField] private Vector3 minSize;
    [SerializeField] private Vector3 minSizePos;
    private Vector3 originalSize;
    private Vector3 originalPos;

    //constants
    private const float TIME_TO_RETURN_TO_IDLE = 0.05f;     // how long it takes for the cube to grow back to normal after the button has been released
    private const float SHRINK_TO_GROW_RATIO = 3;           // how fast does the cube shrink compared to how fast it grows

    private void Awake()
    {
        Instance = this;
        originalPos = Vector3.zero;
        originalSize = Vector3.one;
        growingSizeSpeed = shrinkingSizeSpeed * SHRINK_TO_GROW_RATIO;
        originalShrinkingSpeed = shrinkingSizeSpeed;
    }

    public void Turn()
    {
        CancelInvoke(nameof(returnToIdle));
        mesh.localScale = Vector3.Lerp(mesh.localScale, minSize, shrinkingSizeSpeed);
        mesh.localPosition = Vector3.Lerp(mesh.localPosition, minSizePos, shrinkingSizeSpeed);
    }

    public void ResetSize()
    {
        mesh.localScale = Vector3.one;
    }

    public void TurnToIdle()
    {
        Invoke(nameof(returnToIdle), TIME_TO_RETURN_TO_IDLE);
    }

    void returnToIdle()
    {
        mesh.localScale = Vector3.Lerp(mesh.localScale, originalSize, growingSizeSpeed);
        mesh.localPosition = Vector3.Lerp(mesh.localPosition, originalPos, growingSizeSpeed);
    }

    public void ChangeShrinkSpeed(float dividor)
    {
        shrinkingSizeSpeed = originalShrinkingSpeed;
        shrinkingSizeSpeed /= dividor;
    }
}
