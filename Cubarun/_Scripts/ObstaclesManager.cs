using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesManager : MonoBehaviour
{
    public enum lightType
    {
        MovingObstacleLight,
    }

    public static ObstaclesManager Instance;

    [System.Serializable]
    public struct ObstacleMaterial
    {
        public lightType Type;
        public Color Color;
        public float MinimumEmission;
        public float MaximumEmission;

        public float GetEmission(float t)
        {
            return Mathf.Lerp(MinimumEmission, MaximumEmission, t);
        }
    }

    [SerializeField] private List<ObstacleMaterial> materialsVariations;

    private void Awake()
    {
        Instance = this;
    }

    public ObstacleMaterial GetColor(lightType type)
    {
        return materialsVariations.Find(m => m.Type == type);
    }
}
