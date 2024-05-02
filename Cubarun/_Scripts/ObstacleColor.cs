using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleColor : MonoBehaviour
{
    void Start()
    {
        GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
    }

    public void SetUp(ObstaclesManager.lightType type, float tSpeed)
    {
        Color color;
        float emission;

        if(ObstaclesManager.Instance == null)
        {
            return;
        }

        var material = ObstaclesManager.Instance.GetColor(type);
        color = material.Color;
        emission = material.GetEmission(tSpeed);

        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color * emission);
    }
}
