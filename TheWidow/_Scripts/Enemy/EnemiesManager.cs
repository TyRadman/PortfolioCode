using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// this class manages everything related to the enemy. Whether it's the AI or the jumpscare functionality
public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager Instance;
    private List<Vector3> m_EnemySpawnPoints = new List<Vector3>();
    [Tooltip("Number of medicine bottles required to spawn the Widow")]
    [InspectorName("Bottles Number")][SerializeField] private int m_NumberOfBottlesToSpawn;
    [Header("Jumpscares variables")]
    [SerializeField] private int m_MinJumpScaresNum = 1;
    [SerializeField] private int m_MaxJumpScaresNum = 1;
    private bool m_EnemiesSpawned = false;
    [Header("Enemies")]
    [SerializeField] private List<StatesEntity> m_Enemies;

    private void Awake()
    {
        Instance = this;
        // disable all enemies
        m_Enemies.ForEach(e => e.gameObject.SetActive(false));
        // caching spawn points
        System.Array.ForEach(GameObject.FindGameObjectsWithTag("EnemySpawnPoints"), p => m_EnemySpawnPoints.Add(p.transform.position));
    }

    private void Start()
    {
        // to spawn the enemy after a number of medicine bottle has been picked up
        PlayerStats.Instance.AddListenerToMedicineCollection(NumberOfBottlesUpdate);
        // TimeClass.PerformTask(ActivateSmartWidow, TimeClass.ConvertTimeToSeconds(m_WidowActivationTime));
    }

    private void NumberOfBottlesUpdate()
    {
        // if the player collected enough bottles then the enemy is spawned and this class no longer listen to the medicine collection event
        if (PlayerStats.Instance.MedicinesCollected == m_NumberOfBottlesToSpawn)
        {
            ActivateEnemy(DifficultyModifier.EnemyTag.TheWidow, getFurthestDistanceFromPosition(PlayerStats.Instance.transform.position));
            PlayerStats.Instance.RemoveListenerFromMedicineCollection(NumberOfBottlesUpdate);
        }
    }

    public void ActivateEnemy(DifficultyModifier.EnemyTag _tag, Vector3 _spawnPos)
    {
        // cache the enemy to activate
        StatesEntity enemyToActivate = m_Enemies.Find(e => e.Senses.m_EnemyTag == _tag);
        // position the enemy
        enemyToActivate.gameObject.transform.position = _spawnPos;
        // enable the enemy
        enemyToActivate.gameObject.SetActive(true);
        m_EnemiesSpawned = true;
    }

    #region Helper functions
    public bool EnemiesExistInLevel()
    {
        return m_EnemiesSpawned;
    }

    public void TriggerHearing(Vector3 _target)
    {
        // triggers the hearing of every active enemies with their own different hearing distances
        m_Enemies.ForEach(e => e.Senses.HearingProcess(e.Senses.HearingDistances.DoorOpeningDistance, _target));
    }

    private Vector3 getFurthestDistanceFromPosition(Vector3 _position)
    {
        return m_EnemySpawnPoints.OrderBy(p => Vector3.Distance(p, _position)).LastOrDefault();
    }
    #endregion
}
