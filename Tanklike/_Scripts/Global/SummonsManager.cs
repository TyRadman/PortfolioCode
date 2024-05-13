using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;

namespace TankLike
{
    public class SummonsManager : MonoBehaviour
    {
        private List<SummonAIController> _activeSummons = new List<SummonAIController>();

        public void AddSummon(SummonAIController summon)
        {
            if (!_activeSummons.Contains(summon))
                _activeSummons.Add(summon);
        }

        public void RemoveSummon(SummonAIController summon)
        {
            if (_activeSummons.Contains(summon))
                _activeSummons.Remove(summon);
        }

        public void SetUp()
        {

        }

        public SummonAIController GetSummon(int index)
        {
            return _activeSummons[index];
        }

        public int GetActiveSummonsCount()
        {
            return _activeSummons.Count;
        }
    }
}
