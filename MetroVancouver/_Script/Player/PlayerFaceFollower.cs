using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFaceFollower : MonoBehaviour
{
    [Flags]
    public enum FollowDirections
    {
        Yaw = 1, Pitch = 2, Roll = 4
    }

    [SerializeField] private FollowDirections _directionsToFollow;
    [SerializeField] private bool _isActive = false;
    private Transform _currentObject;
    [Tooltip("The lower the value, the slower the follow process happens")]
    [SerializeField] [Range(0f, 1f)] private float _followSpeed = 0.2f;
    [SerializeField] private Transform _objectToFollow;

    private void Awake()
    {
        _currentObject = transform;
    }

    private void OnEnable()
    {
        _currentObject.position = _objectToFollow.position;
    }

    void Update()
    {
        FollowObjectRotation();
        FollowPosition();
    }

    private void FollowPosition()
    {
        _currentObject.position = Vector3.Lerp(_currentObject.position, _objectToFollow.position, _followSpeed);
    }

    private void FollowObjectRotation()
    {
        if (!_isActive)
        {
            return;
        }

        Vector3 rotation = _currentObject.eulerAngles;
        Vector3 newRotation = _objectToFollow.eulerAngles;

        if ((_directionsToFollow | FollowDirections.Yaw) == FollowDirections.Yaw)
        {
            float currentYaw = rotation.y;
            float newYaw = newRotation.y;

            // we're doing this to correct the rotation once the player's head rotates above 360 or below 0
            float difference = newYaw - currentYaw;

            if (difference > 180)
            {
                difference -= 360;
            }
            else if (difference < -180)
            {
                difference += 360;
            }

            rotation.y += difference;
        }

        if ((_directionsToFollow | FollowDirections.Pitch) == FollowDirections.Pitch)
        {
            rotation.x = newRotation.x;
        }

        if ((_directionsToFollow | FollowDirections.Roll) == FollowDirections.Roll)
        {
            rotation.z = newRotation.z;
        }

        _currentObject.eulerAngles = Vector3.Lerp(_currentObject.eulerAngles, rotation, _followSpeed);
    }
}
