using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetHistory : Singleton<BetHistory>
{
    public Transform winHistoryParent;
    public HistoryItem historyItemPrefab;
    public List<HistoryItem> historyList;
    public int maxCount = 9;
    

    public void ShowData(string test)
    {
        Debug.Log("est");
    }

    public void ShowHistory<T>(T data)
    {
        HistoryItem historyItem = Instantiate(historyItemPrefab, winHistoryParent);
        historyItem.SetValue(data);
        //historyList.Insert(0, historyItem);
       historyList.Add(historyItem);

        if (historyList.Count > maxCount)
        {
            Destroy(historyList[0].gameObject);
            historyList.RemoveAt(0);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (HistoryItem history in historyList)
            {
                Destroy(history.gameObject);
            }
            historyList.Clear();
        }
        
    }
}
