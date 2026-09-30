using FunTarget;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FunTargetHistoryItem : HistoryItem
{
    public TMP_Text itemText;

    public override void SetValue<T>(T value)
    {
        Data data = value as Data;
        itemText.text = data.winValue;
        transform.name = data.winValue;
        //transform.SetSiblingIndex(0);
    }
    public class Data
    {
        public string winValue;
    }
}
