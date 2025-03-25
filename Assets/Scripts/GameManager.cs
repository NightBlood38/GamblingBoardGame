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
    public Button rollDiceButton, useGoldenTicketButton, endTurnButton;
    public GameObject rollDiceUI;
    public GameObject playerUI;
    public GameObject notEnoughMoneyUI;
    public GameObject escMenuUI;
    public TextMeshProUGUI rollDiceNumber, resumeButtonText, exitButtonText;

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
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            escMenuUI.SetActive(true);
            playerUI.SetActive(false);
        }
    }

    //esc menu controls
    public void HoveringOverResumeButton()
    {
        resumeButtonText.color = new Color(0f,0.78f,0f,1f);
    }
    public void EndHoveringOverResumeButton()
    {
        resumeButtonText.color = new Color(0f,0.55f,0f,1f);
    }
    public void HoveringOverExitButton()
    {
        exitButtonText.color = new Color(1f,0f,0f,1f);
    }
    public void EndHoveringOverExitButton()
    {
        exitButtonText.color = new Color(0.7f,0f,0f,1f);
    }
    public void CloseEscMenuUI()
    {
        escMenuUI.SetActive(false);
        playerUI.SetActive(true);
    }

    public void NotEnoughMoney()
    {
        notEnoughMoneyUI.SetActive(true);
        playerUI.SetActive(false);
        GameManager.isUIOpen = true;
    }

    //rolling the dice
    IEnumerator changeNumbersFast(int rollAmount)
    {
        endTurnButton.interactable = false;
        rollDiceButton.interactable = false;
        useGoldenTicketButton.interactable = false;
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
        StartCoroutine(HandleDiceRoll(diceRoll));
    }

    IEnumerator HandleDiceRoll(int rollAmount)
    {
        yield return StartCoroutine(changeNumbersFast(rollAmount));

        if (players[currentPlayerIndex] != null)
        {
            players[currentPlayerIndex].MovePlayerByDiceRoll(rollAmount);
        }
        else
        {
            Debug.LogError("HIBA: currentPlayerIndex nem létező játékosra mutat!");
        }

        rollDiceButton.interactable = false;
    }

    //ends the turn
    public void EndTurn()
    {
        rollDiceButton.interactable = true;
        players[currentPlayerIndex].EndTurn();
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        players[currentPlayerIndex].StartTurn();
    }

    //determine winner
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

    //updates the player UI
    public void UpdatePlayerUI()
    {
        moneyText.text = $"${playerData[currentPlayerIndex].money}";
        goldenTicketText.text = $"Golden tickets: {playerData[currentPlayerIndex].goldenTicketAmount}";
        if(!CurrentPlayerDoesHaveGoldenTicket())
        {
            useGoldenTicketButton.interactable = false;
        }
    }

    //operations with player currencies
    public void AddMoneyToCurrentPlayer(int amount) //money+
    {
        playerData[currentPlayerIndex].money += amount;
    }
    public void AddGoldenTicketToCurrentPlayer()//gt+
    {
        playerData[currentPlayerIndex].goldenTicketAmount++;
    }
    public void SetCurrentPlayerMoney(int amount)//money=
    {
        playerData[currentPlayerIndex].money = amount;
    }
    public void RemoveGoldenTicketFromCurrentPlayer()//gt-
    {
        playerData[currentPlayerIndex].goldenTicketAmount--;
    }
    public int GetCurrentPlayerMoney()//money==
    {
        return playerData[currentPlayerIndex].money;
    }
    public bool CurrentPlayerDoesHaveGoldenTicket()//gt amount
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
    public void RemoveMoneyFromCurrentPlayer(int amount)//money-
    {
        playerData[currentPlayerIndex].money -= amount;
    }

    public void TriggerTileEffectWithGoldenTicket()//use golden ticket
    {
        if(playerData[currentPlayerIndex].goldenTicketAmount > 0)
        {
            RemoveGoldenTicketFromCurrentPlayer();
            players[currentPlayerIndex].HandleTileEffects();
        }
    }

    //getting players
    public PlayerData GetCurrentPlayer()
    {
        Debug.Log($"Current player index: {currentPlayerIndex}");
        Debug.Log($"Current player money: {playerData[currentPlayerIndex].money}");
        return playerData[currentPlayerIndex];
    }
    private PlayerMovement[] FindAllPlayers()
    {
        return FindObjectsOfType<PlayerMovement>();
    }
}
