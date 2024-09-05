using System.Collections;
using UnityEngine;
using UnityEngine.UI;
// the real script
public class EyeIndicator : MonoBehaviour
{
    //public static EyeIndicator Instance;
    [Header("References")]
    [SerializeField] private RectTransform m_EyeTransform;
    [SerializeField] private GameObject m_HearingIcon;
    public Animator Anim_vectorEye;
    private Coroutine m_DisableHearingIcon;
    [SerializeField] private float m_MaxDistance, m_MinDistance;
    [SerializeField] private float m_MinSize;
    [SerializeField] private float m_MaxSize;
    private bool m_TakeInputs = true;
    private const float HEARING_ICON_DISPLAY_DURATION = 1f;
    private const float EYE_SCALING_SPEED = 0.3f;

    private void Awake()
    {
        m_HearingIcon.SetActive(false);
    }

    #region Sight
    public void UpdateIdicator(EyeState _action)
    {
        int actionNumber = (int)_action;

        if(actionNumber > 1)
        {
            Anim_vectorEye.SetTrigger(_action.ToString());
        }
        else
        {
            Anim_vectorEye.SetBool(EyeState.IsPlayerInSight.ToString(), actionNumber == 0);
        }
    }

    public void SetEyeIndicatorSize(float _distanceToPlayer)
    {
        if (!m_TakeInputs)
        {
            return;
        }

        float pct = Mathf.InverseLerp(m_MaxDistance, m_MinDistance, _distanceToPlayer);
        float size = Mathf.Lerp(m_MinSize, m_MaxSize, pct);
        float newSize = Mathf.Lerp(m_EyeTransform.localScale.x, size, EYE_SCALING_SPEED);
        m_EyeTransform.localScale = Vector3.one * newSize;
    }

    public void EnableIsChasing(bool enable)
    {
        Anim_vectorEye.SetBool(EyeState.IsPlayerInSight.ToString(), enable);
    }
    #endregion

    #region Hearing 
    public void DisplayHearingIcon(bool display)
    {
        if (display)
        {
            CancelInvoke();
            m_HearingIcon.SetActive(true);
            Invoke(nameof(HideHearingIcon), HEARING_ICON_DISPLAY_DURATION);
        }
        else
        {
            m_HearingIcon.SetActive(false);
        }
    }

    private void HideHearingIcon()
    {
        DisplayHearingIcon(false);
    }

    #endregion
}

public enum EyeState
{
    IsPlayerInSight = 0, IsPlayerOutOfSight = 1, OnPlayerSpotted = 2,
    OnPlayerLost = 3, OnEyeDisabled = 4, OnEyeAppeared = 5, OnPlayerHidden = 6,
    OnPlayerUnhidden = 7, OnEyeEnabled = 8
}
