using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionCopier : MonoBehaviour
{
    [SerializeField] private Transform _objectToCopy;
    [SerializeField] private float _lerpSpeed = 0.2f;

    private void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, _objectToCopy.rotation, _lerpSpeed);
        transform.position = Vector3.Lerp(transform.position, _objectToCopy.position, _lerpSpeed);
    }
}
