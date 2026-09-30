using FunTarget;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameData;

public class BetPlacement : MonoBehaviour
{
    [Header("BET VALUES")]
    [SerializeField] Button[] topButtons;

    [Header("PRINT THE BET VALUES")]
    [SerializeField] Button[] bottomButtons;

    [Header("CANCEL BUTTONS MANAGER")]
    [SerializeField] Button cancelBetButton;
    [SerializeField] Button cancelSpecificBetButton;

    [Header("ANIMATOR")]
    [SerializeField] Animator[] cancelBetAnimators;

    [Header("COUNT DISPLAY")]
    [SerializeField] TextMeshProUGUI countText;
    private int filledButtonCount = 0;
    private int sumOfBets;
    // Default initial score, adjustable in the Inspector
    private float totalScore;


    private int lastBetIndex = -1;

    private Button selectedTopButton;

    private int selectedTopButtonIndex = -1;

    public Button betOkButton;
    [SerializeField]
    private List<Bet> prevBetList = new List<Bet>();
    [SerializeField]
    private Button prevBetButton;

    public Button PrevBetButton { get => prevBetButton; set => prevBetButton = value; }
    public float TotalScore { get => totalScore; set => totalScore = value; }
    public int SumOfBets
    {
        get => sumOfBets;
        set => sumOfBets = value;
    }
    public Button[] BottomButtons { get => bottomButtons; set => bottomButtons = value; }
    public WinData LatestWinData { get => latestWinData; set => latestWinData = value; }
    public bool WinGenerated { get => winGenerated; set => winGenerated = value; }

    private bool betPlaced;
    private bool bottomButtonCliked;
    public BetResult betResult;
    private WinData latestWinData;
    private bool winGenerated;
    private bool init;
    public List<Bet> currentBetList;
    public TMP_Text winAmountText;
    public WebSocketEvent webSocketEvent;
    private void Start()
    {
        //populate test array with sample data

        latestWinData = null;

        OnTopButtonClick(0, topButtons[0]);
        betOkButton.onClick.AddListener(() => {

            AudioController.Instance.PlayAudio(true, "ClickAudio");
            PlaceBet();
        });
        for (int i = 0; i < topButtons.Length; i++)
        {
            Button button = topButtons[i];

            int index = i; // Capture the current index in a local variable
            button.onClick.AddListener(() => OnTopButtonClick(index, button)); // Add listeners for all top buttons
        }

        for (int i = 0; i < bottomButtons.Length; i++)
        {
            int index = i; // Capture the current index in a local variable
            bottomButtons[i].GetComponent<RightClick>().OnRightClicked += () => {
                ClearBottomButton(index);
            };
            bottomButtons[i].GetComponent<RightClick>().OnHold += () => {
                if (TimerController.Instance.CurrentTime < 10 || GameController.Instance.InputBlockActive()) {

                    return;
                }
                AudioController.Instance.PlayAudio(true, "ClickAudio");
                OnBottomButtonClick(index, GetSelectedChipValue());
                bottomButtonCliked = true;
            };
            //bottomButtons[i].onClick.AddListener(() => {

            //    OnBottomButtonClick(index, GetSelectedChipValue());
            //    bottomButtonCliked = true;
            //});
        }

        cancelBetButton.onClick.AddListener(() =>
        {
            ClearAllBottomButtons();
            currentBetList.Clear();
            prevBetList.Clear();
        }); // Add listener for the cancel all bets button

        cancelSpecificBetButton.onClick.AddListener(CancelLastAddedBet); // Add listener for the cancel specific bet button            
        prevBetButton.onClick.AddListener(PrevBet);
        UpdateSumOfBetsUI();
        Debug.Log(webSocketEvent == null);
        webSocketEvent.OnGameLoaded += OnGameLoaded;
        //WebSocketWebgl.Instance.urlCallback += OnLoadedUrl;
        TimerController.Instance.OnTimerFinished += OnTimerFinished;
        webSocketEvent.OnWinDataReceived += OnWinDataReceived;
        webSocketEvent.OnTimeDataReceieved += OnTimeDataReceived;

        GameController.Instance.UpdateBottomText("Your Minimum Bet value is 1");
        init = true;
    }
    private void OnEnable()
    {
        if (init == false) return;
        betOkButton.GetComponent<Image>().raycastTarget = true;
        betPlaced = false;
        latestWinData = null;
        WinGenerated = false;
        webSocketEvent.OnGameLoaded += OnGameLoaded;
        //WebSocketWebgl.Instance.urlCallback += OnLoadedUrl;
        TimerController.Instance.OnTimerFinished += OnTimerFinished;
        webSocketEvent.OnWinDataReceived += OnWinDataReceived;
        webSocketEvent.OnTimeDataReceieved += OnTimeDataReceived;
        GameController.Instance.ShowInputBlock(false);
        // Debug.Log("Enable bet placement");
        ClearBets(false);
        UpdateCancelBetAnimator();
        UpdateSumOfBetsUI();
        UpdateScoreDisplay();

        bottomButtonCliked = false;
        GameController.Instance.UpdateBottomText("Your Minimum Bet value is 1");
        // Debug.Log("Enable");
    }

    private void OnDisable()
    {
        GameController.Instance.ShowInputBlock(true);
        webSocketEvent.OnGameLoaded -= OnGameLoaded;
        //WebSocketWebgl.Instance.urlCallback -= OnLoadedUrl;
        TimerController.Instance.OnTimerFinished -= OnTimerFinished;
        webSocketEvent.OnWinDataReceived -= OnWinDataReceived;
        webSocketEvent.OnTimeDataReceieved -= OnTimeDataReceived;
        if (betOkButton == null) return;
        betOkButton.GetComponent<Image>().raycastTarget = false;
    }

    private void OnTimeDataReceived(TimeData timeData)
    {
        FunTargetData.Instance.session = timeData.Session;
    }

    private void OnGameLoaded(GameData gameData)
    {
        if (gameData.Status == "true")
        {
            //Wheel.Instance.ShowIndex(gameData.WinNo.WinNo);
            WheelController.Instance.StartWheelPoint(gameData.WinNo.WinNo);
            TotalScore = gameData.Credit;
            BalanceController.Instance.SetBalance(gameData.Credit);
            TimerController.Instance.CurrentTime = gameData.RemainingTime;
            TimerController.Instance.timerRunning = true;
            UpdateScoreDisplay();
            PendingBets(gameData);
        }
        else
        {
            Debug.Log("Status false");
        }

    }

    private void PendingBets(GameData gameData)
    {

        if (gameData.BetDetails != null && gameData.BetDetails.Count > 0)
        {
            BetDetail betDetail = gameData.BetDetails[gameData.BetDetails.Count - 1];

            if (betDetail != null && betDetail.bet_array.Count > 0)
            {
                if (betDetail.win_no == "Not Generated")
                {
                    // Debug.Log("::Bet array count: " + betDetail.bet_array.Count);
                    for (int i = 0; i < betDetail.bet_array.Count; i++)
                    {
                        // Debug.Log("::index: " + i + ": chipValue: " + betDetail.bet_array[i]);
                        if (betDetail.bet_array[i] > 0)
                        {
                            //OnBottomButtonClick(i, betDetail.bet_array[i]);
                            SetBottomButtonValue(i, betDetail.bet_array[i]);
                        }
                    }

                    sumOfBets = (int)betDetail.bet_amount;
                    UpdateSumOfBetsUI();
                    betPlaced = true;
                    GameController.Instance.ShowInputBlock(true);
                }
                else
                {
                    if (betDetail.win_amount > 0)
                    {
                        latestWinData ??= new WinData();
                        latestWinData.WinNo = gameData.WinNo.WinNo;
                        latestWinData.Amounts.TotalWin = gameData.Amounts.TotalWin;
                        latestWinData.Session = gameData.Session;
                        latestWinData.Credit = gameData.Credit;
                        Debug.Log("Win generated, win amount: " + betDetail.win_amount);
                        winAmountText.text = betDetail.win_amount.ToString();
                        sumOfBets = (int)betDetail.bet_amount;
                        UpdateSumOfBetsUI();
                        for (int i = 0; i < betDetail.bet_array.Count; i++)
                        {
                            // Debug.Log("::index: " + i + ": chipValue: " + betDetail.bet_array[i]);
                            if (betDetail.bet_array[i] > 0)
                            {
                                //OnBottomButtonClick(i, betDetail.bet_array[i]);
                                SetBottomButtonValue(i, betDetail.bet_array[i]);
                                string winNoStr = betDetail.win_no;
                                if (winNoStr.Contains("-"))
                                {
                                    winNoStr = winNoStr.Split('-')[0];
                                }
                                if (int.TryParse(winNoStr, out int winNoInt))
                                {
                                    if (i == winNoInt)
                                    {
                                        bottomButtons[i].GetComponent<ChangeButtonImage>().PlaySelectedAnim(true);
                                    }
                                }
                            }
                        }
                        betResult.gameObject.SetActive(true);
                        betResult.ShowTakeMoney();
                        gameObject.SetActive(false);
                    }

                }

            }
            //if (currentBetList.Count > 0)
            //    PlaceBet();
        }
    }
    public void PlaySelectedButtonAnim(int index)
    {
    }

    private void SetBottomButtonValue(int index, int value)
    {
        Button button = bottomButtons[index];
        button.GetComponent<ChangeButtonImage>().ChangeImage();
        TextMeshProUGUI bottomText = button.GetComponentInChildren<TextMeshProUGUI>();
        bottomText.text = value.ToString();
    }

    private void OnWinDataReceived(WinData winData)
    {
        //List<int> winHistory = winData.WinHistory;
        //winHistory.Reverse();
        //for (int i = 0; i < winHistory.Count; i++)
        //{
        //    BetHistory.Instance.ShowHistory(new FunTargetHistoryItem.Data() { winValue = winHistory[i].ToString() });
        //}
        if (winData.Bonus == 2)
            Wheel.Instance.BonusIndex = winData.Bonus;
        else Wheel.Instance.BonusIndex = 4;
        WinGenerated = true;
        latestWinData = winData;
        if (latestWinData.Amounts.TotalWin > 0 && latestWinData.Session != FunTargetData.Instance.session)
        {
            GameController.Instance.ExitGame();
            return;
        }
        Wheel.Instance.wheelIndex = winData.WinNo;

    }

    private void OnLoadedUrl(string url)
    {
      //  Debug.Log("Access token recieved: " + //WebSocketWebgl.Instance.GetURLParameter(url, "access_token"));
       // FunTargetData.Instance.userName = //WebSocketWebgl.Instance.GetURLParameter(url, "username");
      //  FunTargetData.Instance.accessToken = //WebSocketWebgl.Instance.GetURLParameter(url, "access_token");
       // FunTargetData.Instance.credit = //WebSocketWebgl.Instance.GetURLParameter(url, "credit");
      //  FunTargetData.Instance.gameId = //WebSocketWebgl.Instance.GetURLParameter(url, "game_id");
      //  FunTargetData.Instance.url2 = //WebSocketWebgl.Instance.GetURLParameter(url, "url");
      //  FunTargetData.Instance.status = //WebSocketWebgl.Instance.GetURLParameter(url, "status");
      //  FunTargetData.Instance.port = //WebSocketWebgl.Instance.GetURLParameter(url, "port");
        if (FunTargetData.Instance.credit != null && FunTargetData.Instance.credit != "")
        {
            totalScore = float.Parse(FunTargetData.Instance.credit);
            UpdateScoreDisplay();
        }
    }
    private void OnTimerFinished()
    {
        AudioController.Instance.PlayAudio(true, "wheelAudio");
        // Debug.Log("Timer finished");
        if (!WinGenerated)
        {
            Debug.Log("Win not generated!");
            GameController.Instance.SlowNetwork();
            //GameController.Instance.ExitGame();
        }
        StartCoroutine(WaitForWin());
        IEnumerator WaitForWin()
        {
            yield return new WaitUntil(() => latestWinData != null);
            //  Debug.Log("Timer finished");
            Wheel.Instance.SetRotate(true);
            GameController.Instance.UpdateBottomText("");
            //  TimerController.Instance.Reset();

            if (betPlaced == false)
            {
                prevBetList.Clear();
                currentBetList.Clear();
                RebetButtonState();
            }

            betResult.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

    }

    void OnTopButtonClick(int index, Button button)
    {
        // Reset previously selected button image if a different button is clicked
        //if (selectedTopButtonIndex != -1 && selectedTopButtonIndex != index)
        //{
        //    topButtons[selectedTopButtonIndex].GetComponent<ChangeButtonImage>().ResetImage();
        //}

        foreach(Button topButton in topButtons)
        {
            topButton.GetComponent<ChangeButtonImage>().ResetImage();
        }

        // Change the image of the clicked button
        topButtons[index].GetComponent<ChangeButtonImage>().ChangeImage();

        selectedTopButton = button;
        // Update the index of the currently selected top button
        selectedTopButtonIndex = index;
    }
    private void PlaceBet()
    {
        prevBetList.Clear();
        if (currentBetList.Count == 0)
        {
            return;
        }
        foreach (Bet bet in currentBetList)
        {
            prevBetList.Add(bet);
        }
        SendBetToServer();
        // Debug.Log("Bet placed");
        //  ClearBets(false);
        ShowCancelBetAnimation(false);
        UpdateSumOfBetsUI();
        betPlaced = true;
        GameController.Instance.ShowInputBlock(true);
        GameController.Instance.UpdateBottomText("Your bet has been accepted");
        //  Wheel.Instance.SetRotate(true);
    }

    private void SendBetToServer()
    {
        int[] betArray = new int[10];
        foreach (Bet bet in currentBetList)
        {
            if (bet.index > 9) continue;
            int current = betArray[bet.index];
            current += bet.chipValue;
            betArray[bet.index] = current;
        }
        BetDataClass betData1 = new BetDataClass();
        betData1.bet_array = betArray;
        betData1.type = "bet";
        betData1.bet_amount = countText.text;
        Debug.Log("Current session: " + FunTargetData.Instance.session);
        betData1.session = FunTargetData.Instance.session;
        WebSocketEditor.Instance.PlaceBet(betData1);
        currentBetList.Clear();
    }
    // Handle bottom button click
    void OnBottomButtonClick(int index, int chipValue)
    {


        int valueToAdd = 1;
        int topValue = chipValue;

        valueToAdd = topValue;
        //  Debug.Log("Total score: " + totalScore);
        //  Debug.Log("Value to add: " + valueToAdd);

        Button button = bottomButtons[index];
        TextMeshProUGUI bottomText = button.GetComponentInChildren<TextMeshProUGUI>();

        int currentValue = string.IsNullOrEmpty(bottomText.text) ? 0 : int.Parse(bottomText.text);
        if (totalScore - valueToAdd < 0)
        {
            BalanceController.Instance.NotEnoughBalance();
        }
        if (totalScore - valueToAdd < 0 || (currentValue + topValue) > 5000) return;
        bottomText.text = (currentValue + topValue).ToString();

        currentBetList.Add(new Bet() { index = index, chipValue = GetSelectedChipValue() });
        SumOfBets += valueToAdd; // Update the sum of bets
        totalScore -= valueToAdd;

        int buttonIndex = Array.IndexOf(bottomButtons, button);
        RecordBet(buttonIndex);

        bottomButtons[index].GetComponent<ChangeButtonImage>().ChangeImage();

        UpdateCancelBetAnimator();

        UpdateSumOfBetsUI();
        UpdateScoreDisplay();

    }

    public int GetSelectedChipValue()
    {
        return int.Parse(selectedTopButton.GetComponentInChildren<TextMeshProUGUI>().text);
    }
    public void ClearAllBottomButtons()
    {
        ClearBets();

        UpdateCancelBetAnimator();
        UpdateSumOfBetsUI();
        UpdateScoreDisplay();


    }


    public void CancelLastAddedBet()
    {
        if (currentBetList.Count > 0)
        {
            Bet lastBet = currentBetList[currentBetList.Count - 1];

            ClearBottomButton(lastBet.index);
        }
        UpdateCancelBetAnimator();
    }

    private void PrevBet()
    {
        prevBetButton.gameObject.SetActive(false);
        foreach (Bet bet in prevBetList)
        {
            if (totalScore - bet.chipValue < 0)
            {
                BalanceController.Instance.NotEnoughBalance();
                return;
            }
            ChangeButtonImage changeButtonImage = bottomButtons[bet.index].GetComponent<ChangeButtonImage>();
            changeButtonImage.ChangeImage();
            OnBottomButtonClick(Array.IndexOf(bottomButtons, bottomButtons[bet.index]), bet.chipValue);
        }
        prevBetList.Clear();
    }

    private void UpdateSumOfBetsUI()
    {
        if (countText != null)
        {
            countText.text = SumOfBets.ToString();
        }
    }
    private void UpdateScoreDisplay()
    {
        //if (scoreText != null)
        //{
        //    scoreText.text = totalScore.ToString("F2"); // Display score with two decimal places
        //}
        BalanceController.Instance.SetBalance(totalScore);

      
    }
    public void ShowCancelBetAnimation(bool status)
    {
        foreach (Animator animator in cancelBetAnimators)
        {
            animator.SetBool("Show", status);
        }
    }
    public void RecordBet(int index)
    {
        if (index >= 0 && index < bottomButtons.Length)
        {
            lastBetIndex = index;
            bottomButtons[index].GetComponent<ChangeButtonImage>().ChangeImage();
        }
    }
    private void UpdateCancelBetAnimator()
    {
        bool hasValue = false;
        foreach (Button button in bottomButtons)
        {
            TextMeshProUGUI bottomText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (bottomText != null && !string.IsNullOrEmpty(bottomText.text))
            {
                hasValue = true;
                break;
            }
        }

        ShowCancelBetAnimation(hasValue);

    }
    private void ClearBets(bool restoreTotal = true)
    {
       // Debug.Log("Clear bets");
        for (int i = 0; i < bottomButtons.Length; i++)
        {
            TextMeshProUGUI bottomText = bottomButtons[i].GetComponentInChildren<Image>().GetComponentInChildren<TextMeshProUGUI>();
            if (bottomText != null)
            {
                int betValue = string.IsNullOrEmpty(bottomText.text) ? 0 : int.Parse(bottomText.text);
                SumOfBets -= betValue; // Subtract each bet value from sumOfBets
                if (restoreTotal)
                {
                    totalScore += betValue;
                }// Add each bet value back to totalScore
                bottomText.text = string.Empty;
                bottomButtons[i].GetComponent<ChangeButtonImage>().ResetImage();

                bottomButtons[i].GetComponent<ChangeButtonImage>().PlaySelectedAnim(false);
                filledButtonCount--;
            }

        }

        lastBetIndex = -1;
        filledButtonCount = 0;
    }
    public void ClearBottomButton(int index)
    {
        if (index >= 0 && index < bottomButtons.Length)
        {
            TextMeshProUGUI bottomText = bottomButtons[index].GetComponentInChildren<Image>().GetComponentInChildren<TextMeshProUGUI>();
            if (bottomText != null)
            {
                int betValue = string.IsNullOrEmpty(bottomText.text) ? 0 : int.Parse(bottomText.text);
                SumOfBets -= betValue; // Subtract the bet value from sumOfBets
                totalScore += betValue; // Add the bet value back to totalScore
                bottomText.text = string.Empty;
                bottomButtons[index].GetComponent<ChangeButtonImage>().ResetImage();

                if (filledButtonCount > 0)
                {
                    filledButtonCount--;
                }
            }
            List<Bet> itemsToRemove = new List<Bet>();
            foreach (Bet bet in currentBetList)
            {
                if (bet.index == index) {
                    itemsToRemove.Add(bet);
                }
            }
            foreach (Bet bet in itemsToRemove)
            {
                currentBetList.Remove(bet);
            }

            UpdateCancelBetAnimator();
            UpdateSumOfBetsUI();
            UpdateScoreDisplay();
        }
    }
    // Update is called once per frame
    void Update()
    {
        //if(!betPlaced && currentBetList.Count > 0)
        //{
        //    betOkButton.interactable = true;
        //}
        //else
        //{
        //    betOkButton.interactable = false;
        //}
        RebetButtonState();
        OnKeyInput();

        GameController.Instance.ShowInputBlock(CanShowInputBlock());

        if (!betPlaced && TimerController.Instance.CurrentTime < 10 && TimerController.Instance.CurrentTime > 0)
        {
            GameController.Instance.UpdateBottomText("Bet Time Over");
            PlaceBet();
        }



    }

    private bool CanShowInputBlock()
    {
        return betPlaced || TimerController.Instance.CurrentTime < 10;
    }

    private void RebetButtonState()
    {
        if (betPlaced == false && prevBetList.Count > 0 && currentBetList.Count == 0)
        {
            prevBetButton.gameObject.SetActive(true);
            prevBetButton.GetComponent<Animator>().SetBool("Show", true);
        }
        else
        {
            if (prevBetButton.gameObject.activeSelf)
            {
                prevBetButton.GetComponent<Animator>().SetBool("Show", false);
                prevBetButton.gameObject.SetActive(false);
            }
          
        }
    }

    private void OnKeyInput()
    {
        if (betPlaced) return;
        int keyInput = -1;
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            keyInput = 9;
        }else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            keyInput = 0;
        }else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            keyInput = 1;
        }else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            keyInput = 2;
        }else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            keyInput = 3;
        }else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            keyInput = 4;
        }else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            keyInput = 5;
        }else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            keyInput = 6;
        }else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            keyInput = 7;
        }else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            keyInput = 8;
        }
        if(keyInput != -1)
        {
            OnBottomButtonClick(keyInput, GetSelectedChipValue());
        }
    }

    [Serializable]
    public struct Bet
    {
        public int index;
        public int chipValue;
    }
}
