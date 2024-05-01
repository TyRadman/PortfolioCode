using UnityEngine;

[System.Serializable]
public class ItemInfo
{
    [SerializeField] private string m_Title;
    [SerializeField] [TextArea(2, 20)] private string m_Info;

    public void SetUp(UnityEngine.UI.Text _title, UnityEngine.UI.Text _info)
    {
        _title.text = m_Title;
        _info.text = m_Info;
    }
}