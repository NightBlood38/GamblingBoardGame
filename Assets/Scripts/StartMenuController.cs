using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartMenuController : MonoBehaviour
{
    public Button startGameButton, exitButton, backButton, startButton;
    public TextMeshProUGUI optionsText, playerCountText;
    public TMP_Dropdown playerCountDropdown;
    public GameObject inputFieldPrefab;
    public Transform inputFieldParent;

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
        playerCountDropdown.onValueChanged.AddListener(UpdatePlayerCount);
    }

    void UpdatePlayerCount(int index)
    {
        playerCount = index+2;
        Debug.Log("Játékosok száma: " + playerCount);
    }
    public void OnPlayerCountChanged()
    {
        foreach (var field in inputFields)
        {
            Destroy(field);
        }
        inputFields.Clear();

        playerCount = playerCountDropdown.value + 2;
        for (int i = 0; i < playerCount; i++)
        {
            GameObject newInput = Instantiate(inputFieldPrefab, inputFieldParent);
            newInput.GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = $"Játékos {i + 1} neve";
            inputFields.Add(newInput);
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
    }
    public void StartGame()
    {

    }

}
