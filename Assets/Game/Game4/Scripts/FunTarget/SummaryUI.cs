using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace FunTarget
{
    public class SummaryUI : MonoBehaviour
    {
        public List<SummaryItem> spawnedItems = new List<SummaryItem>();
        public SummaryItem summaryItemPrefab;
        public Transform summaryParent;
        public string response;
        public SingleBetDetailsUI singleBetDetailsWindow;
        private string apiUrl = "http://demo.dream36.club/GATEWAY/GAME02/bet_data_1hour/";
        public bool isLoading = false;
        public TMP_Text statusText;

        private void OnEnable()
        {
            // Instead of WebSocket, call REST API
            StartCoroutine(GetBetSummaryFromAPI());
        }

        private IEnumerator<UnityWebRequestAsyncOperation> GetBetSummaryFromAPI()
        {
            if (isLoading) yield break;
            isLoading = true;
            statusText.gameObject.SetActive(true);
            statusText.text = "Waiting to Fetch Data from the Server...";

            using (UnityWebRequest www = UnityWebRequest.Get(apiUrl+ "gAAAAABqVd5NlhjCChxpZp0dYkTKQAAqGreIB066DHM_MnAvKKDQbRXx9CUbnxO1A1zPAktVKJfnD2tSx35DwWAl7tOn6RsooX6QM48MfoSUTJqDI4HyKfGMVFYajBR3CTMsLrHiRtGBtAwWCi-4b_27qGUgzxaEQrGZuK5T9VpoCntYrSJKDHNgD6AXk58AbEbwP0JYkjZDEkb0noHN67sQ59g10l7L-A"))
            {
                // Send request
                yield return www.SendWebRequest();
                isLoading = false;

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    statusText.text = "Error fetching data!";
                    statusText.gameObject.SetActive(true);
                    Debug.LogError("Error fetching data: " + www.error);
                }
                else
                {
                    statusText.text = string.Empty;
                    statusText.gameObject.SetActive(false);
                    // Success — get response string
                    string json = www.downloadHandler.text;
                    OnResponse(json);
                }
            }
        }

        private void OnResponse(string response)
        {
            Debug.Log("Bet info response: " + response);
            ClearCurrentList();
            this.response = response;

            if (!response.TryParse(out JArray _root) || _root == null || _root.Count == 0)
                return;

            foreach (JObject item in _root)
            {
                SummaryItem instance = Instantiate(summaryItemPrefab, summaryParent);
                instance.SetValues(
                    item["bet_amount"].ToString(),
                    item["win_amount"].ToString(),
                    item["credit_before_bet"].ToString(),
                    item["credit_after_bet"].ToString(),
                    item["win_amount"].ToString() == "0" ? "no win" : "win",
                    item["win_no"].ToString()
                );
                spawnedItems.Add(instance);
            }

            foreach (SummaryItem summaryItem in spawnedItems)
            {
                summaryItem.viewButton.onClick.AddListener(() =>
                {
                    int index = spawnedItems.IndexOf(summaryItem);
                    OnSummaryItemClick(index, _root);
                });
            }
        }

        private void OnSummaryItemClick(int index, JArray _root)
        {
            singleBetDetailsWindow.ClearCurrentList();
            singleBetDetailsWindow.gameObject.SetActive(true);
            JObject data = (JObject)_root[index];

            // Parse cleaned_bet field
            string cleanedBetString = data["cleaned_bet"].ToString();
            JArray cleanedBetArray = JArray.Parse(cleanedBetString);
            int[] cleanedBet = cleanedBetArray.ToObject<int[]>();

            for (int i = 0; i < cleanedBet.Length; i++)
            {
                singleBetDetailsWindow.ShowData("", i.ToString(), cleanedBet[i].ToString());
            }
        }

        private void ClearCurrentList()
        {
            foreach (var item in spawnedItems)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
            spawnedItems.Clear();
        }
    }
}
