using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbRotator : MonoBehaviour
{
    [SerializeField] private float _speed = 50f;
    [SerializeField] private Transform _objectToRotate;

    void Update()
    {
        _objectToRotate.Rotate(0f, _speed * Time.deltaTime, 0f);
    }
}
