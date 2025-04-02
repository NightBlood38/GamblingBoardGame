using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public Button startGameButton, exitButton, backButton, startButton, creditButton, backCreditButton, multiplayerButton, backMultiplayerButton, hostGame, joinGame, joinButton;
    public TextMeshProUGUI optionsText, playerCountText, creditsText, cardsText, multiplayerText, generatedCodeText;
    public TMP_Dropdown playerCountDropdown;
    public TMP_InputField codeInputBox;
    public GameObject nameInputPrefab;
    public Transform nameInputContainer;
    public static string[] playerNames = new string[4];
    public Canvas startMenu;

    private int playerCount;
    private Button[] buttons;
    private List<GameObject> inputFields = new List<GameObject>();


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
    public void onStartButtonPressed()
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
        
        foreach(Button btn in buttons)
        {
            btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.7f,0f,0f,1f);
        }
    }
    public void StartGame()
    {
        GameManager.playerCount = playerCount;
        for(int i = 0; i < inputFields.Count; i++)
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
        joinGame.gameObject.SetActive(false);
        generatedCodeText.gameObject.SetActive(true);
        hostGame.gameObject.SetActive(false);

    }
    public void OnJoinButtonPressed()
    {
        codeInputBox.gameObject.SetActive(true);
        hostGame.gameObject.SetActive(false);
        joinGame.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(true);
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
    }

}
