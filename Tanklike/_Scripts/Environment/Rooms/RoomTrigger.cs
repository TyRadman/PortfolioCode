using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment
{
    public class RoomTrigger : MonoBehaviour
    {
        [field: SerializeField] public System.Action OnTriggerEnterEvent { get; set; }
        [field: SerializeField] public System.Action OnTriggerExitEvent { get; set; }
        private Collider _trigger;

        private void Awake()
        {
            _trigger = GetComponent<BoxCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnTriggerEnterEvent?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnTriggerExitEvent?.Invoke();
            }
        }
        public void CloseRooom()
        {
            _trigger.enabled = false;
        }
    }
}
