using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance;

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            print($"Destroyed {gameObject.name} because the script already exists at {Instance.gameObject.name}");
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
    }
}
