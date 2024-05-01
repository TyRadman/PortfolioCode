using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    #region Variables
    public static HUDManager Instance;
    // constants or the names of parameters of the animator
    private const string ANIMATION_SPEED = "MovementSpeed";
    private const string MOVEMENT_BOOL = "MoveDown";
    private const string BRAIN_PIECES_SHINE = "Shine";

    [Header("Bars variables")]
    [SerializeField] private Animator m_BarsAnim;
    [SerializeField] private float m_BarsDisplayDuration = 4f;  // main like stamina and battery
    [SerializeField] private float m_BarsMovementSpeed = 1f;

    [Header("Brain variables")]
    [SerializeField] private Image[] m_BrainPieces;
    [SerializeField] private Animator m_BrainPiecesAnim;
    [SerializeField] private float m_MedDisplayDuration = 4f;    // med as medicine
    [SerializeField] private float m_PieceFadingInSpeed = 0.1f;
    [SerializeField] private float m_BrainPiecesMovementSpeed = 1f;
    private int m_CurrentIndex = 0;
    #endregion

    private void Start()
    {
        Instance = this;
        m_BrainPiecesAnim.SetFloat(ANIMATION_SPEED, m_BrainPiecesMovementSpeed);   // sets the speed of the transition
        m_BarsAnim.SetFloat(ANIMATION_SPEED, m_BarsMovementSpeed);
    }

    #region Main HUD functionality
    public void ToggleBars(bool _show)
    {
        if (_show)
        {
            showStaminaHUD();
        }
        else
        {
            hideBars();
        }
    }

    private void showStaminaHUD()
    {
        m_BarsAnim.SetBool(MOVEMENT_BOOL, true);
        CancelInvoke(nameof(hideBars));

        // if the stamina is full and the flashlight isn't on then there's no need to show the bars that display these two values, so we remove the bars after a while
        if (!PlayerActions.Instance.FlashLightOn && PlayerStats.Instance.StaminaIsFull && PlayerLife.Instance.HealthIsAboveRefill())
        {
            Invoke(nameof(hideBars), m_BarsDisplayDuration);
        }
    }

    private void hideBars()
    {
        if (!PlayerActions.Instance.FlashLightOn && PlayerStats.Instance.StaminaIsFull)
        {
            m_BarsAnim.SetBool(MOVEMENT_BOOL, false);
        }
    }

    private void hide()
    {
        m_BarsAnim.SetBool(MOVEMENT_BOOL, false);
    }
    #endregion

    #region Medicine HUD functionality
    // everytime a medicine bottle is picked up, the brain pieces icon shines for a second
    // the following functions make that happen. Although this can be done with an animator, it feels more controlable when done via code
    public void ShowMedicinePieces()
    {
        StartCoroutine(brainPieceFadeIn(m_BrainPieces[m_CurrentIndex++]));

        m_BrainPiecesAnim.SetBool(MOVEMENT_BOOL, false);

        Invoke(nameof(moveBrainPiecesDown), m_MedDisplayDuration);
    }

    IEnumerator brainPieceFadeIn(Image _piece)
    {
        yield return new WaitForSeconds(0.5f);

        var col = _piece.color;
        m_BrainPiecesAnim.SetTrigger(BRAIN_PIECES_SHINE);
        AudioManager.Instance.PlayAudio("BrainPiece", transform.GetChild(0).GetComponent<AudioSource>(), true);

        while (_piece.color.a < .9f)
        {
            _piece.color = new Color(col.r, col.g, col.b, Mathf.Lerp(_piece.color.a, 0.9f, m_PieceFadingInSpeed));
            yield return null;
        }
    }

    private void moveBrainPiecesDown()
    {
        m_BrainPiecesAnim.SetBool(MOVEMENT_BOOL, true);
    }
    #endregion
}
