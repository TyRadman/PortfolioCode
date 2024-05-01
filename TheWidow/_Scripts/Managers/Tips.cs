using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// holds the data of all tips that will be displayed during the game
public class Tips : MonoBehaviour
{
    public static Tips Instance;
    [SerializeField] private float m_TipDuration = 4f;
    public enum TipsTag
    {
        Movement, Interaction, FlashLight, Inventory, Hiding, Tips, Escape, EnemyHearing
    }

    [System.Serializable]
    public class Tip
    {
        public string Title;
        public TipsTag Tag;
        [TextArea(3,20)] public string Info;
        public Sprite Image;
        [HideInInspector] public float Time;
    }

    [SerializeField] private List<Tip> m_Tips = new List<Tip>();
    private List<Tip> m_UnlockedTips = new List<Tip>();
    [HideInInspector] public bool TipsEnabled = true; 

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UnlockTips(TipsTag.Tips);
    }

    private void OnValidate()
    {
        m_Tips.ForEach(t => t.Title = t.Tag.ToString());
        m_Tips.ForEach(t => t.Time = m_TipDuration);
    }

    public void CopyTips(List<Tip> _list)           // feeds the information from this class to the one that feeds the tips UI
    {
        for (int i = 0; i < m_Tips.Count; i++)
        {
            _list.Add(m_Tips[i]);
        }
    }

    public void UnlockTips(TipsTag _tag)
    {
        var newTip = m_Tips.Find(t => t.Tag == _tag);

        if (!m_UnlockedTips.Contains(newTip))       // to avoid unlocking a tip twice in case it was called twice
        {
            UITipsManager.Instance.SetUpText(newTip);
            m_UnlockedTips.Add(newTip);
        }
    }

    public Tip GetTip(int _index)
    {
        return m_UnlockedTips[_index];
    }

    public int NumberOfTips()
    {
        return m_UnlockedTips.Count;
    }
}