using UnityEngine;
using Newtonsoft.Json;
[System.Serializable]
public class TimeData
{
    [JsonProperty("type")]
    [JsonRequired] // Ensures "type" must be present in the JSON
    public string Type { get; set; }

    [JsonProperty("status")]
    [JsonRequired] // Ensures "status" must be present in the JSON
    public string Status { get; set; }

    [JsonProperty("remaining_seconds")]
    [JsonRequired] // Ensures "remaining_seconds" must be present in the JSON
    public int RemainingSeconds { get; set; }

    [JsonProperty("session")]
    [JsonRequired] // Ensures "session" must be present in the JSON
    public string Session { get; set; }

}