using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class BetDetails
{
    public string ref_no;
    public string username;
    public string time_stamp;
    public string game_id;
    public string bet_amount;
    public int win_amount;
    public string win_no;
    public List<int> bet_array;
    public float credit_before_bet;
    public float credit_after_bet;
    public string session;
    public string timestamp;
    public string claim;
}
