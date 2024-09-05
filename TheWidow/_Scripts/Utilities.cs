using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;
using BT.Nodes;

public static class Utilities
{
    public static StatesEntity GetEntity<T>(this T node) where T : ActionNode
    {
        return node.Agent.GetComponent<EntityComponents>().Entity;
    }

    public static TheWidowComponents GetEnemyComponents<T>(this T node) where T : ActionNode
    {
        return node.Agent.GetComponent<TheWidowComponents>();
    }
}
