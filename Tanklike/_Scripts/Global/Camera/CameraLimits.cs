using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Cam
{
    [System.Serializable]
    public class CameraLimits
    {
        public Vector2 HorizontalLimits;
        public Vector2 VerticalLimits;

        public void AddOffset(Vector3 offset)
        {
            HorizontalLimits.x += offset.x;
            HorizontalLimits.y += offset.x;
            VerticalLimits.x += offset.z;
            VerticalLimits.y += offset.z;
        }

        public void SetValues(CameraLimits limits)
        {
            HorizontalLimits = limits.HorizontalLimits;
            VerticalLimits = limits.VerticalLimits;
        }

        public void AddValues(CameraLimits limits)
        {
            HorizontalLimits.x += limits.HorizontalLimits.x;
            HorizontalLimits.y -= limits.HorizontalLimits.y;
            VerticalLimits.x += limits.VerticalLimits.x;
            VerticalLimits.y -= limits.VerticalLimits.y;
        }

        public void ScaleUpValues(CameraLimits originalLimits, CameraLimits offsetLimits, float multiplier)
        {
            // set the original limts, the ones of the room's position
            HorizontalLimits = originalLimits.HorizontalLimits;
            VerticalLimits = originalLimits.VerticalLimits;

            // add the offset and apply the multiplier to them. The higher the multiplier value, the smaller the offset should get
            HorizontalLimits.x += offsetLimits.HorizontalLimits.x * multiplier;
            HorizontalLimits.y -= offsetLimits.HorizontalLimits.y * multiplier;
            VerticalLimits.x += offsetLimits.VerticalLimits.x * multiplier;
            VerticalLimits.y -= offsetLimits.VerticalLimits.y * multiplier;

            if(HorizontalLimits.x > HorizontalLimits.y)
            {
                float middlePoint = (HorizontalLimits.x + HorizontalLimits.y) / 2f;
                HorizontalLimits = Vector2.one * middlePoint;
            }

            if (VerticalLimits.x > VerticalLimits.y)
            {
                float middlePoint = (VerticalLimits.x + VerticalLimits.y) / 2f;
                VerticalLimits = Vector2.one * middlePoint;
            }
        }
    }
}
