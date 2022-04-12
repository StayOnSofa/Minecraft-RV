using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance()
    {
        T singleton = FindObjectOfType<T>();
        if (singleton == null)
        {
            GameObject gameObject = new GameObject($"[ST: {typeof(T).Name}]");
            singleton = gameObject.AddComponent<T>();
        }

        return singleton;
    }
}
