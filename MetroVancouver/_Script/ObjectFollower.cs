using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollower : MonoBehaviour
{
    [SerializeField] private Transform _objectToFollow;
    [SerializeField] private bool _isActive = false;
    [Tooltip("The lower the value, the slower the follow process happens")]
    [SerializeField] [Range(0f, 1f)] private float _followSpeed = 0.2f;

    void Update()
    {
        FollowProcess();
    }

    private void FollowProcess()
    {
        if (!_isActive)
        {
            return;
        }

        transform.position = Vector3.Lerp(transform.position, _objectToFollow.position, _followSpeed);
    }
}
