using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// a class that displays a small arrow on the object the player is pointing at indicating that interaction is possible
public class InteractionArrow : MonoBehaviour
{
    public static InteractionArrow Instance;
    [SerializeField] private RectTransform m_Arrow;
    private Image m_ArrowGraphics;
    private Camera m_Camera;

    private void Awake()
    {
        Instance = this;
        m_Camera = Camera.main;
        m_ArrowGraphics = m_Arrow.GetComponent<Image>();
        m_ArrowGraphics.enabled = false;
    }

    public static void DisplayArrow()
    {
        Instance.StartCoroutine(Instance.updateArrow());
    }

    private IEnumerator updateArrow()
    {
        Transform pos;
        m_ArrowGraphics.enabled = true;

        while (true)
        {
            pos = InteractionClass.GetActiveInteractable();

            if(pos == null || PlayerObjectsInteraction.Instance.Holding)
            {
                break;
            }

            m_Arrow.position = m_Camera.WorldToScreenPoint(pos.position);
            yield return null;
        }

        m_ArrowGraphics.enabled = false;
    }
}
