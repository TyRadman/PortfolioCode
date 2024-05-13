using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    [RequireComponent(typeof(Rigidbody))]
    public class TankPart : MonoBehaviour
    {
        private const float SHRINKING_DURATION = 0.5f;
        public Rigidbody RigidBody;

        private void OnValidate()
        {
            RigidBody = GetComponent<Rigidbody>();
        }

        public void StartShrinkingCountDown(float duration)
        {
            StartCoroutine(ShrinkingProcess(duration));
        }

        private IEnumerator ShrinkingProcess(float duration)
        {
            yield return new WaitForSeconds(duration);

            float time = 0f;

            while(time < SHRINKING_DURATION)
            {
                time += Time.deltaTime;

                transform.localScale = Vector3.one * Mathf.Lerp(1f, 0f, time / SHRINKING_DURATION);

                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}
