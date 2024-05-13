using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;
using TankLike.LevelGeneration.Quests;
using static TankLike.UI.Notifications.QuestNotificationBar;

namespace TankLike
{
    public class QuestsManager : MonoBehaviour
    {
        [SerializeField] private LevelQuestSettings _settings;
        [SerializeField] private List<Quest_SO> _questTypes;
        [SerializeField] private List<Quest_SO> _activeQuests;
        [SerializeField] private int _questsCount = 3;
        public System.Action<Quest_SO> OnQuestAdded;
        public System.Action<Quest_SO> OnQuestCompleted;
        public System.Action<Quest_SO> OnQuestProgressed;

        public void SetUp()
        {
            Invoke(nameof(CreateQuests), 1f);
        }

        public void CreateQuests()
        {
            for (int i = 0; i < _questTypes.Count; i++)
            {
                AddQuest(_questTypes[i]);
            }
        }

        private void AddQuest(Quest_SO quest)
        {
            Quest_SO newQuest = (Quest_SO)ScriptableObject.CreateInstance(quest.GetType());
            // copies the values of the cached quest to the newly created one
            quest.CopyValuesTo(newQuest);
            _activeQuests.Add(newQuest);
            // set up the quest type
            newQuest.SetUp(_settings);
            // show it in the notifications
            GameManager.Instance.NotificationsManager.PushQuestNotification(newQuest.GetQuestString(), QuestState.Started);
            // invoke the event for the quest navigator (inventory) to add the quest as well
            OnQuestAdded?.Invoke(newQuest);
        }

        public void ReportProgress(Quest_SO quest)
        {
            OnQuestProgressed(quest);
        }

        public void MarkQuestAsCompleted(Quest_SO quest)
        {
            quest.IsActive = false;
            // display the completion notificaiton
            GameManager.Instance.NotificationsManager.PushQuestNotification(quest.GetQuestString(), QuestState.Finished);
            // invoke any subscribers
            OnQuestCompleted?.Invoke(quest);
        }

        public List<Quest_SO> GetQuests()
        {
            return _activeQuests;
        }
    }

    public enum QuestType
    {
        DestroyEnemies = 0, CollectCoins = 1
    }
}
