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
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI goldenTicketText;
    public Button rollDiceButton;

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
    }

    void Update()
    {
        updatePlayerUI();
        
    }

    public void RollDiceForCurrentPlayer()
    {
        int diceRoll = Random.Range(1, 5);
        Debug.Log($"Player {currentPlayerIndex + 1} dobott: {diceRoll}");

        if (players[currentPlayerIndex] != null)
        {
            players[currentPlayerIndex].MovePlayerByDiceRoll(diceRoll);
        }
        else
        {
            Debug.LogError("HIBA: currentPlayerIndex nem létező játékosra mutat!");
        }
        rollDiceButton.interactable = false;
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
    public void updatePlayerUI()
    {
        moneyText.text = $"${playerData[currentPlayerIndex].money}";
        goldenTicketText.text = $"Golden tickets: {playerData[currentPlayerIndex].goldenTicketAmount}";
    }
    
    public void addMoneyToCurrentPlayer(int amount)
    {
        playerData[currentPlayerIndex].money += amount;
    }
    public void addGoldenTicketToCurrentPlayer()
    {
        playerData[currentPlayerIndex].goldenTicketAmount++;
    }
    public void removeGoldenTicketFromCurrentPlayer()
    {
        playerData[currentPlayerIndex].goldenTicketAmount++;
    }
    public void removeMoneyFromCurrentPlayer(int amount)
    {
        playerData[currentPlayerIndex].money -= amount;
    }

    //Megkeresi az összes játékost a jelenetben
    private PlayerMovement[] FindAllPlayers()
    {
        return FindObjectsOfType<PlayerMovement>();
    }
}
