using System.Collections.Generic;
using System;
using Newtonsoft.Json;

[Serializable]
public class Claim
{
    public string type;
}

[Serializable]
public class ClaimResult
{
    [JsonRequired]
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonRequired]
    [JsonProperty("status")]
    public bool Status { get; set; }

    [JsonRequired]
    [JsonProperty("claim")]
    public int Claim { get; set; }

    [JsonRequired]
    [JsonProperty("remaining_time")]
    public int RemainingTime { get; set; }

    [JsonRequired]
    [JsonProperty("win_history")]
    public List<WinHistory> WinHistory { get; set; }

    [JsonRequired]
    [JsonProperty("credit")]
    public float Credit { get; set; }

    [JsonRequired]
    [JsonProperty("win_no")]
    public WinNumber WinNo { get; set; }
}
