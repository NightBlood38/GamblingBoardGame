using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
    public GameObject shopUI;
    public GameObject winnerUI;
    public GameObject playerPrefab;
    public TextMeshProUGUI rollDiceNumber, resumeButtonText, exitButtonText, playerNameText;
    public GameObject[] cameras = new GameObject[6];
    public TextMeshProUGUI winnerText;
    public ParticleSystem particleEffect;
    public static int playerCount;

    private PlayerMovement[] players;
    private int currentPlayerIndex = 0;
    private PlayerData[] playerData;

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
        for (int i = 0; i < playerCount; i++)
        {
            GameObject player = Instantiate(playerPrefab);
            switch(i)
            {
                case 0:
                    player.transform.position = new Vector3(6.33f, 15.75f, -9.36f);
                    break;
                case 1:
                    player.transform.position = new Vector3(6.33f, 15.75f, -10.01f);
                    break;
                case 2:
                    player.transform.position = new Vector3(6.33f, 15.75f, -10.57f);
                    break;
                case 3:
                    player.transform.position = new Vector3(7.28f, 15.75f, -9.36f);
                    break;
                case 4:
                    player.transform.position = new Vector3(7.28f, 15.75f, -10.01f);
                    break;
                case 5:
                    player.transform.position = new Vector3(7.28f, 15.75f, -10.57f);
                    break;
            }
        }
        players = FindAllPlayers();

        if (players == null || players.Length == 0)
        {
            Debug.LogError("NINCS EGYETLEN PLAYER SEM!");
            return;
        }
        for(int i = 1; i < cameras.Length; i++)
        {
            cameras[i].SetActive(false);
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
            playerData[i].SetPlayerName(StartMenuController.playerNames[i]);

            if (playerData[i] == null)
            {
                Debug.LogError($"A(z) {i}. játékosnak nincs PlayerData komponense!");
            }
        }
        players[0].StartTurn();
        useGoldenTicketButton.interactable = false;
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
    public void ExitButtonPressed()
    {
        SceneManager.LoadScene("StartMenu");
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
        cameras[currentPlayerIndex].SetActive(false);
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        players[currentPlayerIndex].StartTurn();
        cameras[currentPlayerIndex].SetActive(true);
    }

    //determine winner
    public void Win(){
        int itemCount = 0;
        for(int i = 0; i < 5; i++){
            if(playerData[currentPlayerIndex].haveItems[i])
            {
                itemCount++;
            }
        }
        if(itemCount == 5)
        {
            shopUI.SetActive(false);
            playerUI.SetActive(false);
            winnerUI.SetActive(true);
            winnerText.text = GetCurrentPlayer().playerName + " WINS!";
            particleEffect.Play();
        }
    }
    public void DisplayWinner(string playerName)
    {
        
        winnerText.gameObject.SetActive(true);
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
        playerNameText.text = playerData[currentPlayerIndex].playerName;
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
