using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class Timer
{
    private static Stopwatch stopwatch = new Stopwatch();

    public static void StartTimer()
    {
        stopwatch.Start();
    }

    public static void StopTimer(string title)
    {
        stopwatch.Stop();
        stopwatch.Reset();
    }

    public static void StartTimerUnits()
    {
        stopwatch.Start();
    }
}
