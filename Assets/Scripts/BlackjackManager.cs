using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Photon.Pun;

public class BlackjackManager : MonoBehaviourPun
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
    }

    //start blackjack game
    public IEnumerator StartNewGame(int betAmount)
    {
        playerCardSum = 0;
        dealerCardSum = 0;
        bet = betAmount;
        gameManager.RemoveMoneyFromCurrentPlayer(bet);
        blackjackUI.SetActive(true);
        playerUI.SetActive(false);
        playerHand.Clear();
        dealerHand.Clear();
        gameOver = false;
        closeButton.gameObject.SetActive(false);
        resultTextLeft.gameObject.SetActive(false);
        resultTextRight.gameObject.SetActive(false);
        playerCardSumText.text = "0";
        dealerCardSumText.text = "0";

        if(PhotonNetwork.InRoom)
        {
            if(PhotonNetwork.LocalPlayer.ActorNumber - 1 == gameManager.GetPlayerIndex())
            {
                hitButton.interactable = false;
                standButton.interactable = false;
                yield return new WaitForSeconds(1);
                DrawCardForPlayermp();
                yield return new WaitForSeconds(1);
                DrawCardForDealermp();
                yield return new WaitForSeconds(1);
                DrawCardForPlayermp();
                yield return new WaitForSeconds(1);
                this.photonView.RPC("DrawCardFaceDownForDealersp", RpcTarget.All);
                hitButton.interactable = true;
                standButton.interactable = true;
            }
            else
            {
                this.photonView.RPC("DisableButtonsForClient", RpcTarget.All);
            }

        }
        else
        {
            hitButton.interactable = false;
            standButton.interactable = false;
            yield return new WaitForSeconds(1);
            DrawCardForPlayersp();
            yield return new WaitForSeconds(1);
            DrawCardForDealersp();
            yield return new WaitForSeconds(1);
            DrawCardForPlayersp();
            yield return new WaitForSeconds(1);
            DrawCardFaceDownForDealersp();
            hitButton.interactable = true;
            standButton.interactable = true;
        }
    }
    //handling button presses in bj UI
    public void OnHitButtonPressed()
    {
        if(PhotonNetwork.InRoom)
        {
            this.photonView.RPC("OnHitButtonPressedmp", RpcTarget.All);
        }
        else
        {
            OnHitButtonPressedsp();
        }
    }
    private void OnHitButtonPressedsp()
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber - 1 == gameManager.GetPlayerIndex())
        {
            if (gameOver) return;

            DrawCardForPlayersp();

            if (CalculateHandValue(playerHand) > 21)
            {
                EndGame("lost");
            }
        }
    }

    [PunRPC]
    private void OnHitButtonPressedmp()
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber - 1 == gameManager.GetPlayerIndex())
        {
            if (gameOver) return;

            DrawCardForPlayermp();

            if (CalculateHandValue(playerHand) > 21)
            {
                this.photonView.RPC("EndGame",RpcTarget.All, "lost");
            }
        }
    }
    [PunRPC]
    public void DisableButtonsForClient()
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber - 1 == gameManager.GetPlayerIndex())
        {
            closeButton.interactable = true;
        }
        else
        {
            closeButton.interactable = false;
            hitButton.interactable = false;
            standButton.interactable = false;
        }
    }
    public void OnStandButtonPressed()
    {
        if(PhotonNetwork.InRoom)
        {
            StartCoroutine(OnStandmpRPC());
        }
        else
        {
            
            StartCoroutine(OnStandsp());
        }
    }
    [PunRPC]
    private void DestroyDealerCard()
    {
        Destroy(dealerCardHolder.transform.GetChild(1).gameObject);
    }
    private IEnumerator OnStandmpRPC()
    {
        standButton.interactable = false;
        hitButton.interactable = false;

        if (gameOver) yield break;

        this.photonView.RPC("DestroyDealerCard", RpcTarget.All);

        playerCardSum = CalculateHandValue(playerHand);

        if(dealerCardSum < 17)
        {
            while (dealerCardSum < 17)
            {
                this.photonView.RPC("DrawCardForDealermp", RpcTarget.All);
                dealerCardSum = CalculateHandValue(dealerHand);
                yield return new WaitForSeconds(1);
            }
        }

        if (dealerCardSum > 21 || playerCardSum > dealerCardSum)
        {
            Debug.Log("Won");
            this.photonView.RPC("EndGame", RpcTarget.All, "won");
            yield return null;
        }
        else if (dealerCardSum > playerCardSum || playerCardSum > 21)
        {
            Debug.Log("Lost");
            this.photonView.RPC("EndGame", RpcTarget.All, "lost");
            yield return null;
        }
        else
        {
            Debug.Log("Push");
            this.photonView.RPC("EndGame", RpcTarget.All, "push");
            yield return null;
        }
    }

    //IEnumerator so I can use WaitForSeconds()
    private IEnumerator OnStandsp()
    {
        standButton.interactable = false;
        hitButton.interactable = false;

        if (gameOver) yield break;
        Destroy(dealerCardHolder.transform.GetChild(1).gameObject);

        DrawCardForDealersp();

        while (CalculateHandValue(dealerHand) < 17)
        {
            yield return new WaitForSeconds(1);
            DrawCardForDealersp();
        }

        playerCardSum = CalculateHandValue(playerHand);
        dealerCardSum = CalculateHandValue(dealerHand);

        if (dealerCardSum > 21 || playerCardSum > dealerCardSum)
        {
            EndGame("won");
        }
        else if (dealerCardSum > playerCardSum || playerCardSum > 21)
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
        if(PhotonNetwork.InRoom)
        {
            this.photonView.RPC("CloseBlackjackUIAction", RpcTarget.All);
        }
        else
        {
            CloseBlackjackUIAction();
        }
    }
    [PunRPC]
    public void CloseBlackjackUIAction()
    {
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

    //draw card for player
    private void DrawCardForPlayermp()
    {
        
        int randomCard = Random.Range(0, 52); 
        this.photonView.RPC("DrawCardForPlayermpRPC", RpcTarget.All, randomCard);
    }
    [PunRPC]
    private void DrawCardForPlayermpRPC(int randomCard)
    {
        playerHand.Add(randomCard);
        playerCardSum = CalculateHandValue(playerHand);
        playerCardSumText.text = $"{playerCardSum}";

        GameObject newCard = Instantiate(cardPrefab, playerCardHolder);
        Sprite cardSprite = GetCardSprite(randomCard);
        newCard.GetComponent<Image>().sprite = cardSprite;

    }
    private void DrawCardForPlayersp()
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
    [PunRPC]
    private void DrawCardForDealermpRPC(int randomCard)
    {
        Debug.Log("Drawing card for dealer with RPC");
        dealerHand.Add(randomCard);
        dealerCardSum = CalculateHandValue(dealerHand);
        dealerCardSumText.text = $"{dealerCardSum}";

        GameObject newCard = Instantiate(cardPrefab, dealerCardHolder);
        Sprite cardSprite = GetCardSprite(randomCard);
        newCard.GetComponent<Image>().sprite = cardSprite;
    }
    [PunRPC]
    private void DrawCardForDealermp()
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber - 1 == gameManager.GetPlayerIndex())
        {
            Debug.Log("DrawCardForDealermp");
            int randomCard = Random.Range(0, 52);

            this.photonView.RPC("DrawCardForDealermpRPC", RpcTarget.All, randomCard);
        }
    }
    private void DrawCardForDealersp()
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
    [PunRPC]
    private void DrawCardFaceDownForDealersp()
    {
        GameObject newCard = Instantiate(cardPrefab, dealerCardHolder);
        newCard.GetComponent<Image>().sprite = cardBack;
    }

    //ending current bj game
    [PunRPC]
    private void EndGame(string playerWon)
    {
        gameOver = true;
        closeButton.gameObject.SetActive(true);
        hitButton.interactable = false;
        standButton.interactable = false;
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
    }

    //calculate the value of cards held in hand
    private int CalculateHandValue(List<int> hand)
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
