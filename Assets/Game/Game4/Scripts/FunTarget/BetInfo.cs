
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FunTarget
{
    public class BetInfo : Singleton<BetInfo>
    {
        public Button infoButton;
        public Button closeButton;
        public GameObject popupWindow;
        [Header("Result")]
        public Button resultButton;
        public GameObject resultWindow;
        [Header("Summary")]
        public Button summaryButton;
        public GameObject summaryWindow;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            popupWindow.SetActive(false);
            infoButton.onClick.AddListener(ShowWindow);
            closeButton.onClick.AddListener(CloseWindow);
            resultButton.onClick.AddListener(() => {
                HideAll();
                resultWindow.SetActive(true);
            });
            summaryButton.onClick.AddListener(() => {
                HideAll();
                summaryWindow.SetActive(true);
            });
        }
        private void HideAll()
        {
            resultWindow.SetActive(false);
            summaryWindow.SetActive(false);
        }

        //private void ShowView()
        //{
        //    viewWindow.SetActive(true);
        //    for(int i = 0; i < 10; i++)
        //    {
        //        GameObject instance = Instantiate(viewItemPrefab, viewItemParent);
        //    }
        //     WebSocket.Instance.SendMessageToServer("{\"type\":\"bet_data_1hour\"}");

        //}

        public void ShowWindow()
        {
            popupWindow.SetActive(true);
            resultWindow.SetActive(true);
        }
        public void CloseWindow()
        {
            popupWindow.SetActive(false);
            resultWindow.SetActive(false);
            summaryWindow.SetActive(false);
        }

        void Update()
        {
            if (TimerController.Instance.CurrentTime < 10 && popupWindow.activeSelf)
            {
                CloseWindow();
            }
        }

    }
}

