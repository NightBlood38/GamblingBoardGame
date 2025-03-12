using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlackjackManager : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public Button hitButton, standButton;
    public GameObject cardPrefab;
    public Transform playerCardHolder, dealerCardHolder;
    public GameObject blackjackUI;
    public GameObject notEnoughMoneyUI;
    public GameManager gameManager;

    private List<int> playerHand = new List<int>();
    private List<int> dealerHand = new List<int>();
    private bool gameOver = false;

    void Start()
    {
        blackjackUI.SetActive(false);
    }

    public void NotEnoughMoney()
    {
        notEnoughMoneyUI.SetActive(true);
        GameManager.isUIOpen = true;
    }
    public void CloseNotEnoughMoneyUI()
    {
        notEnoughMoneyUI.SetActive(false);
        GameManager.isUIOpen = false;
        GameManager.Instance.EndTurn();
    }

    public void StartNewGame()
    {
        blackjackUI.SetActive(true);
        GameManager.isUIOpen = true;
        playerHand.Clear();
        dealerHand.Clear();
        gameOver = false;

        hitButton.interactable = true;
        standButton.interactable = true;

        // Kezdő lapok húzása (véletlenszám generálással)
        DrawCardForPlayer();
        DrawCardForPlayer();
        DrawCardForDealer();

        UpdateMoneyUI();
    }

    void DrawCardForPlayer()
    {
        int cardValue = Random.Range(1, 11); // Véletlenszerű kártya (1-10 között)
        playerHand.Add(cardValue); // Hozzáadjuk a játékos kezéhez

        GameObject newCard = Instantiate(cardPrefab, playerCardHolder);
        newCard.GetComponentInChildren<TextMeshProUGUI>().text = cardValue.ToString();
    }

    void DrawCardForDealer()
    {
        int cardValue = Random.Range(1, 11); // Véletlenszerű kártya (1-10 között)
        dealerHand.Add(cardValue); // Hozzáadjuk az osztó kezéhez

        GameObject newCard = Instantiate(cardPrefab, dealerCardHolder);
        newCard.GetComponentInChildren<TextMeshProUGUI>().text = cardValue.ToString();
    }

    int CalculateHandValue(List<int> hand)
    {
        int sum = 0;
        int aceCount = 0;
        foreach (int card in hand)
        {
            sum += card;
            if (card == 1) aceCount++;
        }

        while (aceCount > 0 && sum + 10 <= 21)
        {
            sum += 10;
            aceCount--;
        }
        return sum;
    }

    public void OnHit()
    {
        if (gameOver) return;

        DrawCardForPlayer();

        if (CalculateHandValue(playerHand) > 21)
        {
            EndGame(false);
        }
    }

    public void OnStand()
    {
        if (gameOver) return;

        // Dealer húz, amíg el nem éri a 17-et
        while (CalculateHandValue(dealerHand) < 17)
        {
            DrawCardForDealer();
        }

        int playerScore = CalculateHandValue(playerHand);
        int dealerScore = CalculateHandValue(dealerHand);

        PlayerData currentPlayer = gameManager.GetCurrentPlayer();
        if (dealerScore > 21 || playerScore > dealerScore)
        {
            EndGame(true);
            currentPlayer.money += PlayerMovement.blackjackBetAmount;
        }
        else
        {
            EndGame(false);
            currentPlayer.money -= PlayerMovement.blackjackBetAmount;
        }
    }

    void EndGame(bool playerWon)
    {
        gameOver = true;
        hitButton.interactable = false;
        standButton.interactable = false;
        GameManager.isUIOpen = false;

        UpdateMoneyUI();
        blackjackUI.SetActive(false);
        GameManager.Instance.EndTurn();
    }

    void UpdateMoneyUI()
    {
        PlayerData currentPlayer = gameManager.GetCurrentPlayer();
        moneyText.text = $"${currentPlayer.money}";
    }
}
