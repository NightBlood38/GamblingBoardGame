using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
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
    private bool isMultiplayer = PhotonNetwork.InRoom;
    private PhotonView photonView;
    private Vector3[] startPositions = new Vector3[]
    {
        new Vector3(6.33f, 15.75f, -9.36f),  // Player 1
        new Vector3(6.33f, 15.75f, -10.01f), // Player 2
        new Vector3(6.33f, 15.75f, -10.57f), // Player 3
        new Vector3(7.28f, 15.75f, -9.36f),  // Player 4
        new Vector3(7.28f, 15.75f, -10.01f), // Player 5
        new Vector3(7.28f, 15.75f, -10.57f)  // Player 6
    };

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

    IEnumerator StartDelayed()
    {
        yield return new WaitForSeconds(5);
        playerCount = PhotonNetwork.PlayerList.Length;
        players = FindAllPlayers();

        if (players == null || players.Length == 0)
        {
            Debug.LogError("NINCS EGYETLEN PLAYER SEM!");
            yield return null;
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
        if(PhotonNetwork.LocalPlayer.ActorNumber - 1 == currentPlayerIndex){
                rollDiceButton.interactable = true;
                endTurnButton.interactable = true;
        }
        else
        {
            endTurnButton.interactable = false;
            rollDiceButton.interactable = false;
        }
    }
    void Start()
    {
        GameObject player;
        if(isMultiplayer)
        {
            player = PhotonNetwork.Instantiate("PlayerPrefab", startPositions[PhotonNetwork.LocalPlayer.ActorNumber - 1], Quaternion.identity);
            photonView = GetComponent<PhotonView>();
            StartCoroutine(StartDelayed());
            return;
        }
        else
        {
            for (int i = 0; i < playerCount; i++)
            {
                    player = Instantiate(playerPrefab);
                    player.transform.position = startPositions[i];
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
        if(isMultiplayer)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene("StartMenu");
        }
        
    }

    public void NotEnoughMoney()
    {
        notEnoughMoneyUI.SetActive(true);
        playerUI.SetActive(false);
        GameManager.isUIOpen = true;
    }

    //rolling the dice
    private IEnumerator changeNumbersFast(int rollAmount)
    {
        endTurnButton.interactable = false;
        rollDiceButton.interactable = false;
        useGoldenTicketButton.interactable = false;
        for (float i = 0; i < 0.2f; i += 0.01f)
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
        
        if(PhotonNetwork.InRoom)
        {
            this.photonView.RPC("HandleDiceRoll", RpcTarget.All, diceRoll);
        }
        else
        {
            HandleDiceRoll(diceRoll);
        }
        
    }
    [PunRPC]
    public void HandleDiceRoll(int rollAmount)
    {
        StartCoroutine(HandleDiceRollAction(rollAmount));
    }
    public int GetPlayerIndex()
    {
        return currentPlayerIndex;
    }

    IEnumerator HandleDiceRollAction(int rollAmount)
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

    public void EndTurn()
    {
        if(PhotonNetwork.InRoom)
        {
            this.photonView.RPC("EndTurnAction", RpcTarget.All);
        }
        else
        {
            EndTurnAction();
        }
    }
    //ends the turn
    [PunRPC]
    public void EndTurnAction()
    {
        rollDiceButton.interactable = true;
        players[currentPlayerIndex].EndTurn();
        cameras[currentPlayerIndex].SetActive(false);
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        players[currentPlayerIndex].StartTurn();
        cameras[currentPlayerIndex].SetActive(true);
        if(PhotonNetwork.InRoom)
        {
            Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
            Debug.Log(currentPlayerIndex);
            Debug.Log(players.Length);
            Debug.Log("==");
            if(PhotonNetwork.LocalPlayer.ActorNumber - 1 == currentPlayerIndex){
                rollDiceButton.interactable = true;
                endTurnButton.interactable = true;
            }
            else
            {
                endTurnButton.interactable = false;
                rollDiceButton.interactable = false;
            }
        }
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
        if(playerData == null)
        {
            return;
        }
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
