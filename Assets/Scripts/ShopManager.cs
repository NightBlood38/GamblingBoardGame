using UnityEngine;
using System.Collections;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public GameObject shopUI;
    private GameManager gameManager;
    public TextMeshProUGUI moneyText;

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
        UpdateMoneyText();
        shopUI.SetActive(true);
        GameManager.isShopOpen = true;
    }

    public void CloseShop()
    {
        shopUI.SetActive(false);
        GameManager.isShopOpen = false;
    }

    public void BuyItem(int cost, string itemName)
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
            UpdateMoneyText();
            Debug.Log($"Successfully bought {itemName} for {cost}!");
        }
        else
        {
            Debug.Log($"Not enough money! Player has {currentPlayer.money}, but needs {cost}.");
        }
    }


    public void BuyDiadem() { BuyItem(5000, "Diadem"); }
    public void BuyRing() { BuyItem(12000, "Ring"); }
    public void BuyDress() { BuyItem(20000, "Dress"); }
    public void BuyShoes() { BuyItem(30000, "Shoes"); }
    public void BuyMonocle() { BuyItem(50000, "Monocle"); }

    public void UpdateMoneyText()
    {
        if (GameManager.Instance != null)
        {
            PlayerData currentPlayer = GameManager.Instance.GetCurrentPlayer();
            if (currentPlayer != null)
            {
                moneyText.text = $"${currentPlayer.money}"; // Kiírja a játékos pénzét
            }
        }
    }
}
