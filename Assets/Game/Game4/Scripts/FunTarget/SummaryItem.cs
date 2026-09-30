using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FunTarget
{
    public class SummaryItem : MonoBehaviour
    {
        public TMP_Text playText, wonText, startPointText, endPointText, statusText, resultText;
        public Button viewButton;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        public void SetValues(string play, string won, string startPoint, string endPoint, string status, string result)
        {
            playText.text = play;
            wonText.text = won;
            startPointText.text = startPoint;
            endPointText.text = endPoint;
            statusText.text = status;
            resultText.text = result;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

