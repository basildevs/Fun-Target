using System.Net.WebSockets;
using UnityEngine;
using System.Threading;
using System.Text;
using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using FunTarget;
public class WebSocketEditor : MonoBehaviour
{
    private ClientWebSocket _webSocket;
    private bool winGenerated;
    private string longResponse;
    public static WebSocketEditor Instance;
    string uri = "wss://demo.dream36.club/GATEWAY/GAME02/ws?token=";
    public Action<string> OnResponse;
    public Action<string> OnGameSummary;
    public bool isConnected = false;
    string fullURL;
    public WebSocketEvent webSocketEvent;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    async void Start()
    {
        Debug.Log("Start");
        _webSocket = new ClientWebSocket();
        //string url = UserDetails.Instance.accessTokenDetail;
        string url = "gAAAAABqVd5NlhjCChxpZp0dYkTKQAAqGreIB066DHM_MnAvKKDQbRXx9CUbnxO1A1zPAktVKJfnD2tSx35DwWAl7tOn6RsooX6QM48MfoSUTJqDI4HyKfGMVFYajBR3CTMsLrHiRtGBtAwWCi-4b_27qGUgzxaEQrGZuK5T9VpoCntYrSJKDHNgD6AXk58AbEbwP0JYkjZDEkb0noHN67sQ59g10l7L-A";
        fullURL = uri + url;
        try
        {
            Debug.Log("Connecting...");
            await _webSocket.ConnectAsync(new Uri(fullURL), CancellationToken.None);
            Debug.Log("Connected");
            isConnected = true;
            ReceiveMessages();
        }
        catch (Exception e)
        {
            Debug.LogError($"WebSocket connection error: {e.Message}");
            webSocketEvent.OnErrorOccured?.Invoke();
        }
    }
    private async void ReceiveMessages()
    {
        var buffer = new byte[1024 * 4];
        while (_webSocket.State == WebSocketState.Open)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                WebSocketReceiveResult result;
                do
                {
                    result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    string chunk = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    stringBuilder.Append(chunk);
                }
                while (!result.EndOfMessage);
                if (_webSocket.State == WebSocketState.Closed) return;
                string message = stringBuilder.ToString();
                if (message.TryParse(out JObject messageObject))
                {
                    //if (messageObject.ContainsKey("message") &&
                    //    messageObject["message"].ToString().Equals("You are blocked", StringComparison.OrdinalIgnoreCase) ||
                    //    messageObject.ContainsKey("message") && messageObject["message"].ToString().Equals("Game is blocked", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    Debug.Log("User is blocked: " + messageObject["message"]);
                    //    GameController.Instance.GameUnavailable();
                    //}

                    if (messageObject.ContainsKey("status") && (messageObject["status"].ToString() == "false"))
                    {
                        Debug.Log("status: " + messageObject["message"].ToString());
                        GameController.Instance.GameUnavailable();
                    }

                    if (messageObject.ContainsKey("type") && messageObject["type"].ToString() == "ping")
                    {
                        string sendMessage = "{\"type\":\"pong\"}";
                        Debug.Log("Ping received. Sending pong...");
                        SendMessageToServer(sendMessage);
                        continue;
                    }
                }
                Debug.Log($"Full message received:\n{message}");
                if (message.TryParse(out GameData gameData))
                {
                    WebSocketEvent.Instance.OnGameLoaded?.Invoke(gameData);
                }
                else if (message.TryParse(out TimeData timeData))
                {
                    if (timeData.RemainingSeconds > 0)
                    {
                        WebSocketEvent.Instance.OnTimeDataReceieved?.Invoke(timeData);
                    }
                }
                else if (message.TryParse(out WinData winData))
                {
                    if (!winGenerated)
                    {
                        WebSocketEvent.Instance.OnWinDataReceived?.Invoke(winData);
                        StartCoroutine(ResetWin());
                    }
                }
                else if (message.TryParse(out ClaimResult claimResult))
                {
                    WebSocketEvent.Instance.OnAmountClaimed?.Invoke(claimResult);
                }

                else if (message.TryParse(out BetMessage betMessage))
                {
                    if (betMessage.Response != null &&
                        betMessage.Response.Contains("bet placed successfully", StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.Log($"Bet Amount: {betMessage.Data.CreditAfterBet}");
                        GameController.Instance.DefaultBalance = betMessage.Data.CreditAfterBet;
                    }
                    else if (betMessage.Response.Contains("Insufficient balance", StringComparison.OrdinalIgnoreCase))
                    {
                        BalanceController.Instance.NotEnoughBalance();
                    }
                }

                else if (message.TryParse(out BetPlaced betPlaced))
                {
                    // Handle betPlaced if needed
                }
                else if (message.TryParse(out Info betInfo))
                {
                    WebSocketEvent.Instance.OnBetInfoReceived?.Invoke(betInfo);
                }
                else if (message == "WebSocket connection opened")
                {
                    // Optional connection message
                }
            }
            catch (Exception e)
            {
                Debug.Log($"WebSocket receive error: {e?.Message}");
                //isConnected = false;
                //webSocketEvent.OnErrorOccured?.Invoke();
            }
        }
    }
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
    private IEnumerator ResetWin()
    {
        winGenerated = true;
        yield return new WaitForSeconds(5f);
        winGenerated = false;
    }
    public void PlaceBet(BetDataClass betData)
    {
        Debug.Log("Bet data:" + JsonUtility.ToJson(betData));
        SendMessageToServer(JsonUtility.ToJson(betData));
    }
    public void SendMessageToServer(string message)
    {
        SendWebSocketMessage(message);
        Debug.Log($"WebSocket send: {message}");
    }
    private async void SendWebSocketMessage(string message)
    {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
        {
            try
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                await _webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                Debug.Log($"Message sent: {message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"WebSocket send error: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("Cannot send message: WebSocket is not open.");
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Current state: " + _webSocket.State);
        }
    }
    private async void OnDestroy()
    {
        Debug.Log("OnDestroy");
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            _webSocket.Dispose();
        }
    }
    private async void OnApplicationQuit()
    {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            _webSocket.Dispose();
        }
    }
}
public static class WebsocketExtension
{
    public static bool TryParse<T>(this string json, out T parsedData)
    {
        try
        {
            parsedData = JsonConvert.DeserializeObject<T>(json);
            return true;
        }
        catch
        {
            parsedData = default;
            return false;
        }
    }
}