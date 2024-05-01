using System.Collections;
using UnityEngine;

public class EntityAnimation : MonoBehaviour
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
}