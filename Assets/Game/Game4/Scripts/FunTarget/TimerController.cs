using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class TimerController : Singleton<TimerController>
{
    public GameType gameType;
    public TMP_Text timerText;
    private float currentTime;
    public float totalTime;
    public bool timerRunning = false;
    public Action OnTimerFinished;
    private float lastTimeReceived;
    public Action OnTimeReceiveError;
    private readonly StringBuilder timeStringBuilder = new StringBuilder();
    public WebSocketEvent webSocketEvent;

    private float lastServerTime;
    private float lastCheckTime;

    private bool wasConnected;
    private bool hasSyncedOnce;
    private float connectedAt;
    private const float SyncGraceSeconds = 5f;

    public float CurrentTime { get => currentTime; set => currentTime = value; }

    // Start is called before the first frame update
    private void Awake()
    {
        //currentTime = totalTime;
    }
    void Start()
    {
        webSocketEvent.OnTimeDataReceieved += OnTimerDataReceived;
        wasConnected = WebSocketEditor.Instance != null && WebSocketEditor.Instance.isConnected;
        if (wasConnected)
        {
            connectedAt = Time.time;
            hasSyncedOnce = false;
            lastCheckTime = Time.time;
        }
    }

    private void OnTimerDataReceived(TimeData data)
    {
        currentTime = data.RemainingSeconds;
        lastTimeReceived = Time.time;
        lastServerTime = data.RemainingSeconds;
        lastCheckTime = Time.time;
        hasSyncedOnce = true;
    }

    // Update is called once per frame
    void Update()
    {
        bool isConnected = WebSocketEditor.Instance != null && WebSocketEditor.Instance.isConnected;
        if (timerRunning)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                DisplayTime(Mathf.Max(0, currentTime));
            }
            else
            {
                currentTime = 60;
                OnTimerFinished?.Invoke();
            }

            if (timerRunning && isConnected && hasSyncedOnce && (Time.time - connectedAt) > SyncGraceSeconds)
            {
                if (Time.time - lastCheckTime >= 10f)
                {
                    lastCheckTime = Time.time;
                    float differenceInTiming = Mathf.Abs(currentTime - lastServerTime);
                    Debug.Log("Difference " + differenceInTiming);

                    if (differenceInTiming >= 13f)
                    {
                        Debug.Log("[WebSocketEvent] Timing difference too high, syncing error.");
                        WebSocketEvent.Instance.OnErrorOccured?.Invoke();
                    }
                }
            }

        }

    }
    public void ResyncTimerAfterClaim(float serverTime)
    {
        lastServerTime = serverTime;
        lastCheckTime = Time.time;
        connectedAt = Time.time;
        hasSyncedOnce = true;
        Debug.Log("[Timer] Resynced after claim.");
    }
    public void Reset()
    {
        Debug.Log("Reset");
        // timerRunning = true;
    }

    void DisplayTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);

        timeStringBuilder.Clear();

        if (gameType == GameType.FunTarget)
        {
            timeStringBuilder.AppendFormat("{0:00}:{1:00}", minutes, seconds);
        }
        else if (gameType == GameType.HorseRace)
        {
            timeStringBuilder.Append(Mathf.FloorToInt(timeInSeconds));
        }

        timerText.text = timeStringBuilder.ToString();


    }
}
