using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class StartMenuController : MonoBehaviour
{
    public Button startGameButton, exitButton, backButton, startButton, creditButton, backCreditButton, multiplayerButton, backMultiplayerButton, hostGame, joinGame, joinButton, leaveButton, multiplayerStartButton;
    public TextMeshProUGUI optionsText, playerCountText, creditsText, cardsText, multiplayerText, generatedCodeText, wrongCodeText, noNameText;
    public TMP_Dropdown playerCountDropdown;
    public TMP_InputField codeInputBox, nameInputBox;
    public GameObject nameInputPrefab, multiplayerNamePrefab;
    public Transform nameInputContainer, multiplayerNameContainer;
    public static string[] playerNames = new string[6];
    public Canvas startMenu;
    public MultiplayerManager multiplayerManager;

    private int playerCount;
    private Button[] buttons;
    private List<GameObject> inputFields = new List<GameObject>();
    private List<GameObject> playerNamesList = new List<GameObject>();


    void Start()
    {
        startGameButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        multiplayerButton.gameObject.SetActive(true);
        creditButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(false);
        optionsText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        creditsText.gameObject.SetActive(false);
        cardsText.gameObject.SetActive(false);
        playerCountDropdown.gameObject.SetActive(false);
        playerCountText.gameObject.SetActive(false);
        UpdatePlayerCount(0);
        GenerateInputFields();
        nameInputContainer.gameObject.SetActive(false);
        buttons = FindObjectsOfType<Button>();
    }
    public void UpdatePlayerCount(int index)
    {
        playerCount = index+2;
        Debug.Log("Játékosok száma: " + playerCount);
    }

    public void GenerateInputFields()
    {
        foreach (var field in inputFields)
        {
            Destroy(field);
        }
        inputFields.Clear();
        
        for (int i = 0; i < playerCount; i++)
        {
            GameObject inputGO = Instantiate(nameInputPrefab, nameInputContainer);
            inputGO.GetComponentInChildren<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = $"Player {i + 1} Name";
            inputFields.Add(inputGO);
        }
    }

    public int GetPlayerCount()
    {
        return playerCount;
    }
    public void onStartGameButtonPressed()
    {
        startGameButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        creditButton.gameObject.SetActive(false);
        multiplayerButton.gameObject.SetActive(false);
        creditsText.gameObject.SetActive(false);
        cardsText.gameObject.SetActive(false);
        backButton.gameObject.SetActive(true);
        optionsText.gameObject.SetActive(true);
        startButton.gameObject.SetActive(true);
        playerCountDropdown.gameObject.SetActive(true);
        playerCountText.gameObject.SetActive(true);
        nameInputContainer.gameObject.SetActive(true);
    }

    public void onBackButtonPressed()
    {
        startGameButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        creditButton.gameObject.SetActive(true);
        creditsText.gameObject.SetActive(false);
        backCreditButton.gameObject.SetActive(false);
        cardsText.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        optionsText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        playerCountDropdown.gameObject.SetActive(false);
        playerCountText.gameObject.SetActive(false);
        nameInputContainer.gameObject.SetActive(false);
        multiplayerButton.gameObject.SetActive(true);
        multiplayerText.gameObject.SetActive(false);
        backMultiplayerButton.gameObject.SetActive(false);
        joinGame.gameObject.SetActive(false);
        hostGame.gameObject.SetActive(false);
        generatedCodeText.gameObject.SetActive(false);
        codeInputBox.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        wrongCodeText.gameObject.SetActive(false);
        noNameText.gameObject.SetActive(false);
        nameInputBox.gameObject.SetActive(false);
        multiplayerNameContainer.gameObject.SetActive(false);
        
        foreach(Button btn in buttons)
        {
            btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.7f,0f,0f,1f);
        }
    }
    public void StartGame()
    {
        GameManager.playerCount = playerCount;
        for(int i = 0; i < playerCount; i++)
        {
            playerNames[i] = inputFields[i].GetComponent<TMP_InputField>().text;
            Debug.Log(playerNames[i]);
        }
        if(playerNames[inputFields.Count-1] != "")
        {
            SceneManager.LoadScene("GameScene");
        }
    }
    public void HoveringOverButton(TextMeshProUGUI currentButton)
    {
        currentButton.color = new Color(1f,0f,0f,1f);
    }
    public void EndHoveringOverButton(TextMeshProUGUI currentButton)
    {
        currentButton.color = new Color(0.7f,0f,0f,1f);
    }
    public void OnExitButtonPressed()
    {
        Application.Quit();
    }
    public void OnCreditsButtonPressed()
    {
        startGameButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        backCreditButton.gameObject.SetActive(true);
        creditButton.gameObject.SetActive(false);
        creditsText.gameObject.SetActive(true);
        cardsText.gameObject.SetActive(true);
        multiplayerButton.gameObject.SetActive(false);
    }
    public void OnHostButtonPressed()
    {
        if(nameInputBox.text != "")
        {
            SetPlayerName();
            multiplayerNameContainer.gameObject.SetActive(true);
            joinGame.gameObject.SetActive(false);
            generatedCodeText.gameObject.SetActive(true);
            hostGame.gameObject.SetActive(false);
            nameInputBox.gameObject.SetActive(false);
            noNameText.gameObject.SetActive(false);
            nameInputBox.gameObject.SetActive(false);
            multiplayerManager.CreateRoom();
            //multiplayerManager.JoinRoom();
            backMultiplayerButton.gameObject.SetActive(false);
            leaveButton.gameObject.SetActive(true);
            multiplayerStartButton.gameObject.SetActive(true);
            UpdatePlayerList();
        }
        else
        {
            noNameText.gameObject.SetActive(true);
        }
    }

    public void OnSuccessfulJoin()
    {
        SetPlayerName();
        multiplayerNameContainer.gameObject.SetActive(true);
        joinGame.gameObject.SetActive(false);
        generatedCodeText.gameObject.SetActive(true);
        hostGame.gameObject.SetActive(false);
        nameInputBox.gameObject.SetActive(false);
        noNameText.gameObject.SetActive(false);
        nameInputBox.gameObject.SetActive(false);
        backMultiplayerButton.gameObject.SetActive(false);
        leaveButton.gameObject.SetActive(true);
        codeInputBox.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        UpdatePlayerList();
    }
    public void OnStartButtonPressed()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
    public void OnLeaveButtonPressed()
    {
        onBackButtonPressed();
        Debug.Log("room left");
        PhotonNetwork.LeaveRoom();
        leaveButton.gameObject.SetActive(false);
    }
    public void OnJoinButtonPressed()
    {
        if(nameInputBox.text != "")
        {
            SetPlayerName();
            codeInputBox.gameObject.SetActive(true);
            hostGame.gameObject.SetActive(false);
            joinGame.gameObject.SetActive(false);
            joinButton.gameObject.SetActive(true);
            nameInputBox.gameObject.SetActive(false);
            noNameText.gameObject.SetActive(false);
            nameInputBox.gameObject.SetActive(false);
        }
        else
        {
            noNameText.gameObject.SetActive(true);
        }
        
    }

    public void OnJoinFailed()
    {
        wrongCodeText.gameObject.SetActive(true);
    }

    public void OnMultiplayerButtonPressed()
    {
        startGameButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        backMultiplayerButton.gameObject.SetActive(true);
        multiplayerButton.gameObject.SetActive(false);
        multiplayerText.gameObject.SetActive(true);
        creditButton.gameObject.SetActive(false);
        joinGame.gameObject.SetActive(true);
        hostGame.gameObject.SetActive(true);
        nameInputBox.gameObject.SetActive(true);
    }

    public void SetPlayerName()
    {
        PhotonNetwork.NickName = nameInputBox.text;
        Debug.Log($"name set to {PhotonNetwork.NickName}");
    }

    public void UpdatePlayerList()
    {
        Debug.Log($"PhotonNetwork.IsConnected: {PhotonNetwork.IsConnected}");
        Debug.Log($"PhotonNetwork.InLobby: {PhotonNetwork.InLobby}");
        Debug.Log($"PhotonNetwork.InRoom: {PhotonNetwork.InRoom}");
        List<string> multiplayerPlayerNames= new List<string>();
        foreach (var player in playerNamesList)
        {
            Destroy(player);
        }
        playerNamesList.Clear();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            multiplayerPlayerNames.Add(player.NickName);
        }

        for (int i = 0; i < multiplayerPlayerNames.Count; i++)
        {
            GameObject playerNameGO = Instantiate(multiplayerNamePrefab, multiplayerNameContainer);
            playerNameGO.GetComponentInChildren<TextMeshProUGUI>().text = multiplayerPlayerNames[i];
            playerNamesList.Add(playerNameGO);
            Debug.Log(multiplayerPlayerNames[i]);
        }
        Debug.Log($"PhotonNetwork.PlayerList count: {PhotonNetwork.PlayerList.Length}");
    }
    

}
