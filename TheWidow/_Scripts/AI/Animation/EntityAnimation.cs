using System;
using System.Collections;
using UnityEngine;

public class EntityAnimation : MonoBehaviour, IController
{
    [SerializeField] protected Animator Anim;
    protected StatesEntity Entity;

    private void Awake()
    {
        Entity = GetComponent<StatesEntity>();
    }

    // animates the enemy according to its state
    public virtual void Animate(MachineState.StateName _state)
    {

    }

    public void DisableCulling()
    {
        Anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }

    public virtual void SetUp(IComponent component)
    {
    }

    internal void Animate(object stateTag)
    {
        throw new NotImplementedException();
    }

    public virtual void Activate()
    {
    }

    public virtual void Dispose()
    {
    }
}