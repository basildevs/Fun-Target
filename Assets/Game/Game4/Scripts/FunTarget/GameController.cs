// DEBUGGING READ-ME: DO NOT EDIT THIS AUTO-GENERATED FILE AS IT'LL BE DELETED//
// To debug simply add a breakpoint in this file.
// 
// With every code change - new file is generated, currently you'll need to re-set breakpoints after each change.
// You can also:
//    - step into the function that was changed (and that will get you to correct source file)
//    - add a function breakpoint in your IDE (this way you won't have to re-add it every time)
//
// Tool can automatically open dynamically-compiled code file every time to make setting breakpoints easier.
// You can adjust that behaviour via 'Window -> FastScriptReload -> Start Screen -> Debugging -> Do not auto-open generated cs file'.
//
// You can always open generated file when needed by clicking link in console, eg.
// 'FSR: Files: FunctionLibrary.cs changed (click here to debug [in bottom details pane]) - compilation (took 240ms)'


using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameData;
using Random = UnityEngine.Random;

namespace FunTarget
{
    public class GameController : Singleton<GameController>
    {
        [SerializeField]
        private GameObject inputBlock;
        public TMP_Text winAmountText;
        public Button collectWinAmountButton;
        public int winAmount = 100;
        public Button exitButton;
        public GameObject loadingScreen;
        public TMP_Text bottomText;
        public GameObject networkErrorPopup;
        public GameObject gameUnavailablePopup;
        public GameObject slowNetworkPoup;
        public Button exitButtonPopup;
        public GameObject timerGlow;
        public WebSocketEvent webSocketEvent;
        private float defaultBalance;

        public float DefaultBalance
        {
            get => defaultBalance;
            set
            {
                defaultBalance = value;
                Debug.Log("[NEW CREDIT]: " + value);
            }
        }

        // Start is called before the first frame update
        public bool InputBlockActive()
        {
            return inputBlock.activeSelf;
        }
        private void OnDisable()
        {

            webSocketEvent.OnErrorOccured -= OnErrorrOccured;
            webSocketEvent.OnGameLoaded -= OnGameLoaded;
            webSocketEvent.OnTimeDataReceieved -= OnTimerDataReceived;
            //TimerController.Instance.OnTimeReceiveError -= ExitGame;
        }

        public void OnError()
        {
            //     exitWindow.SetActive(true);
            //    TimerController.Instance.timerRunning = false;
        }

        public void ShowInputBlock(bool state)
        {
            if (inputBlock == null || inputBlock.activeSelf == state) return;
            // Debug.Log("Input block: " + state);
            inputBlock.SetActive(state);
        }
        void Start()
        {
            exitButtonPopup.onClick.AddListener(ExitGame);
            // loadingScreen.SetActive(true);
            //textList.Reverse();
            webSocketEvent.OnErrorOccured += OnErrorrOccured;
            webSocketEvent.OnGameLoaded += OnGameLoaded;
            webSocketEvent.OnTimeDataReceieved += OnTimerDataReceived;
            //TimerController.Instance.OnTimeReceiveError += ExitGame;
            collectWinAmountButton.onClick.AddListener(CollectWinAmount);
            //    TimerController.Instance.OnTimerFinished += OnTimerFinished;
            exitButton.onClick.AddListener(ExitGame);
            //DefaultBalance = float.Parse(UserDetails.Instance.balanaceAmount);
        }


        private void OnTimerDataReceived(TimeData data)
        {
            FunTargetData.Instance.session = data.Session;
        }
        public void OnErrorrOccured()
        {
            networkErrorPopup.SetActive(true);
            networkErrorPopup.GetComponent<Button>().onClick.AddListener(() => ExitGame());
            AudioListener.volume = 0f;
            TimerController.Instance.timerRunning = false;
            Wheel.Instance.canRotate = false;
        }
        public void ExitGame()
        {
            Debug.Log("From unity: Exit game");
            SceneManager.LoadScene("Login");
            WebSocketEditor.Instance.DestroyObject();
            Debug.Log("From unity: Exit game");
            //UserDetails.Instance.balanaceAmount = DefaultBalance.ToString("F2");
        }
        public void SlowNetwork()
        {
            slowNetworkPoup.SetActive(true);
            slowNetworkPoup.GetComponent<Button>().onClick.AddListener(() => ExitGame());
            AudioListener.volume = 0f;
            TimerController.Instance.timerRunning = false;
            Wheel.Instance.canRotate = false;
        }
        public void GameUnavailable()
        {
            gameUnavailablePopup.SetActive(true);
            gameUnavailablePopup.GetComponent<Button>().onClick.AddListener(() => ExitGame());
            AudioListener.volume = 0f;
            TimerController.Instance.timerRunning = false;
            Wheel.Instance.canRotate = false;
            Debug.Log("From unity: Exit game");
        }
        private void CollectWinAmount()
        {
            BalanceController.Instance.AddToBalance(winAmount);
        }

        private void OnGameLoaded(GameData gameData)
        {
            FunTargetData.Instance.session = gameData.Session;
            if (gameData.Status == "true")
            {

                List<WinHistory> winHistory = gameData.WinHistory;
                for (int i = 0; i < winHistory.Count; i++)
                {
                    ConvertAndShowHistory(winHistory[i]);
                }
                DefaultBalance = gameData.Credit;
                // usernameText.text = gameData.Username;
                // loadingScreen.SetActive(false);
                TimerController.Instance.CurrentTime = gameData.RemainingTime;
                //BetValueController.Instance.TotalScore = gameData.Credit;
                BalanceController.Instance.SetBalance(gameData.Credit);
                //UpdateScoreDisplay();
            }
            else
            {
                Debug.Log("Status false");
            }

        }
        public void ConvertAndShowHistory(WinHistory winHistory)
        {
            BetHistory.Instance.ShowHistory(new FunTargetHistoryItem.Data()
            {
                winValue = winHistory.WinNo.ToString()
            });
        }

        // Update is called once per frame
        void Update()
        {
            if (TimerController.Instance.timerRunning && TimerController.Instance.CurrentTime < 15 && TimerController.Instance.CurrentTime >= 10)
            {
                if (!timerGlow.activeSelf) timerGlow.SetActive(true);
            }
            else
            {
                if (timerGlow.activeSelf) timerGlow.SetActive(false);
            }

        }

        public void UpdateBottomText(string text)
        {
            bottomText.text = text;
        }
    }
}


