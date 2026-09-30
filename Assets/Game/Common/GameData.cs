using Newtonsoft.Json;
using System;
using System.Collections.Generic;
[Serializable]
public class GameData
{
    [JsonRequired]
    [JsonProperty("status")]
    public string Status { get; set; }  // keep string because JSON gives "true"
    [JsonRequired]
    [JsonProperty("message")]
    public string Message { get; set; }
    [JsonRequired]
    [JsonProperty("username")]
    public string Username { get; set; }
    [JsonRequired]
    [JsonProperty("credit")]
    public float Credit { get; set; }
    [JsonRequired]
    [JsonProperty("bet_details")]
    public List<BetDetail> BetDetails { get; set; }
    [JsonRequired]
    [JsonProperty("amounts")]
    public Amounts Amounts { get; set; }
    [JsonRequired]
    [JsonProperty("win_history")]
    public List<WinHistory> WinHistory { get; set; }
    [JsonRequired]
    [JsonProperty("remaining_time")]
    public int RemainingTime { get; set; }
    [JsonRequired]
    [JsonProperty("session")]
    public string Session { get; set; }
    [JsonRequired]
    [JsonProperty("win_no")]
    public WinNumber WinNo { get; set; }   // FIXED: single object, not List
    [JsonRequired]
    [JsonProperty("game_id")]
    public string GameId { get; set; }
    [JsonRequired]
    [JsonProperty("parent_active")]
    public bool ParentActive { get; set; }  // your JSON has true/false
    [JsonRequired]
    [JsonProperty("next_draw")]
    public string NextDrawTime { get; set; }
    [Serializable]
    public class BetDetail
    {
        public string ref_no;
        public string username;
        public string game_id;
        public float bet_amount;
        public float win_amount;
        public string time_stamp;
        public string win_no;
        public List<int> bet_array;
        public float credit_before_bet;
        public float credit_after_bet;
        public string session;
    }
}
[Serializable]
public class WinHistory
{
    [JsonRequired]
    [JsonProperty("win_no")]
    public int WinNo { get; set; }
    [JsonRequired]
    [JsonProperty("time")]
    public string Time { get; set; }
}
[Serializable]
public class WinNumber
{
    [JsonRequired]
    [JsonProperty("win_no")]
    public int WinNo { get; set; }
    [JsonRequired]
    [JsonProperty("time")]
    public string Time { get; set; }
}