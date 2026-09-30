using TMPro;
using UnityEngine;
namespace FunTarget
{
    public class ResultItem : MonoBehaviour
    {
        public TMP_Text timeText, winNoText;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        public void SetResultText(string time, string winNo)
        {
            timeText.text = time;
            winNoText.text = winNo;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

