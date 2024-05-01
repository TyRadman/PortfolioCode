using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    public static PlayerMovement Instance;
    private CharacterController m_Controller;

    public enum States
    {
        crouching, standing, walking, running, hiding
    }

    private States _CurrentState = States.standing;

    public States CurrentState
    {
        get => _CurrentState;
        set
        {
            _CurrentState = value;

            switch (value)
            {
                case States.crouching:
                    {
                        Statistics.CrouchingTime += Time.deltaTime;
                        break;
                    }
                case States.walking:
                    {
                        Statistics.WalkingTime += Time.deltaTime;
                        break;
                    }
                case States.running:
                    {
                        Statistics.RunningTime += Time.deltaTime;
                        break;
                    }
                case States.standing:
                    {
                        Statistics.StandingTime += Time.deltaTime;
                        break;
                    }
            }

            E_PlayerStateOnChange(); // sends the message to the AI that the player's state has changed
        }
    }

    [Header("Speed Variables")]
    [SerializeField] private float m_WalkingSpeed = 1f;
    [SerializeField] private float m_RunningSpeed = 4f;
    [SerializeField] private float m_CrouchingSpeed = .5f;

    [Header("Extra Tweaks")]
    public float YawSensitivity = 1f;
    [SerializeField] private float m_CameraRotationLimit;
    [Tooltip("The speed in which the player's speed transits from walking speed to running speed.")]
    [SerializeField] private float m_BlendingSpeed = 0.1f;
    private float m_SpeedMultiplier = 0f;

    // player bools
    [HideInInspector] public bool IsWalking = false;
    [HideInInspector] public bool IsCrouching = false;
    private bool canCrouch = true;
    /// <summary>
    /// Determines whether the player can run
    /// </summary>
    [HideInInspector] public bool AllowedToRun = true;
    /// <summary>
    /// Determines whether the player can move around with keys
    /// </summary>
    [HideInInspector] public bool AllowedToMove = true;
    /// <summary>
    /// Determines whether the player can look around with the mouse
    /// </summary>
    [HideInInspector] public bool AllowedToLook = true;

    // crouching height floats
    private float m_CameraHeight;
    private float m_ControllerHeight;
    [Header("Crouching Variables")]
    public float m_CrouchingCameraHeight;
    public float m_CrouchingControllerHeight;
    [SerializeField] private float m_LerpingToCrouchSpeed = 0.4f;

    // basic movement variables
    [HideInInspector] public Transform CameraTransform;
    [HideInInspector] public Transform PlayerTransform;
    private Vector3 m_Velocity;
    private float m_CameraRotation;
    private float m_CurrentCameraRotation;
    private float m_BodyRotation;

    // events and delegates
    public delegate void Action();
    public event Action E_PlayerStateOnChange = delegate { };

    // audio variables
    [Header("FootStep audios")]
    [SerializeField] private float m_TimeBetweenWalkingSteps = 2f;
    [SerializeField] private float m_TimeBetweenRunningSteps;
    [SerializeField] private float m_TimeBetweenCrouchingSteps;
    private float m_StepsTimer;
    public AudioClip[] m_RunningFootstepSounds;
    public AudioClip[] m_WalkingFootstepSounds;
    public AudioClip[] m_CrouchingFootstepSounds;
    #endregion

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        PlayerTransform = transform;
        CameraTransform = Camera.main.transform;
        m_Controller = GetComponent<CharacterController>();
        m_CameraHeight = Camera.main.transform.localPosition.y;
        m_ControllerHeight = m_Controller.height;
    }

    #region Movement
    void Update()
    {
        if (!AllowedToMove)
        {
            return;
        }

        movementInput();
        rotationInput();
        crouchInput();
    }

    private void movementInput()
    {
        // calculate movement velocity
        var _xMov = Input.GetAxis("Horizontal");
        var _zMov = Input.GetAxis("Vertical");
        // playing audio
        if ((_xMov != 0 || _zMov != 0) && (m_Controller.velocity.magnitude > 0.5f || m_Controller.velocity.magnitude < -0.5f)) // determines whether to make footsteps audio or not
        {
            footSteps();
        }
        // getting the running input
        var _running = Input.GetKey(KeyCode.LeftShift) && !IsCrouching && AllowedToRun;
        // applying movement to the local directions of the player
        Vector3 _moveHor = transform.right * _xMov;
        Vector3 _movVer = transform.forward * _zMov;

        m_Velocity = (_moveHor + _movVer) * getMultiplier(_running, IsCrouching);

        // determines the state
        if (_running)
        {
            CurrentState = States.running;
            PlayerStats.Instance.ConsumeStamina();
        }
        else if (m_Velocity != Vector3.zero && !IsCrouching)
        {
            CurrentState = States.walking;
        }
        else if (m_Velocity != Vector3.zero && IsCrouching)
        {
            CurrentState = States.crouching;
        }
        else
        {
            CurrentState = States.standing;
        }
    }

    private void rotationInput()
    {
        if (!AllowedToLook)
        {
            return;
        }

        // calculate rotation X
        m_BodyRotation = Input.GetAxisRaw("Mouse X") * YawSensitivity;
        // calculate rotation Y
        m_CameraRotation = Input.GetAxisRaw("Mouse Y") * YawSensitivity;
    }

    private void crouchInput()
    {
        // read crouching input
        if (Input.GetKeyDown(KeyCode.C) && !PlayerStats.Instance.IsHidden && !Input.GetKey(KeyCode.LeftShift) && canCrouch && AllowedToMove)
        {
            toggleCrouch();
        }
    }

    /// <summary>
    /// returns a float that is multiplied by the movement speed. Helps gradually increase or decrease the player's speed to add realism.
    /// </summary>
    /// <param name="_running">Whether the play is running.</param>
    /// <param name="_crouching">Whether the player is crouching.</param>
    /// <returns></returns>
    float getMultiplier(bool _running, bool _crouching)
    {
        if (!_running && !_crouching && (m_SpeedMultiplier < m_WalkingSpeed - 0.05f || m_SpeedMultiplier > m_WalkingSpeed + 0.05f))
        {
            m_SpeedMultiplier = Mathf.Lerp(m_SpeedMultiplier, m_WalkingSpeed, m_BlendingSpeed);
        }
        else if (_running && m_SpeedMultiplier < m_RunningSpeed - 0.05f)
        {
            m_SpeedMultiplier = Mathf.Lerp(m_SpeedMultiplier, m_RunningSpeed, m_BlendingSpeed);
        }
        else if (_crouching)
        {
            m_SpeedMultiplier = m_CrouchingSpeed;
        }

        return m_SpeedMultiplier;
    }

    void FixedUpdate()
    {
        applyRotation();
        applyMovement();
    }

    void applyMovement()
    {
        // setting isWalking the same time we check for movement
        if (IsWalking = m_Velocity != Vector3.zero)                             
        {
            // the 70f is just a push for the speed (a constant)
            m_Controller.SimpleMove(m_Velocity * 70f * Time.fixedDeltaTime);    
        }
    }

    void applyRotation()
    {
        if (!AllowedToLook)
        {
            return;
        }

        m_CurrentCameraRotation += m_CameraRotation * -1; // to be changed soon for people that prefer it inversed
        m_CurrentCameraRotation = Mathf.Clamp(m_CurrentCameraRotation, -m_CameraRotationLimit, m_CameraRotationLimit);
        // up and down
        CameraTransform.localEulerAngles = new Vector3(m_CurrentCameraRotation, 0f, 0f);
        // left and right
        transform.localRotation = Quaternion.Euler(new Vector3(0f, m_BodyRotation, 0f) + transform.localRotation.eulerAngles);
    }

    void toggleCrouch()
    {
        IsCrouching = !IsCrouching;
     
        if (IsCrouching)
        {
            // gets player up
            ToggleCrouch(m_CrouchingControllerHeight, m_CrouchingCameraHeight);
        }
        else
        {
            // makes player crouch
            ToggleCrouch(m_ControllerHeight, m_CameraHeight);
        }
    }

    public void ToggleCrouch(float controllerHeight, float camHeight) // the function that changes the values to the new ones and switches them back again
    {
        canCrouch = false;
        StartCoroutine(lerpToPosition(controllerHeight, camHeight));  // move the player to new view
    }

    // related to crouching
    IEnumerator lerpToPosition(float controllerHeight, float camHeight)
    {
        while (true)
        {
            m_Controller.Move(new Vector3(0, 0.1f * (IsCrouching ? -1 : 1), 0));                    // to push the player down to the ground
            m_Controller.height = Mathf.Lerp(m_Controller.height, controllerHeight, m_LerpingToCrouchSpeed);
            var camPos = CameraTransform.transform.localPosition;
            var newYAxis = Mathf.Lerp(camPos.y, camHeight, m_LerpingToCrouchSpeed);
            CameraTransform.transform.localPosition = new Vector3(camPos.x, newYAxis, camPos.z);

            if (Mathf.Abs(newYAxis) - Mathf.Abs(camPos.y) < 0.1f && Mathf.Abs(m_Controller.height) - Mathf.Abs(controllerHeight) < 0.1f)
            {
                CameraTransform.transform.localPosition = new Vector3(camPos.x, camHeight, camPos.z);
                m_Controller.height = controllerHeight;
                m_Controller.Move(new Vector3(0, 0.1f * (IsCrouching ? -1 : 1), 0));                    // to push the player down to the ground
                break;
            }

            yield return null;
        }

        canCrouch = true;
    }
    #endregion

    #region Audios synchronization
    void footSteps()    // function that synchronizes foot steps audio with the movement
    {
        m_StepsTimer += Time.deltaTime;
        string audioName = string.Empty;
        float maxTime = 0f;

        switch (CurrentState)
        {
            case States.walking:
                {
                    audioName = "WalkingFootStep";
                    maxTime = m_TimeBetweenWalkingSteps;
                    break;
                }
            case States.running:
                {
                    audioName = "RunningFootStep";
                    maxTime = m_TimeBetweenRunningSteps;
                    break;
                }
            case States.crouching:
                {
                    audioName = "CrouchingFootStep";
                    maxTime = m_TimeBetweenCrouchingSteps;
                    break;
                }
        }

        if (m_StepsTimer > maxTime)
        {
            AudioManager.Instance.PlayAudio(audioName, null, true, (CurrentState == States.crouching ? 0.2f : 1f));
            m_StepsTimer = 0f;
        }
    }
    #endregion

    #region Extras
    public void AllowMovement(bool _allow)
    {
        // in case the game was paused while an object was held by the observation class
        if (PlayerObjectsInteraction.Instance.Holding)
        {
            return;
        }

        AllowedToMove = _allow;
        // zeroing all values to prevent moving after pausing
        // resetting movement speed
        m_Velocity = Vector3.zero;
        m_CameraRotation = 0f;
        m_BodyRotation = 0f;
    }

    // a function that keeps the camera rotation after a camera event ends. Only called by the PlayerCameraEvent class.
    public void RestoreRotations()
    {
        float _rotation = CameraTransform.localEulerAngles.x;

        // if the player is looking up then the value of the x rotation will be between 360 and 280 which doesn't work when applied to the euler angles because it's clamped between -80 and 80 so we substract it from 360 to get a value between 0 and -80.
        if (_rotation > 280)
        {
            _rotation = (CameraTransform.localEulerAngles.x - 360);
        }

        m_CurrentCameraRotation = _rotation;
    }
    #endregion
}