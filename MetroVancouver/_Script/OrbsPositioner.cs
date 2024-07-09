using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbsPositioner : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _orbs;
    [SerializeField] private float _heightToPlayer = 3f;

    void Update()
    {
        _orbs.position = new Vector3(_player.position.x, _player.position.y + _heightToPlayer, _player.position.z);
    }
}
