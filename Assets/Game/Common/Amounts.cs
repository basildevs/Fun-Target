using UnityEngine;
using Newtonsoft.Json;
[System.Serializable]
public class Amounts
{
    [JsonProperty("total_bet")]  // Maps this field to "total_bet" in the JSON
    [JsonRequired]              // Field is required in the JSON
    public float TotalBet { get; set; }

    [JsonProperty("total_win")]
    [JsonRequired]
    public float TotalWin { get; set; }
}