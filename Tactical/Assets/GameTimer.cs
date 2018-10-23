using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour {
    public long TimePerTurn = 30000;
    public Text TimerText;

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

	// Update is called once per frame
	void Update () {
        long TimeLeft = TimePerTurn - timer.ElapsedMilliseconds;
        if (TimeLeft >= 0)
        {
            TimerText.text = (TimeLeft / 1000).ToString();
        }
        else
        {
            if (Global.GameInstance.CurrentTurn)
            {
                Global.GameInstance.EndTurn();
            }
        }
	}
}
