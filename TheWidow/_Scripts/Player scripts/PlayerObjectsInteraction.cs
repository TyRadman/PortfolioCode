using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

// the class that allows the player to hold and observe items before picking them up
public class PlayerObjectsInteraction : MonoBehaviour
{
    public static PlayerObjectsInteraction Instance;
    private Transform m_Cam;
    [SerializeField] private float m_MovingSpeed = 1f;
    [SerializeField] private float m_DistanceFromCamera = 1f;
    private Vector3 m_PreviousPosition;
    private Quaternion m_PreviousRotation;
    private Transform m_CurrentObject;

    private bool m_Holding = false;
    [HideInInspector] public bool Holding
    {
        get
        {
            return m_Holding;
        }
        set
        {
            m_Holding = value;

            if (value)                          // if we're holding an item
            {
                StartCoroutine(receiveInput()); // then we recieve input that will return it back to place
            }
        }
    }

    [SerializeField] private float m_VolumeSwitchingSpeed = 0.7f;
    [Header("UI variables")]
    [SerializeField] private Text m_InfoText;
    [SerializeField] private Text m_TitleText;
    [SerializeField] private Animator m_TextAnim;
    private const string SHOW_CLIP_NAME = "Show";
    private const string HIDE_CLIP_NAME = "Hide";
    private const string SPEED_VAR_NAME = "Speed";

    private void Awake()
    {
        Instance = this;
        m_Cam = Camera.main.transform;
        m_TextAnim.SetFloat(SPEED_VAR_NAME, m_MovingSpeed * 2f);
    }

    private IEnumerator receiveInput()
    {
        while (Holding)
        {
            if (PauseMenuManager.IsPaused)
            {
                continue;
            }

            m_CurrentObject.Rotate(new Vector3(-Input.GetAxisRaw("Mouse Y"), -Input.GetAxisRaw("Mouse X"), 0f));

            if (Input.GetKeyDown(KeyCode.E))// && Holding == true)            // the next interaction for the object currently observed
            {
                ReleaseObservedObject();
            }

            yield return null;
        }
    }

    private void ReleaseObservedObject()
    {
        Holding = false;
        var observed = m_CurrentObject.GetComponent<I_ObservedObject>();
        PlayerMovement.Instance.AllowMovement(true);
        InteractionClass.CanInteract = true;
        observed.ItemHeld = false;
        // StartCoroutine(switchVolume(false));     // should be replaced with modifications the existing post processing volume
        m_TextAnim.SetTrigger(HIDE_CLIP_NAME);
        observed.AfterObservationInteraction();
    }

    public void ReleaseObject()
    {
        if (m_CurrentObject != null)        // in case the last object we observed was taken by the player, therefore, destroyed in code
        {
            Observe(m_CurrentObject, true);
        }
    }

    public void Observe(Transform _obj, bool _held, float _distance = 0f)
    {
        PlayerMovement.Instance.AllowMovement(_held);
        TipsManager.Instance.CloseWindow();             // if there was a tip displayed, close it
        TipsManager.Instance.StopTipsDelayedShow();

        if (_held)  // releasing control over the object to avoid modifying its axis as it goes back to its previous position
        {
            Holding = false;
        }
        else       // if the item is held then its info will be display 
        {
            _obj.GetComponent<I_ObservedObject>().Info.SetUp(m_TitleText, m_InfoText);
        }

        m_CurrentObject = _obj;
        m_TextAnim.SetTrigger(_held ? HIDE_CLIP_NAME : SHOW_CLIP_NAME);

        if (_distance == 0f)
        {
            _distance = m_DistanceFromCamera;
        }

        StartCoroutine(changePositionAndRotation(_held, _distance));
    }


    private IEnumerator changePositionAndRotation(bool _held, float _distance)
    {
        // rotation adjustments
        Quaternion newRot;

        if (!_held)
        {
            m_PreviousRotation = m_CurrentObject.rotation;
            m_CurrentObject.LookAt(m_Cam);
            newRot = m_CurrentObject.rotation;
            m_CurrentObject.rotation = m_PreviousRotation;
        }
        else
        {
            newRot = m_PreviousRotation;
        }

        // position adjustments
        var newPos = _held ? m_PreviousPosition : m_Cam.position + m_Cam.forward * _distance;
        m_PreviousPosition = _held ? m_PreviousPosition : m_CurrentObject.position;

        var time = 0f;

        while (time < m_MovingSpeed)
        {
            time += Time.deltaTime;
            var t = time / m_MovingSpeed;
            // rotating
            m_CurrentObject.rotation = Quaternion.Lerp(m_CurrentObject.rotation, newRot, t);
            // moving
            m_CurrentObject.position = Vector3.Lerp(m_CurrentObject.position, newPos, t);

            yield return null;
        }

        Holding = !_held;

        if (!Holding)       // once we're not holding an object, every tip that was not fully displayed is displayed
        {
            TipsManager.Instance.CheckStackingTips();
        }
    }
}