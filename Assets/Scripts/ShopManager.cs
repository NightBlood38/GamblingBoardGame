using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject shopUI;
    public GameManager gameManager;
    public TextMeshProUGUI moneyText;
    public Button diademButton;
    public Button ringButton;
    public Button shoeButton;
    public Button dressButton;
    public Button monocleButton;
    public GameObject playerUI;
    

    private void Start()
    {
        shopUI.SetActive(false);        
    }

    //open shop UI
    public void OpenShop()
    {
        UpdateShopUI();
        shopUI.SetActive(true);
        playerUI.SetActive(false);
        GameManager.cannotThrowDice = true;
        GameManager.isUIOpen = true;
    }

    //close shop UI
    public void CloseShop()
    {
        shopUI.SetActive(false);
        playerUI.SetActive(true);
        GameManager.cannotThrowDice = false;
        GameManager.isUIOpen = false;
    }

    //handling purchases and adding them to player inventory
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

    //buy buttons
    public void BuyDiadem() { BuyItem(5000, "Diadem",0); }
    public void BuyRing() { BuyItem(12000, "Ring",1); }
    public void BuyDress() { BuyItem(20000, "Dress",2); }
    public void BuyShoes() { BuyItem(30000, "Shoes",3); }
    public void BuyMonocle() { BuyItem(50000, "Monocle",4); }

    //updating shop UI based on player inventory
    public void UpdateShopUI()
    {
        if (GameManager.Instance != null)
        {
            PlayerData currentPlayer = gameManager.GetCurrentPlayer();
            if (currentPlayer != null)
            {
                moneyText.text = $"${currentPlayer.money}";
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
