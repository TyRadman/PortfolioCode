using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Utils
{
    public static class Helper
    {
        /// <summary>
        /// Returns a random float value between the vector's x and y values.
        /// </summary>
        /// <param name="_value"></param>
        /// <returns></returns>
        public static float RandomValue(this Vector2 _value)
        {
            return Random.Range(_value.x, _value.y);
        }

        public static float Lerp(this Vector2 _value, float _tValue)
        {
            return Mathf.Lerp(_value.x, _value.y, _tValue);
        }

        public static int Lerp(this Vector2Int _value, float _tValue)
        {
            return (int)Mathf.Lerp(_value.x, _value.y, _tValue);
        }

        /// <summary>
        /// Returns a random integer value between the vector's x and y values.
        /// </summary>
        /// <param name="_value"></param>
        /// <returns></returns>
        public static int RandomValue(this Vector2Int _value)
        {
            return Random.Range(_value.x, _value.y + 1);
        }

        public static Vector2 RandomVector2(this Vector2 _value)
        {
            return new Vector2(Random.Range(-_value.x, _value.x), Random.Range(-_value.y, _value.y));
        }

        #region List Methods
        /// <summary>
        /// Shuffles the list randomly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            // basically swapping between two element in each iteration
            for (int i = 0; i < list.Count; i++)
            {
                int k = Random.Range(0, list.Count);
                T value = list[k];
                list[k] = list[i];
                list[i] = value;
            }
        }

        /// <summary>
        /// Returns a random item in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T RandomItem<T>(this IList<T> list)
        {
            // basically swapping between two element in each iteration
            return list[Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Returns a random item in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="removeItem">If set to true, the selected item is removed from the list.</param>
        /// <returns></returns>
        public static T RandomItem<T>(this IList<T> list, bool removeItem)
        {
            T selectedItem = list[Random.Range(0, list.Count)];

            if (removeItem)
            {
                list.Remove(selectedItem);
            }

            return selectedItem;
        }

        public static List<T> ArrayToList<T>(T[] array)
        {
            List<T> list = new List<T>();

            for (int i = 0; i < array.Length; i++)
            {
                list.Add(array[i]);
            }

            return list;
        }

        public static List<T> Duplicate<T>(this List<T> list)
        {
            List<T> newList = new List<T>();
            list.ForEach(i => newList.Add(i));
            return newList;
        }
        #endregion

        /// <summary>
        /// Colors the string value based on the color selected.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="_color">The new color of the string.</param>
        /// <returns></returns>
        public static string Color(this string text, Color _color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(_color)}>{text}</color>";
        }

        /// <summary>
        /// Returns the tag of the opposite side of the given tag.
        /// </summary>
        /// <param name="tag">The tag of the requester</param>
        /// <returns></returns>
        public static string GetOpposingTag(string tag)
        {
            return tag == TanksTag.Player.ToString() ? TanksTag.Enemy.ToString() : TanksTag.Player.ToString();
        }

        public static int AddInRange(int value, int valueToAdd, int minRange, int maxRange)
        {
            value += valueToAdd;

            if (value < minRange)
            {
                value = maxRange;
            }
            else if (value > maxRange)
            {
                value = minRange;
            }
            else
            {
                value = Mathf.Clamp(value, minRange, maxRange);
            }

            return value;
        }

        public static int AddInRange(int value, int valueToAdd, int maxRange)
        {
            int minRange = 0;
            value += valueToAdd;

            if (value < minRange)
            {
                value = maxRange;
            }
            else if (value > maxRange)
            {
                value = minRange;
            }
            else
            {
                value = Mathf.Clamp(value, minRange, maxRange);
            }

            return value;
        }

        public static Vector3 GetMouseWorldPosition(LayerMask mouseColliderLayerMask)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 999f, mouseColliderLayerMask))
            {
                return hit.point;
            }
            else
            {
                return Vector3.zero;
            }
        }

        public static Vector3 GetRandomPointInsideSphere(Vector3 center, float radius)
        {
            return center + Random.insideUnitSphere * radius;            
        }

        public static Vector3 GetRandomPointInsideCube(Vector3 center, Vector3 size)
        {
            Vector3 randomPoint = new Vector3(
            Random.Range(-size.x / 2f, size.x / 2f),
            Random.Range(-size.y / 2f, size.y / 2f),
            Random.Range(-size.z / 2f, size.z / 2f)
        );

            return center + randomPoint;
        }

        public static bool IsPointInsideSphere(Vector3 point, Vector3 center, float radius)
        {
            // Calculate the distance between the point and the center of the sphere
            float distance = Vector3.Distance(point, center);

            // Check if the distance is less than or equal to the radius of the sphere
            return distance <= radius;
        }

        public static string GetInputIcon(int index)
        {
            return $"<sprite={index}>";
        }

        public struct SimpleSpline
        {
            private Vector3 _a0, _a1, _a2;

            public SimpleSpline(Vector3 a0, Vector3 a1, Vector3 a2)
            {
                _a0 = a0;
                _a1 = a1;
                _a2 = a2;
            }

            public Vector3 Evaluate(float t)
            {
                return Mathf.Pow(t, 2f) * (_a2 - 2f * _a1 + _a0) + t * (2f * _a1 - 2f * _a0) + _a0;
            }

            public Vector3 EvaluateTangent(float t)
            {
                return 2f * t * (_a2 - 2f * _a1 + _a0) + (2f * _a1 - 2f * _a0);
            }
        }
    }
}

public static class GlobalHelper
{
    public static void StopCoroutineSafe(this MonoBehaviour monoBehavior, Coroutine coroutine)
    {
        if(coroutine == null)
        {
            return;
        }

        monoBehavior.StopCoroutine(coroutine);
    }

    public static void PlayAnimation(this MonoBehaviour monoBehavior, Animation animation, AnimationClip clip)
    {
        if(animation == null || clip == null)
        {
            return;
        }

        if (animation.isPlaying)
        {
            animation.Stop();
        }

        animation.clip = clip;
        animation.Play();
    }

    public static void PlayAnimation(this MonoBehaviour monoBehavior, Animation animation)
    {
        if (animation == null)
        {
            return;
        }

        if (animation.isPlaying)
        {
            animation.Stop();
        }

        animation.Play();
    }
}
