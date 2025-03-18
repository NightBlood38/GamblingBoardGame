using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlackjackManager : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public Button hitButton, standButton, closeButton;
    public GameObject cardPrefab;
    public Transform playerCardHolder, dealerCardHolder;
    public GameObject blackjackUI;
    public GameObject notEnoughMoneyUI;
    public GameManager gameManager;
    public TextMeshProUGUI playerCardSumText, dealerCardSumText, resultTextLeft, resultTextRight;
    public GameObject playerUI;
    public Sprite[] cardSprites;

    private List<int> playerHand = new List<int>();
    private List<int> dealerHand = new List<int>();
    private bool gameOver = false;
    private int playerCardSum;
    private int dealerCardSum;
    private int bet;

    void Awake()
    {
        // Betölti az összes kártya sprite-ot a mappából
        cardSprites = Resources.LoadAll<Sprite>("Standard 52 Cards/Standard Rounded Cards/Cards");
        
        if (cardSprites.Length == 0)
        {
            Debug.LogError("Nem találhatóak a kártyák! Ellenőrizd az elérési utat.");
        }
    }

    public Sprite GetCardSprite(int index)
    {
        if (index < 0 || index >= cardSprites.Length)
        {
            Debug.LogError("Érvénytelen kártya index: " + index);
            return null;
        }
        return cardSprites[index];
    }

    void Start()
    {
        blackjackUI.SetActive(false);
    }

    public void NotEnoughMoney()
    {
        notEnoughMoneyUI.SetActive(true);
        playerUI.SetActive(false);
        GameManager.isUIOpen = true;
    }
    public void CloseNotEnoughMoneyUI()
    {
        notEnoughMoneyUI.SetActive(false);
        playerUI.SetActive(true);
        GameManager.isUIOpen = false;
    }

    public void StartNewGame(int betAmount)
    {
        playerCardSum = 0;
        dealerCardSum = 0;
        bet = betAmount;
        gameManager.removeMoneyFromCurrentPlayer(bet);
        blackjackUI.SetActive(true);
        playerUI.SetActive(false);
        GameManager.isUIOpen = true;
        playerHand.Clear();
        dealerHand.Clear();
        gameOver = false;
        closeButton.gameObject.SetActive(false);
        resultTextLeft.gameObject.SetActive(false);
        resultTextRight.gameObject.SetActive(false);

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
        int randomCard = Random.Range(0, 52); 

        playerHand.Add(randomCard);
        playerCardSum = CalculateHandValue(playerHand);
        playerCardSumText.text = $"{playerCardSum}";

        GameObject newCard = Instantiate(cardPrefab, playerCardHolder);
        Sprite cardSprite = GetCardSprite(randomCard);
        newCard.GetComponent<Image>().sprite = cardSprite;
    }


    void DrawCardForDealer()
    {
        int randomCard = Random.Range(0, 52);

        dealerHand.Add(randomCard);
        dealerCardSum = CalculateHandValue(dealerHand);
        dealerCardSumText.text = $"{dealerCardSum}";

        GameObject newCard = Instantiate(cardPrefab, dealerCardHolder);
        Sprite cardSprite = GetCardSprite(randomCard);
        newCard.GetComponent<Image>().sprite = cardSprite;
    }


    int CalculateHandValue(List<int> hand)
    {
        int sum = 0;
        int aceCount = 0;
        int cardRank = 0;
        int cardValue = 0;

        foreach (int card in hand)
        {
            cardRank = (card % 13) + 2;
            if(cardRank > 10 && cardRank != 14){
                cardValue = 10;
            }
            else if(cardRank == 14)
            {
                cardValue = 1;
                aceCount++;
            }
            else
            {
                cardValue = cardRank;
            }
            sum += cardValue;
            if (cardRank == 14) aceCount++;
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
        PlayerData currentPlayer = gameManager.GetCurrentPlayer();
        gameOver = true;
        hitButton.interactable = false;
        standButton.interactable = false;
        closeButton.gameObject.SetActive(true);
        int goldenTicketLuckyNumber = Random.Range(0, 100);

        if(playerWon)
        {
            resultTextLeft.color = Color.green;
            resultTextLeft.text = "WON";
            resultTextRight.color = Color.green;
            resultTextRight.text = "WON";
            gameManager.addMoneyToCurrentPlayer(2*bet);
        }
        else
        {
            resultTextLeft.color = Color.red;
            resultTextLeft.text = "LOST";
            resultTextRight.color = Color.red;
            resultTextRight.text = "LOST";
        }
        resultTextLeft.gameObject.SetActive(true);
        resultTextRight.gameObject.SetActive(true);

        if(goldenTicketLuckyNumber < 4)
        {
            currentPlayer.goldenTicketAmount += 1;
        }

        UpdateMoneyUI();
    }

    public void CloseBlackjackUI()
    {
        GameManager.isUIOpen = false;
        blackjackUI.SetActive(false);
        playerUI.SetActive(true);
        foreach (Transform child in playerCardHolder)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in dealerCardHolder)
        {
            Destroy(child.gameObject);
        }
    }

    void UpdateMoneyUI()
    {
        PlayerData currentPlayer = gameManager.GetCurrentPlayer();
        moneyText.text = $"${currentPlayer.money}";
    }
}
