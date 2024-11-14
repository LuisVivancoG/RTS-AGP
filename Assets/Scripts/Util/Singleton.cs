using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<TSingletonClass> : MonoBehaviour where TSingletonClass : MonoBehaviour
{
    public static TSingletonClass Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
            Instance = this as TSingletonClass;
    }

    private void OnApplicationQuit()
    {
        Destroy(gameObject);
        Instance = null;
    }
}
