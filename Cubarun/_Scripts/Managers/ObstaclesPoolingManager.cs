using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesPoolingManager : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private int initialNumberOfObstacles = 20;
    private Transform obstaclesParent;
    private List<Obstacle> stableObstacles = new List<Obstacle>();
    private List<Obstacle> movingSideWaysObstacles = new List<Obstacle>();
    private List<Obstacle> movingUpAndDownObstacles = new List<Obstacle>();

    private void Awake()
    {
        obstaclesParent = new GameObject("Obstacles").transform;
    }

    private void createObstacles()
    {
        for (int i = 0; i < initialNumberOfObstacles; i++)
        {
            foreach (var item in obstaclePrefabs)
            {
                var obstacle = Instantiate(item, obstaclesParent).GetComponent<Obstacle>();
                obstacle.gameObject.SetActive(false);

                switch (obstacle.ObstacleType)
                {
                    case Obstacle.ObstacleTypeEnum.Stable:
                        stableObstacles.Add(obstacle);
                        break;
                    case Obstacle.ObstacleTypeEnum.MovingSideWays:
                        movingSideWaysObstacles.Add(obstacle);
                        break;
                    case Obstacle.ObstacleTypeEnum.MovingUpAndDown:
                        movingUpAndDownObstacles.Add(obstacle);
                        break;
                }
            }
        }
    }
}