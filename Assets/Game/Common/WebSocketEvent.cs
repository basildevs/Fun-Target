using System;
using UnityEngine;

[CreateAssetMenu(fileName ="WebSocketEvents",menuName ="ScriptableObjects/WebSocketEvents")]
public class WebSocketEvent : ScriptableObject
{
    public Action<GameData> OnGameLoaded;
    public Action<WinData> OnWinDataReceived;
    public Action<TimeData> OnTimeDataReceieved;
    public Action<ClaimResult> OnAmountClaimed;
    public Action<Info> OnBetInfoReceived;
    public Action<string> OnBetSummaryReceived;
    public Action OnNextGameplayStarting;
    public Action OnErrorOccured;

    public static WebSocketEvent instance;
    public static WebSocketEvent Instance { get {
            if (instance == null) {
                instance = Resources.Load<WebSocketEvent>("WebSocketEvents");
            }
            return instance; 
        
        } }
}
