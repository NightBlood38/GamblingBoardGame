using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Photon.Pun;

public class LuckyCardsManager : MonoBehaviourPun
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
    public GameObject playerUI;
    public Button closeButton, drawButton;

    private void Start()
    {
        luckyCardsUI.SetActive(false);
    }

    //draw a card
    [PunRPC]
    private void DrawLuckyCardAction()
    {
        if(PhotonNetwork.InRoom)
        {
            if(PhotonNetwork.LocalPlayer.ActorNumber - 1 == gameManager.GetPlayerIndex())
            {
                currentLuckyCardIndex = Convert.ToByte(UnityEngine.Random.Range(0, luckyCardEffects.Length));
                Debug.Log(luckyCardEffects[currentLuckyCardIndex]);
                TriggerLuckyCardEffect(currentLuckyCardIndex);
                closeButton.interactable = true;
                drawButton.interactable = false;
                UpdateLuckyCardText(currentLuckyCardIndex);  

            }
            else
            {
                closeButton.interactable = false;
                drawButton.interactable = false;
            }
        }
        else
        {
            currentLuckyCardIndex = Convert.ToByte(UnityEngine.Random.Range(0, luckyCardEffects.Length));
            Debug.Log(luckyCardEffects[currentLuckyCardIndex]);
            TriggerLuckyCardEffect(currentLuckyCardIndex);
            closeButton.interactable = true;
            drawButton.interactable = false;
            UpdateLuckyCardText(currentLuckyCardIndex);  
        }
        
    }
    public void StartLuckyCards()
    {
        luckyCardText.text = "";
        if(PhotonNetwork.InRoom)
        {
            if(PhotonNetwork.LocalPlayer.ActorNumber - 1 == gameManager.GetPlayerIndex())
            {
                closeButton.interactable = false;
                drawButton.interactable = true;

            }
            else
            {
                drawButton.interactable = false;
                closeButton.interactable = false;
            }
        }
        else
        {
            drawButton.interactable = true;
            closeButton.interactable = false;
        }
        luckyCardsUI.gameObject.SetActive(true);
        playerUI.SetActive(false);
    }
    public void DrawLuckyCard()
    {
        if(PhotonNetwork.InRoom)
        {
            this.photonView.RPC("DrawLuckyCardAction", RpcTarget.All);
        }
        else
        {
            DrawLuckyCardAction();
        }
    }

    //close lucky card UI
    [PunRPC]
    private void CloseLuckyCardsUIAction()
    {
        luckyCardsUI.SetActive(false);
        playerUI.SetActive(true);
    }
    public void CloseLuckyCardsUI()
    {
        if(PhotonNetwork.InRoom)
        {
            this.photonView.RPC("CloseLuckyCardsUIAction", RpcTarget.All);
        }
        else
        {
            CloseLuckyCardsUIAction();
        }
    }

    //handling the effects of lucky cards
    public void TriggerLuckyCardEffect(byte playerLuckyCardsIndex)
    {
        Debug.Log(playerLuckyCardsIndex);
        switch(playerLuckyCardsIndex)
        {
            case 0:
                gameManager.AddMoneyToCurrentPlayer(500);
                break;
            case 1:
                gameManager.AddMoneyToCurrentPlayer(2000);
                break;
            case 2:
                gameManager.AddMoneyToCurrentPlayer(1000);
                break;
            case 3:
                gameManager.AddMoneyToCurrentPlayer(1000);
                break;
            case 4:
                gameManager.AddMoneyToCurrentPlayer(2000);
                break;
            case 5:
                gameManager.AddMoneyToCurrentPlayer(4000);
                break;
            case 6:
                gameManager.RemoveMoneyFromCurrentPlayer(2000);
                break;
            case 7:
                gameManager.RemoveMoneyFromCurrentPlayer(1000);
                break;
            case 8:
                gameManager.RemoveMoneyFromCurrentPlayer(500);
                break;
            case 9:
                gameManager.SetCurrentPlayerMoney(0);
                break;
            case 10:
                gameManager.SetCurrentPlayerMoney(10000);
                break;
            case 11:
                gameManager.AddGoldenTicketToCurrentPlayer();
                break;
            case 12:
                gameManager.AddMoneyToCurrentPlayer(Convert.ToInt32(gameManager.GetCurrentPlayerMoney()*0.1));
                break;
        }
    }

    //updating lucky cards UI based on which cards is drawn
   
    private void UpdateLuckyCardText(byte playerLuckyCardsIndex)
    {
        if(PhotonNetwork.InRoom)
        {
            this.photonView.RPC("UpdateLuckyCardTextAction", RpcTarget.All, playerLuckyCardsIndex);
        }
        else
        {
            UpdateLuckyCardTextAction(playerLuckyCardsIndex);
        }
        
    }
     [PunRPC]
    private void UpdateLuckyCardTextAction(byte playerLuckyCardsIndex)
    {
        luckyCardText.text = luckyCardEffects[playerLuckyCardsIndex];
    }
}
