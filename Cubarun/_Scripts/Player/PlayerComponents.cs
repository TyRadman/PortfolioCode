using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponents : MonoBehaviour
{
    public static PlayerComponents Instance;
    private BoxCollider boxCollider;
    private CapsuleCollider capsuleCollider;
    private Rigidbody rb;
    private CubeAnimator anim;
    private RigidbodyConstraints originalConstraints;
    [SerializeField] private float laserPushForce;

    internal void Rotate()
    {
        rb.AddTorque(new Vector3(Random.Range(0, 90), Random.Range(0, 90), Random.Range(0, 90)));
    }

    private void Awake()
    {
        Instance = this;
        anim = GetComponent<CubeAnimator>();
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        originalConstraints = rb.constraints;
    }

    public void Turn(bool turning)
    {
        if (turning)
        {
            anim.Turn();
        }
        else
        {
            anim.TurnToIdle();
        }
    }

    public void MoveRigidBody(Vector3 velocity)
    {
        rb.velocity = velocity;
    }

    public void PlayerState(bool dead)
    {
        boxCollider.enabled = dead;
        capsuleCollider.enabled = !dead;
        rb.constraints = dead? RigidbodyConstraints.None : originalConstraints;
    }

    public void FreezeMovement()
    {
        rb.velocity = Vector3.zero;
    }

    public void ResetSize()
    {
        anim.ResetSize();
    }

    public void ChangeSize(float sizeMultiplier, float transitionSpeed = 0f)
    {
        StartCoroutine(changeTheSize(sizeMultiplier, transitionSpeed));
    }

    private IEnumerator changeTheSize(float newSize, float transitionSpeed)
    {
        float time = 0f;
        var newScale = transform.localScale * newSize;
        var oldScale = transform.localScale;

        while (time < transitionSpeed)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(oldScale, newScale, time / transitionSpeed);

            yield return null;
        }
    }

    public void ApplyLaserPush(Vector3 contactPoint)
    {

        Vector3 force = new Vector3(Random.Range(0, laserPushForce) * HandBook.RandonSign(),
            Random.Range(0, laserPushForce) * HandBook.RandonSign(), Random.Range(0, laserPushForce) * HandBook.RandonSign());
        
        rb.AddForceAtPosition(force, contactPoint);
    }
}
