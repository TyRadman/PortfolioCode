using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TankLike.UI.Notifications.QuestNotificationBar;

namespace TankLike.UI.Notifications
{
    public class NotificationsManager : MonoBehaviour
    {

        [System.Serializable]
        public class QuestTask
        {
            public string Message;
            public QuestState State;
        }

        [SerializeField] private List<NotificationBarSettings_SO> _notificationSettings;
        [Header("Quests")]
        [SerializeField] private QuestNotificationBar _questBar;
        private const float QUEST_ON_DISPLAY_SHORT_DURATION = 1f;
        private const float QUEST_ON_DISPLAY_LONG_DURATION = 3f;
        private const float QUEST_HIDING_DURATION = 1f;
        private WaitForSeconds _questShortWait;
        private WaitForSeconds _questLongWait;
        private WaitForSeconds _questHideWait;
        [SerializeField] private List<PlayerNotificationController> _playerNotifications;
        [SerializeField] private List<QuestTask> _questTasks = new List<QuestTask>();

        private void Awake()
        {
            _questShortWait = new WaitForSeconds(QUEST_ON_DISPLAY_SHORT_DURATION);
            _questLongWait = new WaitForSeconds(QUEST_ON_DISPLAY_LONG_DURATION);
            _questHideWait = new WaitForSeconds(QUEST_HIDING_DURATION);
        }

        public void SetUp()
        {
            _playerNotifications.ForEach(p => p.SetUp());
        }

        public void PushCollectionNotification(NotificationType notificationType, int amount, int playerIndex)
        {
            NotificationBarSettings_SO settings = _notificationSettings.Find(s => s.Type == notificationType);
            _playerNotifications[playerIndex].PushCollectionNotification(notificationType, amount, settings);
        }

        #region Quest Notification
        public void PushQuestNotification(string _message, QuestState type)
        {
            QuestTask task = new QuestTask() { Message = _message, State = type };
            _questTasks.Add(task);

            if (_questTasks.Count == 1)
            {
                StartCoroutine(PerformingTaskProcess(task));
            }
        }

        private IEnumerator PerformingTaskProcess(QuestTask task)
        {
            _questBar.Display(task.Message, task.State);

            yield return new WaitForEndOfFrame();

            // if this is the last task, then display it twice as long, otherwise, make it short
            if (_questTasks.Count == 1) yield return _questLongWait;
            else yield return _questShortWait;

            _questBar.Hide();
            yield return _questHideWait;
            // remove the task from the task list
            _questTasks.Remove(task);

            if (_questTasks.Count > 0)
            {
                StartCoroutine(PerformingTaskProcess(_questTasks[0]));
            }
        }
        #endregion
    }

    public enum NotificationType
    {
        Coins = 0, Gems = 1, Tool = 2, NewQuest = 3, CompletedQuest = 4, Ammo = 5, Health = 6, BossKey
    }
}