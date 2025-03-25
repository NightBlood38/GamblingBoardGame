using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
    
    private Sprite cardBack;
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
        cardBack = Resources.Load<Sprite>("Standard 52 Cards/Standard Rounded Cards/Card Back/cardBackBlue");
        
        if (cardSprites.Length == 0)
        {
            Debug.LogError("Nem találhatóak a kártyák! Ellenőrizd az elérési utat.");
        }
    }

    //getting card images
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

    //close NEM UI
    public void CloseNotEnoughMoneyUI()
    {
        notEnoughMoneyUI.SetActive(false);
        playerUI.SetActive(true);
        GameManager.isUIOpen = false;
    }

    //start blackjack game
    public IEnumerator StartNewGame(int betAmount)
    {
        hitButton.interactable = false;
        standButton.interactable = false;
        playerCardSum = 0;
        dealerCardSum = 0;
        bet = betAmount;
        gameManager.RemoveMoneyFromCurrentPlayer(bet);
        blackjackUI.SetActive(true);
        playerUI.SetActive(false);
        GameManager.isUIOpen = true;
        playerHand.Clear();
        dealerHand.Clear();
        gameOver = false;
        closeButton.gameObject.SetActive(false);
        resultTextLeft.gameObject.SetActive(false);
        resultTextRight.gameObject.SetActive(false);
        UpdateMoneyUI();
        playerCardSumText.text = "0";
        dealerCardSumText.text = "0";

        // Kezdő lapok húzása (véletlenszám generálással)
        yield return new WaitForSeconds(1);
        DrawCardForPlayer();
        yield return new WaitForSeconds(1);
        DrawCardForDealer();
        yield return new WaitForSeconds(1);
        DrawCardForPlayer();
        yield return new WaitForSeconds(1);
        DrawCardFaceDownForDealer();
        hitButton.interactable = true;
        standButton.interactable = true;
    }

    //handling button presses in bj UI
    public void OnHitButtonPressed()
    {
        if (gameOver) return;

        DrawCardForPlayer();

        if (CalculateHandValue(playerHand) > 21)
        {
            EndGame("lost");
        }
    }

    public void OnStandButtonPressed()
    {
        Destroy(dealerCardHolder.transform.GetChild(1).gameObject);
        StartCoroutine(OnStand());
    }

    //IEnumerator so I can use StartCoroutine()
    IEnumerator OnStand()
    {
        standButton.interactable = false;
        hitButton.interactable = false;

        if (gameOver) yield break;

        DrawCardForDealer();

        while (CalculateHandValue(dealerHand) < 17)
        {
            yield return new WaitForSeconds(1);
            DrawCardForDealer();
        }

        int playerScore = CalculateHandValue(playerHand);
        int dealerScore = CalculateHandValue(dealerHand);

        if (dealerScore > 21 || playerScore > dealerScore)
        {
            EndGame("won");
        }
        else if (dealerScore > playerScore || playerScore > 21)
        {
            EndGame("lost");
        }
        else
        {
            EndGame("push");
        }
    }

    //closing bj UI
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

    //updating money on the bj UI
    void UpdateMoneyUI()
    {
        PlayerData currentPlayer = gameManager.GetCurrentPlayer();
        moneyText.text = $"${currentPlayer.money}";
    }

    //draw card for player
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

    //draw card for dealer
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

    //draw a face down card for dealer
    void DrawCardFaceDownForDealer()
    {
        GameObject newCard = Instantiate(cardPrefab, dealerCardHolder);
        newCard.GetComponent<Image>().sprite = cardBack;
    }

    //ending current bj game
    void EndGame(string playerWon)
    {
        gameOver = true;
        closeButton.gameObject.SetActive(true);
        int goldenTicketLuckyNumber = Random.Range(0, 100);

        if(playerWon == "won")
        {
            resultTextLeft.color = Color.green;
            resultTextLeft.text = "WON";
            resultTextRight.color = Color.green;
            resultTextRight.text = "WON";
            gameManager.AddMoneyToCurrentPlayer(2*bet);
        }
        else if (playerWon == "lost")
        {
            resultTextLeft.color = Color.red;
            resultTextLeft.text = "LOST";
            resultTextRight.color = Color.red;
            resultTextRight.text = "LOST";
        }
        else if ( playerWon == "push")
        {
            resultTextLeft.color = Color.white;
            resultTextLeft.text = "PUSH";
            resultTextRight.color = Color.white;
            resultTextRight.text = "PUSH";
            gameManager.AddMoneyToCurrentPlayer(bet);
        }
        resultTextLeft.gameObject.SetActive(true);
        resultTextRight.gameObject.SetActive(true);

        if(goldenTicketLuckyNumber < 4)
        {
            gameManager.AddGoldenTicketToCurrentPlayer();
        }

        UpdateMoneyUI();
    }

    //calculate the value of cards held in hand
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

        if(hand.Count != 1 && aceCount > 0)
        {
            while (sum + 10 <= 21)
            {
                sum += 10;
                aceCount--;
            }
        }
        else if (hand.Count == 1 && aceCount > 0)
        {
            sum = 11;
        }
        return sum;
    }
}
