using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour {
    public long TimePerTurn = 30000;

    System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

    public void Start()
    {
        StartTimer();
    }
    public long GetTicks()
    {
        return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }
    public void StartTimer()
    {
        timer.Restart();
    }

    public string GetTimerText()
    {
        long TimeLeft = TimePerTurn - timer.ElapsedMilliseconds;
        if (TimeLeft > 0)
        {
            return (TimeLeft / 1000).ToString();
        }
        else
            return 0.ToString();
    }

	// Update is called once per frame
	void Update () {
        long TimeLeft = TimePerTurn - timer.ElapsedMilliseconds;
        if (TimeLeft < 0)
        {
            if (Global.GameInstance.CurrentTurn)
            {
                Global.GameInstance.EndTurn();
            }
        }
	}
}
