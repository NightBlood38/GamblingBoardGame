using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LuckyCardsManager : MonoBehaviour
{
    private string[] luckyCardEffects = {
    "You won the lottery! Your reward is $500",
    "Your gambling addiction paid off. Collect $2000",
    "A friend sends you a generous gift. You get $1000",
    "You hit the jackpot. $1000 has been sent to your account.",
    "You've placed a risky bet, but it was worth it. You won $2000",
    "You accidentally took part in a heist. Your payout was $4000",
    "You went to the casino, but it backfired. You lost $2000",
    "Your luck has run out. You will now lose $1000",
    "Your gambling dept catches up with you! Pay $500 to the bank",
    "The government collaped, money lost its value, set your money to $0",
    "The fate is a curious thing. Set your money to $10000",
    "You've got THE GOLDEN TICKET. You're guaranteed to win BIG",
    "Your good fortune is rewarded. Enjoy a little refund from the taxman"
    };
    private byte currentLuckyCardIndex;

    public TextMeshProUGUI luckyCardText;
    public GameManager gameManager;
    public GameObject luckyCardsUI;

    void Start()
    {
        luckyCardsUI.SetActive(false);
    }

    public void DrawLuckyCard()
    {
        currentLuckyCardIndex = Convert.ToByte(UnityEngine.Random.Range(0, luckyCardEffects.Length));
        UpdateLuckyCardText();
        luckyCardsUI.SetActive(true);
        GameManager.isUIOpen = true;
    }

    public void CloseLuckyCardsUI()
    {
        luckyCardsUI.SetActive(false);
        GameManager.isUIOpen = false;
        GameManager.Instance.EndTurn();
    }

    public void TriggerLuckyCardEffect()
    {
        PlayerData currentPlayer = gameManager.GetCurrentPlayer();
        switch(currentLuckyCardIndex)
        {
            case 0:
                currentPlayer.money +=500;
                break;
            case 1:
                currentPlayer.money += 2000;
                break;
            case 2:
                currentPlayer.money += 1000;
                break;
            case 3:
                currentPlayer.money += 1000;
                break;
            case 4:
                currentPlayer.money += 2000;
                break;
            case 5:
                currentPlayer.money += 4000;
                break;
            case 6:
                currentPlayer.money -= 2000;
                break;
            case 7:
                currentPlayer.money -= 1000;
                break;
            case 8:
                currentPlayer.money -= 500;
                break;
            case 9:
                currentPlayer.money = 0;
                break;
            case 10:
                currentPlayer.money = 10000;
                break;
            case 11:
                currentPlayer.goldenTicketAmount += 1;
                break;
            case 12:
                currentPlayer.money = currentPlayer.money + Convert.ToInt32(currentPlayer.previousBetSum*0.1);
                break;
        }
        Debug.Log($"The current player has ${currentPlayer.money}");
    }

    private void UpdateLuckyCardText()
    {
        PlayerData currentPlayer = gameManager.GetCurrentPlayer();
        luckyCardText.text = luckyCardEffects[currentLuckyCardIndex];
    }
}
