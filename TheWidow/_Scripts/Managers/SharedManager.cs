using UnityEngine;

public class SharedManager : MonoBehaviour
{
    [TextArea]
    //public string[] GameTips;
    //public Sprite[] GameTipsImages;
    static public SharedManager Instance;
    [SerializeField] private Tips.Tip[] m_Tips;

    void Start()
    {
        Instance = this;
    }

    public Tips.Tip GetRandomTip()
    {
        return m_Tips[Random.Range(0, m_Tips.Length)];
    }

    public int GetTipsNum()
    {
        return m_Tips.Length;
    }

    public Tips.Tip GetTipWithIndex(int _index)
    {
        return m_Tips[_index];
    }
}