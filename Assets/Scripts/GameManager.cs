using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private PlayerMovement[] players;
    private int currentPlayerIndex = 0;
    private PlayerData[] playerData;
    public static bool cannotThrowDice = false;
    public static bool isShopOpen = false;

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
        
        if (Input.GetKeyDown(KeyCode.H) && !cannotThrowDice && !isShopOpen)
        {
            RollDiceForCurrentPlayer();
        }
        else if(Input.GetKeyDown(KeyCode.H) && cannotThrowDice && isShopOpen)
        {
            Debug.Log("nem tudsz dobni a kockával");
        }
        
    }

    void RollDiceForCurrentPlayer()
    {
        int diceRoll = Random.Range(1, 5);
        Debug.Log($"Player {currentPlayerIndex + 1} dobott: {diceRoll}");

        if (players[currentPlayerIndex] != null)
        {
            Debug.Log("A MovePlayerByDiceRoll meghívása...");
            players[currentPlayerIndex].MovePlayerByDiceRoll(diceRoll);
        }
        else
        {
            Debug.LogError("HIBA: currentPlayerIndex nem létező játékosra mutat!");
        }
    }


    public void EndTurn()
    {
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

    //Megkeresi az összes játékost a jelenetben
    private PlayerMovement[] FindAllPlayers()
    {
        return FindObjectsOfType<PlayerMovement>();
    }
}
