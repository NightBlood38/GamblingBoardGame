using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool cannotThrowDice = false;
    public static bool isUIOpen = false;
    public static bool canStartMoving = false;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI goldenTicketText;
    public Button rollDiceButton, useGoldenTicketButton;
    public GameObject rollDiceUI;
    public TextMeshProUGUI rollDiceNumber;

    private PlayerMovement[] players;
    private int currentPlayerIndex = 0;
    private PlayerData[] playerData;
    private bool isGameOver = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        players = FindAllPlayers();

        if (players == null || players.Length == 0)
        {
            Debug.LogError("NINCS EGYETLEN PLAYER SEM!");
            return;
        }

        playerData = new PlayerData[players.Length];
        Debug.Log($"Játékosok száma: {players.Length}");

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null)
            {
                Debug.LogError($"A(z) {i}. játékos NINCS inicializálva!");
                continue;
            }

            playerData[i] = players[i].GetComponent<PlayerData>();

            if (playerData[i] == null)
            {
                Debug.LogError($"A(z) {i}. játékosnak nincs PlayerData komponense!");
            }
        }
        players[0].StartTurn();
        useGoldenTicketButton.interactable = false;
        rollDiceUI.SetActive(false);
    }

    void Update()
    {
        UpdatePlayerUI();
        
    }

    IEnumerator changeNumbersFast(int rollAmount)
{
        for (float i = 0; i < Random.Range(0f, 1f); i += 0.01f)
        {
            rollDiceNumber.text = $"{Random.Range(1, 5)}";
            yield return new WaitForSeconds(i);
        }

        rollDiceNumber.text = $"{rollAmount}";
        canStartMoving = true;
    }


    public void RollDiceForCurrentPlayer()
    {
        rollDiceUI.SetActive(true);
        int diceRoll = Random.Range(1, 5);

        // Elindítjuk a Coroutine-t, és várunk, amíg befejeződik
        StartCoroutine(HandleDiceRoll(diceRoll));
    }

    IEnumerator HandleDiceRoll(int rollAmount)
    {
        // A számokat gyorsan dobáljuk
        yield return StartCoroutine(changeNumbersFast(rollAmount));

        // Miután a Coroutine befejeződött, engedélyezzük a mozgást
        if (players[currentPlayerIndex] != null)
        {
            players[currentPlayerIndex].MovePlayerByDiceRoll(rollAmount);
        }
        else
        {
            Debug.LogError("HIBA: currentPlayerIndex nem létező játékosra mutat!");
        }

        // Visszaállítjuk a gombot
        rollDiceButton.interactable = false;

        // Ha van aranyjegy, engedélyezzük a gombot
        if (CurrentPlayerDoesHaveGoldenTicket())
        {
            useGoldenTicketButton.interactable = true;
        }
    }



    public void EndTurn()
    {
        rollDiceButton.interactable = true;
        players[currentPlayerIndex].EndTurn();
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        players[currentPlayerIndex].StartTurn();
    }

    public PlayerData GetCurrentPlayer()
    {
        Debug.Log($"Current player index: {currentPlayerIndex}");
        Debug.Log($"Current player money: {playerData[currentPlayerIndex].money}");
        return playerData[currentPlayerIndex];
    }

    public void Win(){
        for(int i = 0; i < 5; i++){
            if(playerData[currentPlayerIndex].haveItems[i])
            {
                isGameOver = true;
            }
            else
            {
                isGameOver = false;
            }
        }
        Debug.Log(isGameOver);
    }
    public void UpdatePlayerUI()
    {
        moneyText.text = $"${playerData[currentPlayerIndex].money}";
        goldenTicketText.text = $"Golden tickets: {playerData[currentPlayerIndex].goldenTicketAmount}";
        if(!CurrentPlayerDoesHaveGoldenTicket())
        {
            useGoldenTicketButton.interactable = false;
        }
    }
    
    public void AddMoneyToCurrentPlayer(int amount)
    {
        playerData[currentPlayerIndex].money += amount;
    }
    public void AddGoldenTicketToCurrentPlayer()
    {
        playerData[currentPlayerIndex].goldenTicketAmount++;
    }
    public void SetCurrentPlayerMoney(int amount)
    {
        playerData[currentPlayerIndex].money = amount;
    }
    public void RemoveGoldenTicketFromCurrentPlayer()
    {
        playerData[currentPlayerIndex].goldenTicketAmount--;
    }
    public int GetCurrentPlayerMoney()
    {
        return playerData[currentPlayerIndex].money;
    }
    public bool CurrentPlayerDoesHaveGoldenTicket()
    {
        if(playerData[currentPlayerIndex].goldenTicketAmount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void RemoveMoneyFromCurrentPlayer(int amount)
    {
        playerData[currentPlayerIndex].money -= amount;
    }

    public void TriggerTileEffectWithGoldenTicket()
    {
        if(playerData[currentPlayerIndex].goldenTicketAmount > 0)
        {
            RemoveGoldenTicketFromCurrentPlayer();
            players[currentPlayerIndex].HandleTileEffects();
        }
    }

    //Megkeresi az összes játékost a jelenetben
    private PlayerMovement[] FindAllPlayers()
    {
        return FindObjectsOfType<PlayerMovement>();
    }
}
