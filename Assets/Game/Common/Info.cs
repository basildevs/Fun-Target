
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public class Info 
{
    [JsonProperty("status")]
    [JsonRequired]
    public string Status { get; set; }

    [JsonProperty("data")]
    [JsonRequired]
    public List<Data> data { get; set; }

    [Serializable]
    public class Data
    {
        //[JsonProperty("id")]
        //[JsonRequired]
        //public int id;
        //[JsonProperty("game")]
        //[JsonRequired]
        //public string game;
        //[JsonProperty("session")]
        //[JsonRequired]
        //public string session;
        [JsonProperty("win_no")]
        [JsonRequired]
        public int win_no;
        [JsonProperty("time")]
        [JsonRequired]
        public string time;
    }
}
