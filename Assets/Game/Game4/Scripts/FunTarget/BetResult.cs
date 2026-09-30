using FunTarget;
using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Analytics.IAnalytic;
using TMPro;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

public class BetResult : MonoBehaviour
{
    [Header("ANIMATOR")]
    [SerializeField] Animator[] cancelBetAnimators;
    private bool init;
    public BetPlacement betPlacement;
    public Animator takeAnimator;
    public Button takeButton;
    private Image takeButtonImage;
    public TMP_Text winAmountText;
    public WebSocketEvent webSocketEvent;
    public SpriteRenderer initialWheelSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        takeButtonImage = takeButton.GetComponent<Image>();
        takeButtonImage.raycastTarget = false;
        takeButton.onClick.AddListener(TakeBet);
        init = true;
        gameObject.SetActive(false);
    }

    private void TakeBet()
    {
        AudioController.Instance.PlayAudio(true, "takeAudio");
        betPlacement.TotalScore = betPlacement.LatestWinData.Credit;
        BalanceController.Instance.SetBalance(betPlacement.TotalScore);
        winAmountText.text = "0";
        takeAnimator.SetBool("Show", false);

        takeButtonImage.raycastTarget = false;
        GameController.Instance.ShowInputBlock(false);

        Claim claim = new Claim();
        claim.type = "claim";
        WebSocketEditor.Instance.SendMessageToServer(JsonUtility.ToJson(claim));
    }
    private void OnEnable()
    {
        if (!init) return;

        TimerController.Instance.OnTimerFinished += OnTimerFinished;

        foreach (Animator animator in cancelBetAnimators)
        {
            animator.SetBool("Show", false);
        }
        GameController.Instance.UpdateBottomText("");
        Wheel.Instance.OnWheelRotationFinished += OnWheelRotationFinished;
        webSocketEvent.OnAmountClaimed += OnWinClaimed;
    }

    private void OnTimerFinished()
    {
        TimerController.Instance.timerRunning = false;
    }

    private void OnWinClaimed(ClaimResult result)
    {
        //  SetTimer();
        TimerController.Instance.CurrentTime = result.RemainingTime;
        TimerController.Instance.timerRunning = true;
        Wheel.Instance.ShowIndex(result.WinNo.WinNo);
        betPlacement.TotalScore = result.Credit;
        BalanceController.Instance.SetBalance(result.Credit);
        initialWheelSprite.enabled = true;
        GameController.Instance.DefaultBalance = result.Credit;
        betPlacement.gameObject.SetActive(true);
        WheelController.Instance.StartWheelPoint(result.WinNo.WinNo);
        TimerController.Instance.ResyncTimerAfterClaim(result.RemainingTime);
        List<WinHistory> winHistory = result.WinHistory;
        for (int i = 0; i < winHistory.Count; i++)
        {
            GameController.Instance.ConvertAndShowHistory(winHistory[i]);
        }
        TimerController.Instance.Reset();
        gameObject.SetActive(false);
    }

    //public async void SetTimer()
    //{
    //    TimeData timeData = null;
    //    WebSocketEvent.Instance.OnTimeDataReceieved += (_timeData) =>
    //    {
    //        timeData = _timeData;
    //    };
    //    while (timeData == null) {
    //        await Awaitable.NextFrameAsync();
    //    }
    //    TimerController.Instance.CurrentTime = timeData.RemainingSeconds;
    //    Debug.Log("Received time data");
    //    Debug.Log("Current time: " + timeData.RemainingSeconds);
    //}

    private void OnWheelRotationFinished()
    {
        List<WinHistory> winHistory = betPlacement.LatestWinData.WinHistory;
        GameController.Instance.DefaultBalance = betPlacement.LatestWinData.Credit;
        for (int i = 0; i < winHistory.Count; i++)
        {
            GameController.Instance.ConvertAndShowHistory(winHistory[i]);
        }
        if (betPlacement.WinGenerated && betPlacement.LatestWinData.Amounts.TotalWin > 0)
        {
            winAmountText.text = betPlacement.LatestWinData.Amounts.TotalWin.ToString();
            betPlacement.BottomButtons[betPlacement.LatestWinData.WinNo].GetComponent<ChangeButtonImage>().PlaySelectedAnim(true);
            ShowTakeMoney();
            return;
        }
        else
        {
            betPlacement.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    private bool CurrentWinDataNotNull()
    {
        bool IsNull = betPlacement.LatestWinData != null && betPlacement.LatestWinData.Amounts != null;
        if (IsNull) Debug.Log("Current win data null");
        return IsNull;
    }

    public void ShowTakeMoney()
    {
        GameController.Instance.UpdateBottomText("Congratulation!!! you Win");
        Debug.Log("Show take money");
        takeButtonImage.raycastTarget = true;
        //Take win amount;
        GameController.Instance.ShowInputBlock(true);
        takeAnimator.SetBool("Show", true);
    }

    private void OnDisable()
    {
        TimerController.Instance.OnTimerFinished -= OnTimerFinished;
        Wheel.Instance.OnWheelRotationFinished -= OnWheelRotationFinished;
        webSocketEvent.OnAmountClaimed -= OnWinClaimed;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

