using UnityEditor.Search;
using UnityEngine;

public class WaypointInfo : MonoBehaviour
{
    [field: SerializeField] public OrbsGroup OrbsGroup { get; private set; }
    [field: SerializeField] public Transform _destinationPoint { get; private set; }
    [SerializeField] private Animation _anim;
    [SerializeField] private AnimationClip _bounceClip; 
    [SerializeField] private AnimationClip _idleClip; 
    public string waypointTitle;
    public bool AtWaypoint{get; private set;} = false;

    private void Awake()
    {
        // disable the orbs on start in case some of them were left enabled
        OrbsGroup?.gameObject.SetActive(false);
    }

    // Play the bounce clip 
    public void Activate()
    {
        if (_anim != null && _bounceClip != null)
        {
            _anim.clip = _bounceClip;
            _anim.Play();
        }
        else
        {
            Debug.Log("What is wrong");
        }

        AtWaypoint = true;
    }

    // Play the idle clip 
    public void Deactivate()
    {
        if (_anim != null && _idleClip != null)
        {
            _anim.Stop();
            _anim.clip = _idleClip; 
            _anim.Play();
        }

        AtWaypoint = false;
    }
}
