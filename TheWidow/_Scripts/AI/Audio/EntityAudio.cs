using System.Collections.Generic;
using UnityEngine;

public class EntityAudio : MonoBehaviour
{
    public struct AudioState
    {
        public MachineState.StateName State;
        public string[] AudioNames;
        // time between two audios
        public float MinRestTime;
        public float MaxRestTime;
    }

    [SerializeField] private List<AudioState> m_Audios = new List<AudioState>();

    [HideInInspector] public bool IsAvailable = true;

    public void PlayAudio(MachineState.StateName _state, int _index = -1, bool _interrupt = false)
    {
        if(!IsAvailable && !_interrupt)
        {
            return;
        }

        // if there was no index specified as a parament then we choose randomly
        int selectedIndex = _index == -1 ? Random.Range(0, m_Audios.Count) : _index;
        // select an audio set that has the selected state
        AudioState selectedAudioSet = m_Audios.Find(a => a.State == _state);

        // debugging stuff
        if(selectedAudioSet.AudioNames.Length == 0)
        {
            print($"No audios with a {_state} state");
        }

        // play the selected audio
        AudioManager.Instance.PlayAudio(selectedAudioSet.AudioNames[selectedIndex], null, true);
    }
}