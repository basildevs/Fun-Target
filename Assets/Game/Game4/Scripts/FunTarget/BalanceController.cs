using System.Collections;
using System.Collections.Generic;
using FunTarget;
using TMPro;
using UnityEngine;

public class BalanceController : Singleton<BalanceController>
{
    public TMP_Text balanceText;
    [SerializeField]
    private float currentBalance;
    private float startingBalance;

    public float StartingBalance { get => startingBalance; set => startingBalance = value; }
    public float CurrentBalance { get => currentBalance; set => currentBalance = value; }

    private void Start()
    {
        StartingBalance = currentBalance;
        UpdateUI();
    }
    public float GetBalance()
    {
        return currentBalance;
    }
    public void SetBalance(float balance)
    {
        //Debug.Log("Set balance: " + balance);
        currentBalance = balance;
        UpdateUI();
    }

    public void AddToBalance(float balance)
    {
        currentBalance += balance;
        UpdateUI();
    }

    public void ReduceBalance(float balance)
    {
        if ((currentBalance - balance) < 0)
        {
            Debug.Log("Not enough balance");
            return;
        }
        currentBalance -= balance;
        UpdateUI();
    }
    public bool HasEnoughBalance(float reduceAmount)
    {
        if ((currentBalance - reduceAmount) < 0)
        {
            NotEnoughBalance();
            return false;
        }
        else
        {
            return true;
        }
    }
    public void NotEnoughBalance()
    {
        GameController.Instance.bottomText.text = "Not enough balance";

    }
    private void UpdateUI()
    {
        balanceText.text = currentBalance.ToString();
    }
}
