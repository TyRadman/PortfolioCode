using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class IndicatorCurve : MonoBehaviour
    {
        [SerializeField] private LineRenderer _line;
        [SerializeField] private float _height;
        [SerializeField] private int _levelOfDetail = 10;
        [SerializeField] private AnimationCurve _curve;

        public void EnableLine(bool enable)
        {
            _line.enabled = enable;
        }

        public void UpdateCurve(Vector3 startPoint, Vector3 endPoint)
        {
            _line.positionCount = _levelOfDetail;

            for (int i = 0; i < _levelOfDetail; i++)
            {
                float t = (float)i / ((float)_levelOfDetail - 1f);
                float x = Mathf.Lerp(startPoint.x, endPoint.x, t);
                float z = Mathf.Lerp(startPoint.z, endPoint.z, t);
                Vector3 position = new Vector3(x, _curve.Evaluate(t) * _height, z);
                _line.SetPosition(i, position);
            }
        }
    }
}
