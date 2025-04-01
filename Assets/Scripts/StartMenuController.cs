using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public Button startGameButton, exitButton, backButton, startButton;
    public TextMeshProUGUI optionsText, playerCountText;
    public TMP_Dropdown playerCountDropdown;
    public GameObject nameInputPrefab;
    public Transform nameInputContainer;
    public static string[] playerNames = new string[4];

    private int playerCount;
    private List<GameObject> inputFields = new List<GameObject>();


    void Start()
    {
        startGameButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(false);
        optionsText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        playerCountDropdown.gameObject.SetActive(false);
        playerCountText.gameObject.SetActive(false);
        UpdatePlayerCount(0);
        GenerateInputFields();
        nameInputContainer.gameObject.SetActive(false);
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
        backButton.gameObject.SetActive(false);
        optionsText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        playerCountDropdown.gameObject.SetActive(false);
        playerCountText.gameObject.SetActive(false);
        nameInputContainer.gameObject.SetActive(false);
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

}
