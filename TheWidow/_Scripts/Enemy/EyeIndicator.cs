using System.Collections;
using UnityEngine;
using UnityEngine.UI;
// the real script
public class EyeIndicator : MonoBehaviour
{
    public static EyeIndicator Instance;
    [Header("References")]
    [SerializeField] private RectTransform m_EyeTransform;
    [SerializeField] private Image[] m_HearingIcon;
    public Animator Anim_vectorEye;
    private Coroutine m_DisableHearingIcon;
    [SerializeField] private float m_MaxDistance, m_MinDistance;
    [SerializeField] private float m_MinSize;
    [SerializeField] private float m_MaxSize;
    private bool m_TakeInputs = true;

    #region Constants
    private const string START_EYE = "startVectorEye";
    private const string NOTICING_PLAYER = "seenPlayer";
    private const string PLAYER_IN_SIGHT_CHASING= "playerInSight";
    private const string LOST_PLAYER= "playerLost";
    private const string EYE_VANISH = "EyeVanish";
    private const string EYE_APPEAR= "EyeAppear";
    private const string HIDE= "Hide";
    private const string UNHIDE = "Unhide";
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    //private void Update()
    //{
    //    print(PlayerStats.Instance.IsHidden + ", " + Time.time);
    //}

    public void UpdateIdicator(EyeState _action, bool _toggle = false)
    {
        if (!m_TakeInputs && !_toggle)
        {
            return;
        }

        if (_toggle)
        {
            m_TakeInputs = !m_TakeInputs;
        }

        //if (m_TakeInputs)
        //{
        //    SetEyeIndicatorSize(_distanceToPlayer);
        //}
        
        // playing different animations according to enemy state
        switch (_action)
        {
            case EyeState.StartLooking:
                {
                    Anim_vectorEye.SetTrigger(START_EYE);
                    break;
                }
            case EyeState.Noticing:
                {
                    Anim_vectorEye.SetTrigger(NOTICING_PLAYER);
                    break;
                }
            case EyeState.SeeingAndChasing:
                {
                    Anim_vectorEye.SetBool(PLAYER_IN_SIGHT_CHASING, false);
                    break;
                }
            case EyeState.LookingAndChasing:
                {
                    Anim_vectorEye.SetBool(PLAYER_IN_SIGHT_CHASING, true);
                    break;
                }
            case EyeState.Lost:
                {
                    Anim_vectorEye.SetTrigger(LOST_PLAYER);
                    break;
                }
            case EyeState.Vanish:
                {
                    Anim_vectorEye.SetTrigger(EYE_VANISH);
                    break;
                }
            case EyeState.Appear:
                {
                    Anim_vectorEye.SetTrigger(EYE_APPEAR);
                    break;
                }
            case EyeState.Hiding:
                {
                    Anim_vectorEye.SetTrigger(HIDE);
                    break;
                }
            case EyeState.OutOfHiding:
                {
                    Anim_vectorEye.SetTrigger(UNHIDE);
                    break;
                }
        }
    }

    public void SetEyeIndicatorSize(float _distanceToPlayer)
    {
        if (!m_TakeInputs)
        {
            return;
        }

            print($"Last distance is {_distanceToPlayer}");
        float pct = Mathf.InverseLerp(m_MaxDistance, m_MinDistance, _distanceToPlayer);
        m_EyeTransform.localScale = Vector3.one * Mathf.Lerp(m_MinSize, m_MaxSize, pct);
        // m_EyeTransform.localScale = Vector3.Lerp(m_EyeTransform.localScale, newSize, 0.1f);
    }

    #region Hearing 
    public void HearingIconAlert(bool heard)
    {
        if (heard)
        {
            if (m_DisableHearingIcon != null)
            {
                StopCoroutine(m_DisableHearingIcon);
            }

            enableHearingIcon(true); // display  the hearing icon
                                     // removing the hearing icon afterwards when the player is no longer making a noise
            m_DisableHearingIcon = StartCoroutine(disableHearingIcon());
        }
        else if (m_DisableHearingIcon != null)
        {
            m_DisableHearingIcon = null;
        }
    }

    IEnumerator disableHearingIcon()
    {
        yield return new WaitForSecondsRealtime(2f);
        enableHearingIcon(false);
    }

    void enableHearingIcon(bool _enable)
    {
        m_HearingIcon[0].enabled = _enable;
        m_HearingIcon[1].enabled = _enable;
    }
    #endregion
}

public enum EyeState
{
    StartLooking, Noticing, SeeingAndChasing, LookingAndChasing, Vanish, Appear, Lost, Hiding, OutOfHiding
}
