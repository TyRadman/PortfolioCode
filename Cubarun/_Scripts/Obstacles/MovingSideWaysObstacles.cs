using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Obstacles
{
    public class MovingSideWaysObstacles : MonoBehaviour
    {
        [SerializeField] private float speed = 2f; // how long it takes the obstacle to reach one side 
        private const float maxOffset = 5f;
        private Vector3 startPoint;
        private Vector3 endPoint;

        private void OnEnable()
        {
            startPoint = new Vector3(-maxOffset, 1, transform.position.z);
            endPoint = new Vector3(maxOffset, 1, transform.position.z);
        }

        private void Start()
        {
            StartCoroutine(moveToTheSide(true));
        }

        private IEnumerator moveToTheSide(bool forth)
        {
            float time = Mathf.InverseLerp(-maxOffset, maxOffset, transform.position.x) * speed;

            while(time < speed)
            {
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(forth? startPoint : endPoint, forth? endPoint : startPoint, time / speed);
                yield return null;
            }
        }
    }
}