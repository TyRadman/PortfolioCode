using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Utils;
using TankLike.ItemsSystem;

namespace TankLike.LevelGeneration.Quests
{
    [CreateAssetMenu(fileName = "DestroyEnemiesQuest", menuName = "Other/ Quest Types/ Collect Collectables")]
    public class CollectCollectable_Quest : Quest_SO
    {
        [SerializeField] private Vector2Int _collectablesCountRange;
        private int _collectablesRequired;
        private int _currentlyCollectedItems;
        [Tooltip("If set to true, the value set for Collectable Tag will be ignored")]
        [SerializeField] private CollectableTag _collectableTag;

        public override void SetUp(LevelQuestSettings settings)
        {
            base.SetUp(settings);

            _currentlyCollectedItems = 0;
            _collectablesRequired = _collectablesCountRange.RandomValue();
            GameManager.Instance.ReportManager.OnCollectableCollected += OnProgress;
        }

        public void OnProgress(Collectable collectable, int playerIndex)
        {
            if (collectable.Tag != _collectableTag) return;

            _currentlyCollectedItems++;

            OnProgress();
        }

        public override string GetQuestString()
        {
            return $"Collect {_collectablesRequired.ToString().Color(Color.green)} {_collectableTag}{(_collectablesRequired > 1? "s" : "")}";
        }

        public override string GetQuestProgressString()
        {
            return $"{_currentlyCollectedItems} / {_collectablesRequired}";
        }

        public override float GetProgressAmount()
        {
            return (float)_currentlyCollectedItems / (float)_collectablesRequired;
        }

        public override bool IsCompleted()
        {
            return _currentlyCollectedItems >= _collectablesRequired;
        }

        public override void OnCompletion()
        {
            base.OnCompletion();

            GameManager.Instance.ReportManager.OnCollectableCollected -= OnProgress;
            GameManager.Instance.QuestsManager.MarkQuestAsCompleted(this);
        }

        #region Copying
        public override void CopyValuesTo(Quest_SO quest)
        {
            CollectCollectable_Quest newQuest = (CollectCollectable_Quest)(quest);
            newQuest.SetValues(_collectableTag, _collectablesCountRange);
        }

        public void SetValues(CollectableTag enemyType, Vector2Int itemsCountRange)
        {
            _collectableTag = enemyType;
            _collectablesCountRange = itemsCountRange;
        }
        #endregion
    }
}