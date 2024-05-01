using System.Collections;
using UnityEngine;


// a class that updates the level depending on certain events that call functions from this class
public class LevelUpdating : MonoBehaviour
{
    [System.Serializable]
    private struct PieceToShow
    {
        // objects to show once the event is triggered
        public GameObject[] ShowedPiece;
        // objects to hide once the event is triggered
        public GameObject[] HiddenPiece;
        // how many medicine bottles must be picked to trigger this event
        public int BottlesRequired;
        // what message/s should be played when this event is triggered
        [TextArea]
        public string[] Message;

        public void ShowPieces()
        {
            if (ShowedPiece != null)
            {
                System.Array.ForEach(ShowedPiece, p => p.SetActive(false));
            }
            if (HiddenPiece != null)
            {
                System.Array.ForEach(HiddenPiece, p => p.SetActive(true));
            }
        }
    }

    [SerializeField] private PieceToShow[] UpdatedPieces;

    private void Start()
    {
        enablePiece(0);
        PlayerStats.Instance.AddListenerToMedicineCollection(CheckForUpdates);              // whenever a medicine is collected, the function is called
    }

    void CheckForUpdates()
    {
        // everytime a bottle is picked, this function checks if there are any events accociated with this number of bottles
        if (System.Array.Exists(UpdatedPieces, up => up.BottlesRequired == PlayerStats.Instance.MedicinesCollected))
        {
            enablePiece(PlayerStats.Instance.MedicinesCollected);
        }
    }

    void enablePiece(int _numberOfBottles)
    {
        if (!System.Array.Exists(UpdatedPieces, up => up.BottlesRequired == _numberOfBottles))
        {
            return;
        }

        var items = System.Array.FindAll(UpdatedPieces, up => up.BottlesRequired == _numberOfBottles);  // caches all pieces that have this number of medicine required
        System.Array.ForEach(items, i => i.ShowPieces());                               // shows all pieces
        System.Array.ForEach(items, i => StartCoroutine(DisplayMessages(i.Message)));   // displays the messages of all pieces
    }

    IEnumerator DisplayMessages(string[] _messages)           // can be used by external classes other than the game manager
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