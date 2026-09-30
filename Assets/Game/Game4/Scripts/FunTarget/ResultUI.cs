using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;
using UnityEngine.Networking;

namespace FunTarget
{
    public class ResultUI : MonoBehaviour
    {
        public List<ResultItem> spawnedItems;
        public ResultItem resultItemPrefab;
        public Transform resultParent;
        private string apiURL = "http://demo.dream36.club/GATEWAY/GAME02/result_history/";
        private bool isLoading = false;
        public TMP_Text statusText;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }
        private void OnEnable()
        {
            //WebSocketEditor.Instance.SendMessageToServer("{\"type\":\"info\"}");

            //WebSocketEvent.Instance.OnBetInfoReceived += OnBetInfoReceieved;
            StartCoroutine(GetBetInfoFromAPI());

        }

        private IEnumerator<UnityWebRequestAsyncOperation> GetBetInfoFromAPI()
        {
            if (isLoading) yield break;
            isLoading = true;
            statusText.gameObject.SetActive(true);
            statusText.text = "Waiting to Fetch Data from the Server...";

            using (UnityWebRequest www = UnityWebRequest.Get(apiURL + "gAAAAABqVd5NlhjCChxpZp0dYkTKQAAqGreIB066DHM_MnAvKKDQbRXx9CUbnxO1A1zPAktVKJfnD2tSx35DwWAl7tOn6RsooX6QM48MfoSUTJqDI4HyKfGMVFYajBR3CTMsLrHiRtGBtAwWCi-4b_27qGUgzxaEQrGZuK5T9VpoCntYrSJKDHNgD6AXk58AbEbwP0JYkjZDEkb0noHN67sQ59g10l7L-A"))
            {
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
                    string json = www.downloadHandler.text;
                    List<Info.Data> dataList = JsonConvert.DeserializeObject<List<Info.Data>>(json);
                    OnBetInfoReceieved(dataList);
                }
            }
        }

        private void OnBetInfoReceieved(List<Info.Data> dataList)
        {
            ClearSpawnedItems();

            foreach (var data in dataList)
            {
                ResultItem resultItem = Instantiate(resultItemPrefab, resultParent);
                spawnedItems.Add(resultItem);
                resultItem.SetResultText(data.time, data.win_no.ToString());
            }
        }

        private void ClearSpawnedItems()
        {
            foreach (ResultItem item in spawnedItems)
                Destroy(item.gameObject);
            spawnedItems.Clear();
        }

        private void OnDisable()
        {
            //WebSocketEvent.Instance.OnBetInfoReceived -= OnBetInfoReceieved;
        }
        // Update is called once per frame
        void Update()
        {

        }
    }

}

