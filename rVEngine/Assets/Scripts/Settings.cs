using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private int _fps = 60;
    private void Start()
    {
        Application.targetFrameRate = _fps;
    }
}
