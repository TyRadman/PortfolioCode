using System.Collections;
using System.Collections.Generic;
using TankLike.Combat.Destructible;
using TankLike.LevelGeneration;
using UnityEngine;
using System.Linq;

namespace TankLike.Environment.LevelGeneration
{
    public class DestructiblesManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LevelDestructibleData_SO _currentDestructibleDropData;

        public void SetUp()
        {
            // set the highest drop chance for each dropper
            _currentDestructibleDropData.DropsData.ForEach(d => d.SetUp());
        }

        /// <summary>
        /// Sets the items that the dropper will drop when destroyed
        /// </summary>
        /// <param name="dropper"></param>
        public void SetDestructibleValues(IDropper dropper)
        {
            // retreieve the data based on the tag
            DestructibleDrop selectedTagCollectables = _currentDestructibleDropData.DropsData.Find(d => d.Tag == dropper.Tag);
            // set the drops tags to the destructible just build
            dropper.SetCollectablesToSpawn(selectedTagCollectables);
        }

        public LevelDestructibleData_SO GetDestructibleDropData()
        {
            return _currentDestructibleDropData;
        }
    }
}
