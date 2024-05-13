using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.InGame
{
    public class SegmentedBar : MonoBehaviour
    {
        [System.Serializable]
        public class BarLayer
        {
            public SpriteRenderer Sprite;
            [HideInInspector] public Material Material;
            public bool Fill;
        }

        [SerializeField] private List<BarLayer> _bars;
        private const string LINE_WIDTH = "_LineWidth";
        private const string COLOR = "_Color";
        private const string ROTATION = "_Rotation";
        private const string VALUE = "_Value";
        private const string SPACING = "_SegmentSpacing";
        private const string COUNT = "_SegmentCount";
        private const string ALPHA = "_Alpha";
        private int _segmentsCount = 0;

        public void SetUp()
        {
            _bars.ForEach(b => b.Material = b.Sprite.material);
            _segmentsCount = _bars[0].Material.GetInt(COUNT);
        }

        public void SetTotalAmount(float amount)
        {
            List<BarLayer> bars = _bars.FindAll(b => b.Fill);
            bars.ForEach(b => b.Material.SetFloat(VALUE, amount));

            if (bars[0].Material.GetFloat(VALUE) == 0f)
            {
                bars.ForEach(b => b.Material.SetFloat(ALPHA, 0));
            }
            else
            {
                bars.ForEach(b => b.Material.SetFloat(ALPHA, 1));
            }
        }

        public void AddAmountToSegments(float amount)
        {
            List<BarLayer> bars = _bars.FindAll(b => b.Fill);
            amount /= _segmentsCount;
            float newAmount = bars[0].Material.GetFloat(VALUE) + amount;
            bars.ForEach(b => b.Material.SetFloat(VALUE, b.Material.GetFloat(VALUE) + amount));

            if (bars[0].Material.GetFloat(VALUE) == 0f)
            {
                bars.ForEach(b => b.Material.SetFloat(ALPHA, 0));
            }
            else
            {
                bars.ForEach(b => b.Material.SetFloat(ALPHA, 1));
            }
        }

        public float GetAmount()
        {
            return _bars.Find(b => b.Fill).Material.GetFloat(VALUE);
        }

        public void SetCount(int barsCount)
        {
            _bars.ForEach(b => b.Material.SetInt(COUNT, barsCount));
            _segmentsCount = _bars[0].Material.GetInt(COUNT);
        }
    }
}
