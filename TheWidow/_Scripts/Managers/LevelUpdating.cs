using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


// a class that updates the level depending on certain events that call functions from this class
public class LevelUpdating : MonoBehaviour
{
    [System.Serializable]
    private class BottleCollectionLevelUpdate
    {
        public UnityEvent OnNumberOfBottlesCollected;
        // how many medicine bottles must be picked to trigger this event
        public int BottlesRequired;
        // what message/s should be played when this event is triggered
        [TextArea]
        public string[] Message;
    }

    [SerializeField] private List<BottleCollectionLevelUpdate> m_BottleCollectionLevelUpdates;

    private void Start()
    {
        TriggerUpdate(0);
        PlayerStats.Instance.OnMedicineCollected += CheckForUpdates;              // whenever a medicine is collected, the function is called
    }

    void CheckForUpdates()
    {
        // everytime a bottle is picked, this function checks if there are any events accociated with this number of bottles
        if (m_BottleCollectionLevelUpdates.Exists(up => up.BottlesRequired == PlayerStats.Instance.MedicinesCollected))
        {
            TriggerUpdate(PlayerStats.Instance.MedicinesCollected);
        }
    }

    private void TriggerUpdate(int _numberOfBottles)
    {
        if (!m_BottleCollectionLevelUpdates.Exists(up => up.BottlesRequired == _numberOfBottles))
        {
            return;
        }

        List<BottleCollectionLevelUpdate> updates = m_BottleCollectionLevelUpdates.FindAll(up => up.BottlesRequired == _numberOfBottles);  // caches all pieces that have this number of medicine required
        updates.ForEach(i => i.OnNumberOfBottlesCollected?.Invoke());
        updates.ForEach(i => StartCoroutine(DisplayMessages(i.Message)));
    }

    private IEnumerator DisplayMessages(string[] _messages)           // can be used by external classes other than the game manager
    {
        if (_messages[0] != "" || _messages.Length > 0)     // otherwise just skip the coroutine
        {
            int index = 0;
            var wait = new WaitForSeconds(0.1f);

            while (index < _messages.Length)
            {
                if (DialogueManager.Instance.Free)
                {
                    DialogueManager.Instance.TypeMessage(_messages[index++]);
                }

                yield return wait;
            }
        }
    }
}