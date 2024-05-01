using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

[RequireComponent(typeof(EntitySensing))]
public class StatesEntity : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent Agent;
    [HideInInspector] public EntityAnimation Animation;
    [HideInInspector] public Transform PlayerTransform;
    [HideInInspector] public Transform EntityTransform;
    [HideInInspector] public List<Vector3> WayPoints = new List<Vector3>();
    [HideInInspector] public Vector3 Target;
    [HideInInspector] public MachineState m_HearingEvent;
    [HideInInspector] public MachineState m_SightEvent;
    [HideInInspector] public bool IsActive = true;
    [HideInInspector] public bool StateRunning = false;
    [HideInInspector] public bool PlayerIsSeen = false;
    [HideInInspector] public float PlayerLosingTime;
    [HideInInspector] public EntitySensing Senses;
    [HideInInspector] public EyeIndicator Indicator;
    [SerializeField] private float m_RefreshRate = 20f;
    private List<MachineState> m_States = new List<MachineState>();
    private WaitForSeconds m_LoopWaitTime;
    [HideInInspector] public MachineState CurrentMachineState;
    [HideInInspector] public MachineState NextMachineState;
    public bool AffectsIndicator = false;
    [Header("References")]
    [SerializeField] private GameObject m_StatesParent;
    [Header("Difficulty Values")]
    public MovementSpeeds Speeds;
    [SerializeField] private Text m_TestingText;
    // difficulty values

    private void Awake()
    {
        setUpAwakeReferences();
        extractStates();
    }

    private void Start()
    {
        getDifficultyValues();
        setUpStartReferences();
        setExternalTriggers();
        performState();
        startEyeIndicator();
    }

    private void performState()
    {
        m_TestingText.text = $"State: {CurrentMachineState.StateTag}";
        StartCoroutine(entityLoop());
    }

    private IEnumerator entityLoop()
    {
        // to inform the states that the entity is performing a state
        StateRunning = true;
        // invoke the start function of the state
        CurrentMachineState.StartActivity();

        // keep invoking the update functinon of the state as long as the bool is true
        while (StateRunning)
        {
            CurrentMachineState.UpdateActivity();
            yield return m_LoopWaitTime;
        }

        // after we stop the update function we invoke the end 
        CurrentMachineState.EndActivity();

        // the state is switched to the next one
        CurrentMachineState = NextMachineState; 

        // the next state is performed
        performState();
    }

    public void PerfromNextState(MachineState.StateName _name)
    {
        StateRunning = false;
        // fetch a state with a matching tag
        NextMachineState = m_States.Find(s => s.StateTag == _name);
    }

    #region Agent Methods
    public void SetAgentTarget(Vector3 _target)
    {
        Target = _target;
        Agent.SetDestination(Target);
    }

    public void SetAgentSpeed(float _speed)
    {
        Agent.speed = _speed;
    }

    public void RotateToTarget(Vector3 _target, float _rotationAmount = 0.3f)
    {
        // gets the direction vector that points towards the player
        Vector3 dir = (_target - transform.position).normalized;

        // if the entity is facing the object
        if(dir.x == 0f && dir.z == 0f)
        {
            return;
        }

        // turns it into a quaternion
        Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        // applies the quaternion to the rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, _rotationAmount);
    }

    // ensures the selected destination of the navmesh is reachable
    public bool DestinationIsReachable(Vector3 _destination)
    {
        NavMeshPath path = new NavMeshPath();
        Agent.CalculatePath(_destination, path);
        return path.status != NavMeshPathStatus.PathPartial;
    }
    #endregion

    #region Senses Related Methods
    private void setExternalTriggers()
    {
        // checks the states and assigns specific methods as listeners for the sight and hearing events
        m_States.ForEach(s => s.AssignListeners());
    }

    public void TriggerSight()
    {
        // if the same state is not active and the condition of this state for not playing is not fulfilled then play it
        if(m_SightEvent.SensesDealBreaker())
        {
            return;
        }

        Senses.CanHear = false;

        if (AffectsIndicator)
        {
            Indicator.UpdateIdicator(EyeState.Noticing);
        }

        PerfromNextState(m_SightEvent.StateTag);
    }

    public void TriggerHearing()
    {
        if (m_HearingEvent.SensesDealBreaker())
        {
            return;
        }

        PerfromNextState(m_HearingEvent.StateTag);
    }

    public float DistanceToTarget(Vector3 _target)
    {
        // creates a new path
        NavMeshPath path = new NavMeshPath();
        // fills this path with data from the enemy to the play
        Agent.CalculatePath(PlayerTransform.position, path);
        // creates an array of V3s and sets it's size to the number of path corners plus two which is the number of points (corners + the start + the end point)
        var pathPoints = new Vector3[path.corners.Length + 2];
        // sets the first point as the enemy's position
        pathPoints[0] = transform.position;
        // sets the last point as the destination which is where the noise was made
        pathPoints[pathPoints.Length - 1] = PlayerTransform.position;

        for (int i = 1; i < pathPoints.Length - 1; i++)
        {
            // sets the other points using the path corners
            pathPoints[i] = path.corners[i - 1];
        }

        float distanceFromPlayer = 0f;

        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            // calculates the path's length by summing up the distances between points
            distanceFromPlayer += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
        }

        return distanceFromPlayer;
    }

    public void PlayerSeen(bool _seen)
    {
        PlayerIsSeen = _seen;
        PlayerStats.Instance.IsSeen = _seen;

        if (AffectsIndicator)
        {
            Indicator.UpdateIdicator(_seen ? EyeState.SeeingAndChasing : EyeState.LookingAndChasing);
        }
    }
    #endregion

    #region Initializers
    private void extractStates()
    {
        int statesNum = m_StatesParent.GetComponents<MachineState>().Length;

        for (int i = 0; i < statesNum; i++)
        {
            // add the state to the list
            m_States.Add(m_StatesParent.GetComponents<MachineState>()[i]);
            // set the mutual references to it
            m_States[i].SetMutualReferences(this);
        }

        // set the first state as the current state 
        CurrentMachineState = m_States[0];
        NextMachineState = CurrentMachineState;
    }

    private void setUpStartReferences()
    {
        PlayerTransform = PlayerStats.Instance.transform;
        EntityTransform = transform;
        System.Array.ForEach(GameObject.FindGameObjectsWithTag("WayPoint"), w => WayPoints.Add(w.transform.position));
        m_RefreshRate = 1 / m_RefreshRate;
        m_LoopWaitTime = new WaitForSeconds(m_RefreshRate);
    }

    private void setUpAwakeReferences()
    {
        Agent = GetComponent<NavMeshAgent>();
        Animation = GetComponent<EntityAnimation>();
        Indicator = FindObjectOfType<EyeIndicator>();
        Senses = GetComponent<EntitySensing>();
        Senses.enabled = true;
    }

    private void getDifficultyValues()
    {
        // fetch a difficulty with the same tag as the entity's tag
        DifficultyModifier dif = GameManager.Instance.CurrentDifficulty;
        // set up values
        PlayerLosingTime = dif.LosingPlayerTime;
    }

    private void startEyeIndicator()
    {
        if(Indicator == null)
        {
            print("Player is the problem");
        }

        Indicator.UpdateIdicator(EyeState.StartLooking);
    }
    #endregion

    #region Helpers
    public bool CheckCurrentState(MachineState.StateName _tag)
    {
        return CurrentMachineState.StateTag == _tag;
    }
    #endregion
}
