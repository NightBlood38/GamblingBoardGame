using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject shopUI;
    private GameManager gameManager;
    public TextMeshProUGUI moneyText;
    public Button diademButton;
    public Button ringButton;
    public Button shoeButton;
    public Button dressButton;
    public Button monocleButton;
    

    private void Start()
    {
        shopUI.SetActive(false);

        // Késleltetett keresés
        StartCoroutine(FindGameManager());
        
    }

    private IEnumerator FindGameManager()
    {
        // Várunk, amíg a GameManager inicializálódik
        while (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            yield return null; // Egy frame-et várunk
        }
    }

    public void OpenShop()
    {
        UpdateShopUI();
        shopUI.SetActive(true);
        GameManager.cannotThrowDice = true;
        GameManager.isUIOpen = true;
    }

    public void CloseShop()
    {
        shopUI.SetActive(false);
        GameManager.cannotThrowDice = false;
        GameManager.isUIOpen = false;
        GameManager.Instance.EndTurn();
    }

    public void BuyItem(int cost, string itemName, int index)
    {
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }

        PlayerData currentPlayer = gameManager.GetCurrentPlayer();
        
        if (currentPlayer == null)
        {
            Debug.LogError("Current player is null!");
            return;
        }

        Debug.Log($"Current money before purchase: {currentPlayer.money}");

        if (currentPlayer.SpendMoney(cost))
        {
            currentPlayer.haveItems[index] = true;
            UpdateShopUI();
            Debug.Log($"Successfully bought {itemName} for {cost}!");
            gameManager.Win();
        }
        else
        {
            Debug.Log($"Not enough money! Player has {currentPlayer.money}, but needs {cost}.");
        }
    }

    public void BuyDiadem() { BuyItem(5000, "Diadem",0); }
    public void BuyRing() { BuyItem(12000, "Ring",1); }
    public void BuyDress() { BuyItem(20000, "Dress",2); }
    public void BuyShoes() { BuyItem(30000, "Shoes",3); }
    public void BuyMonocle() { BuyItem(50000, "Monocle",4); }

    public void UpdateShopUI()
    {
        if (GameManager.Instance != null)
        {
            PlayerData currentPlayer = gameManager.GetCurrentPlayer();
            if (currentPlayer != null)
            {
                moneyText.text = $"${currentPlayer.money}"; // Kiírja a játékos pénzét
            }

            if(currentPlayer.haveItems[0])
            {
                diademButton.interactable = false;
            }
            else
            {
                diademButton.interactable = true;
            }
            if(currentPlayer.haveItems[1])
            {
                ringButton.interactable = false;
            }
            else
            {
                ringButton.interactable = true;
            }
            if(currentPlayer.haveItems[2])
            {
                dressButton.interactable = false;
            }
            else
            {
                dressButton.interactable = true;
            }
            if(currentPlayer.haveItems[3])
            {
                shoeButton.interactable = false;
            }
            else
            {
                shoeButton.interactable = true;
            }
            if(currentPlayer.haveItems[4])
            {
                monocleButton.interactable = false;
            }
            else
            {
                monocleButton.interactable = true;
            }
            Image diademButtonImage = diademButton.GetComponent<Image>();
            Image ringButtonImage = ringButton.GetComponent<Image>();
            Image dressButtonImage = dressButton.GetComponent<Image>();
            Image shoeButtonImage = shoeButton.GetComponent<Image>();
            Image monocleButtonImage = monocleButton.GetComponent<Image>();
            
            if(currentPlayer.money < 5000 && !currentPlayer.haveItems[0])
            {
                diademButtonImage.color = Color.red;
            }
            else
            {
                diademButtonImage.color = Color.green;
            }
            if(currentPlayer.money < 12000 && !currentPlayer.haveItems[1])
            {
                ringButtonImage.color = Color.red;
            }
            else
            {
                ringButtonImage.color = Color.green;
            }
            if(currentPlayer.money < 20000 && !currentPlayer.haveItems[2])
            {
                dressButtonImage.color = Color.red;
            }
            else
            {
                dressButtonImage.color = Color.green;
            }
            if(currentPlayer.money < 30000 && !currentPlayer.haveItems[3])
            {
                shoeButtonImage.color = Color.red;
            }
            else
            {
                shoeButtonImage.color = Color.green;
            }
            if(currentPlayer.money < 50000 && !currentPlayer.haveItems[4])
            {
                monocleButtonImage.color = Color.red;
            }
            else
            {
                monocleButtonImage.color = Color.green;
            }
        }
    }
}
