using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
[Serializable]
public class BetDataClass
{
    public string type;
    public int[] bet_array;
    public string bet_amount;
    public string session;
}
[Serializable]
public class BetMessage
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("response")]
    public string Response { get; set; }

    [JsonProperty("data")]
    public BetData Data { get; set; }

    [JsonProperty("session")]
    public string Session { get; set; }
}

[Serializable]
public class BetData
{
    [JsonProperty("ref_no")]
    public string RefNo { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("time_stamp")]
    public string TimeStamp { get; set; }

    [JsonProperty("game_id")]
    public string GameId { get; set; }

    [JsonProperty("bet_amount")]
    public string BetAmount { get; set; }

    [JsonProperty("win_amount")]
    public double WinAmount { get; set; }

    [JsonProperty("win_no")]
    public string WinNo { get; set; }

    [JsonProperty("bet_array")]
    public List<int> BetArray { get; set; }

    [JsonProperty("bet_weight")]
    public List<int> BetWeight { get; set; }

    [JsonProperty("credit_before_bet")]
    public float CreditBeforeBet { get; set; }

    [JsonProperty("credit_after_bet")]
    public float CreditAfterBet { get; set; }

    [JsonProperty("session")]
    public string Session { get; set; }

    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonProperty("claim")]
    public string Claim { get; set; }

    [JsonProperty("bonus")]
    public string Bonus { get; set; }
}