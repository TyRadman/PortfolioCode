using System.Collections.Generic;
using UnityEngine;

// the puzzle where keys of a piano must be played in a correct order to unlock a box. The order can be found in a letter and is random on every run
public class TonesPuzzle : MonoBehaviour
{
    public static TonesPuzzle Instance;
    [SerializeField] private bool m_RandomSong = false;
    [SerializeField] private string m_CorrectTone;
    private List<string> m_Tones = new List<string>();
    [SerializeField] private int m_MinSongLength = 4;
    [SerializeField] private int m_MaxSongLength = 9;
    private readonly string m_PuzzleKeys = "abcdefghijklmn";
    [Header("References")]
    [SerializeField] private string m_SoundEffect;
    [SerializeField] private UnityEngine.UI.Text tone;
    [SerializeField] private Animator m_Anim;
    private int m_KeysNum;

    private void Awake()
    {
        Instance = this;
        m_KeysNum = transform.GetChild(2).childCount;

        m_CorrectTone = string.Empty;
        int length = Random.Range(m_MinSongLength, m_MaxSongLength);

        for (int i = 0; i < length; i++)
        {
            m_CorrectTone += (Random.Range(0, m_KeysNum) + 1).ToString();
        }

        var encryptedKeys = "";
        List<char> letters = new List<char>();
        List<char> fixedLetters = new List<char>();

        for (int i = 0; i < m_CorrectTone.Length; i++)
        {
            letters.Add(m_PuzzleKeys[i]);
            fixedLetters.Add(m_PuzzleKeys[i]);
        }


        for (int i = 0; i < m_CorrectTone.Length; i++)
        {
            var selectedLetter = letters[Random.Range(0, letters.Count)];
            encryptedKeys += "#" + selectedLetter + m_CorrectTone[fixedLetters.IndexOf(selectedLetter)] + " ";
            letters.Remove(selectedLetter);
        }

        // to simplify the process, if the correct keys order is 1234 we assign letters in accordance like so a1b2c3d4 and then we shuffle the pairs and print them on a letter like so #b2 #d4 #a1 #c3

        tone.text += encryptedKeys;
    }

    // the function that is played when a key of the piano is pressed
    public void PlayTone(string _tone)
    {
        m_Tones.Add(_tone);

        if (m_Tones.Count > m_CorrectTone.Length)
        {
            m_Tones.RemoveAt(0);
        }

        var totalTone = "";
        m_Tones.ForEach(t => totalTone += t);

        // if the sequence of tones played is correct then the box is opened
        if (totalTone == m_CorrectTone)
        {
            m_Anim.SetTrigger("Open");
            AudioManager.Instance.PlayAudio(m_SoundEffect, transform.GetChild(0).GetComponent<AudioSource>(), true);
            GetComponent<BoxCollider>().enabled = false;
            enabled = false;
        }
    }
}
