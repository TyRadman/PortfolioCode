using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;

public class StatesEntity : MonoBehaviour, IController
{
    [HideInInspector] public NavMeshAgent Agent;
    [HideInInspector] public EntityAnimation Animation;
    [HideInInspector] public Transform PlayerTransform;
    [HideInInspector] public Transform EntityTransform;
    [HideInInspector] public List<Vector3> WayPoints = new List<Vector3>();
    [HideInInspector] public Vector3 Target;
    [HideInInspector] public bool IsActive = true;
    [HideInInspector] public bool StateRunning = false;
    [HideInInspector] public EyeIndicator Indicator;
    [SerializeField] private float m_RefreshRate = 20f;
    [SerializeField] private List<MachineState> m_States = new List<MachineState>();
    private WaitForSeconds m_LoopWaitTime;
    [HideInInspector] public MachineState CurrentMachineState;

    private MachineState m_NextMachineState;
    [SerializeField] private Text m_TestingText;
    private EntityComponents m_Components;

    public void SetUp(IComponent component)
    {
        m_Components = (EntityComponents)component;
        CacheValues();
        SetUpStates();
    }

    private void CacheValues()
    {
        Agent = GetComponent<NavMeshAgent>();
        Animation = GetComponent<EntityAnimation>();
        Indicator = FindObjectOfType<EyeIndicator>();
        PlayerTransform = PlayerStats.Instance.transform;
        EntityTransform = transform;
        System.Array.ForEach(GameObject.FindGameObjectsWithTag("WayPoint"), w => WayPoints.Add(w.transform.position));
        m_RefreshRate = 1 / m_RefreshRate;
        m_LoopWaitTime = new WaitForSeconds(m_RefreshRate);
    }

    private void SetUpStates()
    {
        m_States.ForEach(s => s.SetUp(m_Components));
        CurrentMachineState = m_States[0];
        m_NextMachineState = CurrentMachineState;
    }

    public void Activate()
    {
    }

    public void StopState()
    {
    }

    public void SetNextState(MachineState.StateName stateName)
    {
        m_NextMachineState = m_States.Find(s => s.StateTag == stateName);
    }

    public void PerfromNextState()
    {
    }

    #region Agent Methods
    /// <summary>
    /// Sets a target position for the entity and moves it towards that target
    /// </summary>
    /// <param name="_target"></param>
    public void SetAgentTarget(Vector3 _target)
    {
        Target = _target;
        Agent.SetDestination(Target);
    }

    public void SetAgentSpeed(float _speed)
    {
        //Debug.Log($"Speed {_speed}");
        Agent.speed = _speed;
    }

    public void RotateToTarget(Vector3 _target, float _rotationAmount = 0.3f)
    {
        // gets the direction vector that points towards the player
        Vector3 dir = (_target - transform.position).normalized;

        // if the entity is facing the object
        if (dir.x == 0f && dir.z == 0f)
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
    public float GetDistanceToTarget(Vector3 _target)
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
    #endregion

    #region Helpers
    public void Dispose()
    {
    }
    #endregion
}
