using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
[System.Serializable]
public class WinData
{
    [JsonProperty("type")]
    [JsonRequired]
    public string Type { get; set; }
    [JsonProperty("status")]
    [JsonRequired]
    public string Status { get; set; }
    [JsonProperty("message")]
    [JsonRequired]
    public string Message { get; set; }
    [JsonProperty("session")]
    [JsonRequired]
    public string Session { get; set; }
    [JsonProperty("win_no")]
    [JsonRequired]
    public int WinNo { get; set; }
    [JsonProperty("bonus")]
    [JsonRequired]
    public int Bonus { get; set; }
    [JsonProperty("credit")]
    [JsonRequired]
    public float Credit { get; set; }
    [JsonProperty("seconds")]
    [JsonRequired]
    public int Seconds { get; set; }
    [JsonProperty("amounts")]
    [JsonRequired]
    public Amounts Amounts { get; set; }
    [JsonProperty("win_history")]
    [JsonRequired]
    public List<WinHistory> WinHistory { get; set; }
    [JsonProperty("next_draw")]
    [JsonRequired]
    public string NextDraw { get; set; }
    public WinData()
    {
        Amounts = new Amounts();
        WinHistory = new List<WinHistory>();
    }
}