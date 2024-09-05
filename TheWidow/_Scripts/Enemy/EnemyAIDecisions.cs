using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIDecisions : MonoBehaviour
{
    #region Events
    public delegate void OnStateChange(EnemyActions _action);
    public event OnStateChange StateChanged = delegate { };
    #endregion

    #region Classes instances
    private TheWidowAnimation m_Animations;
    private EyeIndicator m_Eye;
    #endregion

    #region Components variables
    [HideInInspector] public NavMeshAgent Agent;
    private Transform m_PlayerTransform;
    [SerializeField] private Transform m_Caster;
    private Vector3 m_Target;
    private readonly List<Vector3> m_WayPoints = new List<Vector3>();
    private Coroutine m_TimerToLosePlayer;
    private Coroutine m_StandingCoroutine;
    private Coroutine m_ScreamingCoroutine;
    private Coroutine m_EnemyDecision;
    [SerializeField] private List<BoxCollider> m_SightColliders;
    #endregion

    #region Enums
    public enum EnemyActions
    {
        stand, patrol, scream, chase, kill
    }
    private EnemyActions m_Action;
    public EnemyActions Action
    {
        get
        {
            return m_Action;
        }
        set
        {
            m_Action = value;
            StateChanged(value);            // sends an event with the new state
            //m_Animations.Animate(value);    // animates the widow accordingly
        }
    }
    #endregion

    #region Enemy related variables
    [Header("Enemy Properties:")]
    public float ChaseSpeed = 4f;
    public float PatrolSpeed = 1f;
    [HideInInspector] public float LosingPlayerTime = 4f;
    [SerializeField] private float NormalSightDistance = 7f;
    [SerializeField] private float m_ScreamTime = 2f;
    [SerializeField] private float m_MinWaitingTime;
    [SerializeField] private float m_MaxWaitingTime;
    private readonly float m_FlashLightExtraSight = 30f;
    [SerializeField] private float m_CatchingDistance = 1.5f;
    [SerializeField] private float m_KillingDistance = 1f;
    [Header("hearing distances:")]
    [SerializeField] private float m_NormalHearingDistance = 0f;
    [SerializeField] private float m_Walking_hearingDistance = 10f;
    [SerializeField] private float m_Running_hearingDistance = 15f;
    [SerializeField] private float m_Crouching_hearingDistance = 5f;
    [Header("Other:")]
    public bool EnemyCanSee = true; // testing variable. To avoid getting seen when testing
    public static float DistanceFromPlayer;
    public float DoorOpeningHearingDistance;
    public float InteractionsHearingDistance;
    public bool PlayerIsSeen = false;
    private bool m_PlayerWasHeard = false;
    private float m_HearingDistance = 0f;
    private const string HARD = "hard";
    // tips related variables
    private bool m_EverSeen = false;
    private bool m_EverHeard = false;
    #endregion

    void OnEnable()
    {
        // to avoid getting those classes a start method which gives us less control over their timing      
        initializeComponents();
        // gets the nav mesh agent component
        Agent = GetComponent<NavMeshAgent>();
        // gets the difficulty from the game manager and all values that the difficulty has
        SetDetectingDistances(GameManager.Instance.Settings.CurrentDifficulty);

        // if the player is hidden when the enemy spawns
        if (PlayerStats.Instance.HidingSpot != null)    
        {
            // assigns a reference to the enemy within the playerStats class which assigns the enemy only if the player is not hidden
            PlayerStats.Instance.HidingSpot.AssignEnemy();             
        }

        GameManager.Instance.EnemySpawned = true;
    }

    #region Initiating values
    void Start()
    {
        m_Eye = GetComponent<EyeIndicator>();
        // assigning classes to variables
        m_Animations = GetComponent<TheWidowAnimation>();  

        #region Subscriptions
        // sets the setHearingDistanceValue function as a listener the change of the player state event, so that the hearing values change according to the state of the player
        PlayerMovement.Instance.E_PlayerStateOnChange += setHearingDistanceValue;
        // whenever the flashlight state is changed, the mobility of the enemy is changed as well
        FindObjectOfType<PlayerActions>().FlashLightStateChanged += increaseMobility;
        #endregion

        m_PlayerTransform = PlayerStats.Instance.transform;
        // sets the widow as a move-to target before assigning a destination
        setTarget(transform.position);                          
        Agent.stoppingDistance = m_KillingDistance;
        // the main enemy loop
        m_EnemyDecision = StartCoroutine(enemyDecisions());     
        Agent.speed = PatrolSpeed;
        // finds all waypoints in the level
        getWayPoints();                                         
    }
    #endregion

    #region Sight
    void performSight()
    {
        // the sight doesn't work if the player is dead, or if the enemy can't see, or if the enemy is killing the player
        if (PlayerStats.Instance.IsDead || !EnemyCanSee || Action == EnemyActions.kill)
        {
            return;
        }

        // debug related code
        Debug.DrawRay(m_Caster.position, m_Caster.TransformDirection(Vector3.forward) * NormalSightDistance, Color.red);
        // points the "caster" to look at the player the whole time
        m_Caster.transform.LookAt(m_PlayerTransform);   

        // shoots a ray from the caster to the player
        if (Physics.Raycast(m_Caster.position, m_Caster.TransformDirection(Vector3.forward), out RaycastHit hit, NormalSightDistance))
        {
            // if the player is hit with the ray
            if (hit.collider.CompareTag("Player"))  
            {
                PlayerIsSeen = true;

                // this is only possible once. Just to show the tip on the side of the screen when the player is seen for the first time ever
                if (!m_EverSeen) 
                {
                    m_EverSeen = false;
                    // displays the tip
                    TipsManager.Instance.DisplayTip(Tips.TipsTag.Escape);   
                }

                // when the player is detected
                if (Action != EnemyActions.chase && Action != EnemyActions.scream)  
                {
                    // if the player was holding an object, the onject is released
                    PlayerObjectsInteraction.Instance.ReleaseObject();
                    // switch the state of the enemy
                    Action = EnemyActions.scream;                       
                }

                return;
            }

            PlayerIsSeen = false;
        }
    }
    #endregion

    #region Enemy's decisions generator
    IEnumerator enemyDecisions()
    {
        // we cache the waitForSeconds to avoid creating a new instance of it everytime we use it
        var wait = new WaitForSeconds(0.1f);   

        while (true)
        {
            switch (Action)
            {
                case EnemyActions.stand:
                    stand();
                    break;
                case EnemyActions.patrol:
                    patrol(m_Target);
                    break;
                case EnemyActions.chase:
                    chase();
                    break;
                case EnemyActions.scream:
                    scream();
                    break;
                case EnemyActions.kill:
                    kill();
                    break;
            }

            performHearing();
            performSight();
            yield return wait;
        }
    }
    #endregion

    #region Enemy action functions
    // called when the player is captured by the enemy
    void kill()
    {
        // rotate the enemy towards the player so that the animation plays well
        rotateToTarget(m_PlayerTransform.position, 1f);
        // stop the enemy states switching coroutine
        StopCoroutine(m_EnemyDecision);
        // create a fading screen
        FadingScreen.Instance.Fade(0.3f, 0.02f, 0.1f, Color.black);
        // call the game over function
        GameManager.Instance.GameOver();
        // the animator in Unity culls objects that are not seen. When the player is dead, we disable culling because we use a different camera that doesn't have a culling feature
        m_Animations.DisableCulling();                                  
        PlayerMovement.Instance.AllowMovement(false);
        PlayerStats.Instance.IsDead = true;
        // stopping the enemy
        Agent.enabled = false; 
    }

    // the phase that takes place right before chasing
    void scream()
    {
        if (m_ScreamingCoroutine == null)
        {
            m_ScreamingCoroutine = StartCoroutine(scream_Coroutine(m_ScreamTime));
        }

        // makes the enemy rotate towards the player
        rotateToTarget(m_PlayerTransform.position); 
    }

    void chase()
    {
        #region Chasing process
        // if the player didn't hide then he's the target
        // if he's hidden then the enemy goes to the place the player was seen
        Agent.speed = ChaseSpeed;

        if (!PlayerStats.Instance.IsHidden)
        {
            if (Vector3.Distance(transform.position, m_Target) > m_CatchingDistance)
            {
                setTarget(m_PlayerTransform.position);
            }
            else
            {
                Action = EnemyActions.kill;
                return;
            }
        }
        // if the player is hidden then the enemy moves to the hiding spot
        else if (PlayerStats.Instance.IsHidden) 
        {
            if (Vector3.Distance(transform.position, m_Target) > Agent.stoppingDistance)
            {
                setTarget(PlayerStats.Instance.HidingSpot.EnemyStandingPlace);
            }
            else
            {
                // once it reaches the hiding spot it increaases the number of time the player escaped into that spot
                // increases the number of times and once the limit is exceeded then the player is no longer hidden
                PlayerStats.Instance.HidingSpot.IncreaseSpottingTimes();

                // the enemy will stand only if the player is hidden
                if (PlayerStats.Instance.IsHidden)                
                {
                    Action = EnemyActions.stand;
                }
            }
        }
        #endregion

        #region Losing the player while chasing
        // if the pllayer vanished from the enemy's sight
        if (!PlayerIsSeen)
        {
            // makes sure this is the first time the player is seen since last time he wasn't seen
            if (m_TimerToLosePlayer == null)        
            {
                // starts a coroutine that starts a count down when the player is out of sight after which the enemy goes back to patroling
                m_TimerToLosePlayer = StartCoroutine(losingPlayer()); 
            }
        }
        // if he was spotted again while being chased
        else
        {
            if (m_TimerToLosePlayer != null)    
            {
                StopCoroutine(m_TimerToLosePlayer);
                m_TimerToLosePlayer = null;
            }
        }
        #endregion
    }

    // the patrol function moves the enemy to a selected waypoint
    void patrol(Vector3 _target)
    {   
        // comparing the square magnitude of a vector to the square of a number calculates the distance between them but not precisely
        if ((transform.position - _target).sqrMagnitude > 3 && Action != EnemyActions.scream) // if the enemy is not close enough to the way point
        {
            Agent.SetDestination(_target);
        }
        // once the waypoint is reached, the enemy switches state to standing
        else
        {
            Action = EnemyActions.stand;
        }
    }

    // the state where the enemy just stands still before partoling to the next way point
    void stand()
    {
        // set the target as the previous target which is the waypoint that the enemy was moving to
        setTarget(m_Target);    
        Agent.speed = 0f;       
     
        if (m_StandingCoroutine == null)
        {
            // wait in place for a random time before patroling again
            m_StandingCoroutine = StartCoroutine(standForSeconds(Random.Range(m_MinWaitingTime, m_MaxWaitingTime)));
        }
    }
    #endregion

    #region Enemy action coroutines (helpers of the functions)
    IEnumerator scream_Coroutine(float time)
    {
        m_Target = transform.position;

        // emptying the standing coroutine if the previous action was standing
        if (m_StandingCoroutine != null)
        {
            StopCoroutine(m_StandingCoroutine);
            m_StandingCoroutine = null;
        }

        setTarget(m_Target);    
        Agent.speed = 0f;
        yield return new WaitForSeconds(time);
        Action = EnemyActions.chase;
        Agent.speed = ChaseSpeed;
        setTarget(m_PlayerTransform.position);
        m_ScreamingCoroutine = null;
    }

    // the standing phase code 
    IEnumerator standForSeconds(float time)
    {
        // makes the enemy face the destination it was heading to
        rotateToTarget(m_PlayerTransform.position); 
        yield return new WaitForSeconds(time);
        // the part where the enemy loses the player
        if (Action != EnemyActions.chase)
        {
            // if the game difficulty is hard then choose the closest waypoint to the player
            if (GameManager.Instance.Settings.CurrentDifficulty.name == HARD)    
            {
                chooseCloseWayPoint();
            }
            else
            {
                // choose a random different waypoint
                chooseDifferentWayPoint();          
            }

            Agent.speed = PatrolSpeed;
            Action = EnemyActions.patrol;
        }

        m_StandingCoroutine = null;
    }

    IEnumerator losingPlayer()
    {
        yield return new WaitForSeconds(LosingPlayerTime);
        // changes the eye indicator st                     
        Action = EnemyActions.stand;
    }
    #endregion

    #region Choosing a new waypoint
    // chooses a way point different from the one chosen previously and makes sure its reachable by the enemy
    void chooseDifferentWayPoint()
    {
        // caches all waypoints except for the previously chosen point and the unreachable points
        List<Vector3> availableWayPoints = m_WayPoints.FindAll(wp => wp != m_Target && destinationIsReachable(wp));
        setTarget(availableWayPoints[Random.Range(0, availableWayPoints.Count)]);
    }

    // chooses a waypoint close to the player
    void chooseCloseWayPoint()
    {
        // caches all waypoints except for the previously chosen point and the unreachable points
        List<Vector3> newWayPoints = m_WayPoints.FindAll(wp => wp != m_Target && destinationIsReachable(wp));
        // selects the closest waypoint by ordering the waypoints according to the distance between them and the player and choosing the first one
        Vector3 closestPoint = newWayPoints.OrderBy(w => Vector3.Distance(w, m_PlayerTransform.position)).First();
        setTarget(closestPoint);
    }
    #endregion

    #region Collider triggers
    void performHearing()
    {
        // if the enemy is not chasing, killing and screaming and the player is not hiding then we perform hearing
        if (Action != EnemyActions.chase && Action != EnemyActions.kill && Action != EnemyActions.scream && 
            !PlayerStats.Instance.IsHidden)
        {
            // if the player was heard
            if (m_PlayerWasHeard)
            {
                // this condition is there only to display the tip of here when the player is heard for the first time in a run
                if(!m_EverHeard)
                {
                    m_EverHeard = false;
                    TipsManager.Instance.DisplayTip(Tips.TipsTag.EnemyHearing);
                }

                // moving the enemy to the player, not in a chasing state, but as a patrol to check the place
                moveToClonePoint(m_PlayerTransform.position); 
            }
            // the hearing process determines whether the  player was heard or not 
            else
            {
                m_PlayerWasHeard = Heard(m_HearingDistance, m_PlayerTransform.position);
            }
        }
    }
    #endregion

    #region Hearing function
    // tells whether the player was heard by the enemy or not
    public bool Heard(float _distance, Vector3 _destination)
    {
        // checks if the destination can be reached or not. Ideally, this should never be true in this function, but it has to be logged
        if (!destinationIsReachable(_destination))      
        {
            Debug.LogWarning("Why is the player in a place the enemy can't reach!?");
            return false;
        }

        // creates a new path
        NavMeshPath path = new NavMeshPath();
        // fills this path with data from the enemy to the play
        Agent.CalculatePath(m_PlayerTransform.position, path);
        // creates an array of V3s and sets it's size to the number of path corners plus two which is the number of points (corners + the start + the end point)
        var pathPoints = new Vector3[path.corners.Length + 2];
        // sets the first point as the enemy's position
        pathPoints[0] = transform.position;
        // sets the last point as the destination which is where the noise was made
        pathPoints[pathPoints.Length - 1] = _destination;           

        for (int i = 1; i < pathPoints.Length - 1; i++)
        {
            // sets the other points using the path corners
            pathPoints[i] = path.corners[i - 1];                  
        }

        DistanceFromPlayer = 0f;

        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            // calculates the path's length by summing up the distances between points
            DistanceFromPlayer += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);   
        }

        // caches whether the distance calculated is greater than the distance we set as minimum
        var heard = DistanceFromPlayer <= _distance;
        // feeds this bool to the hearing icon so that it triggers it if the value is true
        //m_Eye.DisplayHearingIcon(heard);
        // finally returns the value
        return heard;                                           
    }
    #endregion

    #region Setting the hearing distances
    // whenever the player's state is changed this function is called, to determine the hearing distance
    void setHearingDistanceValue()
    {
        switch (PlayerMovement.Instance.CurrentState)
        {
            case PlayerMovement.States.hiding:
                m_HearingDistance = 0f;
                break;
            case PlayerMovement.States.crouching:
                // crouching generally is noisier than crouching while hidden
                m_HearingDistance = m_Crouching_hearingDistance + m_NormalHearingDistance;
                break;
            case PlayerMovement.States.running:
                m_HearingDistance = m_Running_hearingDistance + m_NormalHearingDistance;
                break;
            case PlayerMovement.States.standing:
                m_HearingDistance = 0f;
                break;
            case PlayerMovement.States.walking:
                m_HearingDistance = m_Walking_hearingDistance + m_NormalHearingDistance;
                break;
        }
    }
    #endregion

    #region Enemy movement to heard points
    // function that moves the enemy to the player location where he was last heard
    public void moveToClonePoint(Vector3 _target)
    {
        Agent.speed = (PatrolSpeed + ChaseSpeed) / 2f;

        // necessary so it doesn't decide to patrol later
        if (m_StandingCoroutine != null)                        
        {
            StopCoroutine(m_StandingCoroutine);
            m_StandingCoroutine = null;
        }

        // if the enemy's state was standing
        if (Action == EnemyActions.stand)                       
        {
            // we stop the enemy main function
            StopCoroutine(m_EnemyDecision);
            // we set a target
            setTarget(_target);
            // we start a patrol function
            Action = EnemyActions.patrol;
            // we return the main function
            m_EnemyDecision = StartCoroutine(enemyDecisions()); 
        }
        else
        {
            // we set that point as a target
            setTarget(_target);                                 
        }

        // then we stop updating the hearing unless a player invokes it
        m_PlayerWasHeard = false;                               
    }
    #endregion

    #region Mini functions
    void rotateToTarget(Vector3 _target, float _rotationAmount = 0.3f)
    {
        // gets the direction vector that points towards the player
        Vector3 dir = (_target - transform.position).normalized;
        // turns it into a quaternion
        Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        // applies the quaternion to the rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, _rotationAmount);
    }

    void getWayPoints()
    {
        System.Array.ForEach(GameObject.FindGameObjectsWithTag("WayPoint"), w => m_WayPoints.Add(w.transform.position));
    }

    // the function that gets called when a flash light event occurs
    void increaseMobility(bool _flashLightOn) 
    {
        // increases the sight distance by a set amount multiplied by a sign that determines whether the value is increased or decreased
        NormalSightDistance += _flashLightOn ? m_FlashLightExtraSight : -m_FlashLightExtraSight;
        // disabling all sight colliders if the flashlight is turned on. These colliders make sure the enemy can't see what's behind it unless the flashlight is on
        m_SightColliders.ForEach(c => c.enabled = !_flashLightOn);  
    }
    void setTarget(Vector3 _target)
    {
        m_Target = _target;
        Agent.SetDestination(m_Target);
    }

    // ensures the selected destination of the navmesh is reachable
    bool destinationIsReachable(Vector3 _destination)
    {
        NavMeshPath path = new NavMeshPath();
        Agent.CalculatePath(_destination, path);
        return path.status != NavMeshPathStatus.PathPartial;
    }

    // sets values based on selected difficulty
    public void SetDetectingDistances(DifficultyModifier _values)
    {
        //m_Crouching_hearingDistance = _values.CrouchingDistance;
        //m_Walking_hearingDistance = _values.WalkingDistance;
        //m_Running_hearingDistance = _values.RunningDistance;
        //NormalSightDistance = _values.SightDistance;
        //LosingPlayerTime = _values.LosingPlayerTime;
        //PatrolSpeed = _values.WalkingSpeed;
        //ChaseSpeed = _values.ChasingSpeed;
        //DoorOpeningHearingDistance = _values.DoorOpeningDistance;

        // gives the enemy extra sight if the flash light was on
        if (FindObjectOfType<PlayerActions>().FlashLight.gameObject.activeSelf)         
        {
            NormalSightDistance += m_FlashLightExtraSight;
        }

        // sight colliders modifications. The harder the difficulty is, the more the enemy can see
        for (int i = 0; i < m_SightColliders.Count - 1; i++)
        {
            switch (_values.SightRadius)
            {
                case 0:
                    m_SightColliders[i].size = new Vector3(m_SightColliders[i].size.x, m_SightColliders[i].size.y, m_SightColliders[i].size.z);
                    break;
                case 1:
                    m_SightColliders[i].size = new Vector3(m_SightColliders[i].size.x / 2, m_SightColliders[i].size.y, m_SightColliders[i].size.z);
                    break;
                case 2:
                    m_SightColliders[i].enabled = false;
                    break;
            }
        }
    }
    #endregion

    // initializes other class that the enemy depends on
    public void initializeComponents()
    {
        //GetComponent<TheWidowAnimation>().Initialize();
        //GetComponent<EnemyAudio>().Initialize();
    }
}