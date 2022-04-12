using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coroutine : MonoBehaviour
{
    public void Begin(IEnumerator routine)
    {
        StartCoroutine(routine);
    }
}
