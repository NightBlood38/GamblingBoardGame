using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Photon.Pun;

public class RouletteManager : MonoBehaviourPun
{
    private Button[] betButtons;
    private TextMeshProUGUI[] rouletteNumbers;
    private Button pressedButton;
    private string betNumber;
    private int betAmount;
    private string[] blackNumbers = {"2","4","6","8","10","11","13","15","17","20","22","24","26","28","29","31","33","35"};
    private Color currentNumberColor;
    private string chosenNumber;
    private Color prevMpColor;
    private Color[] allNumberColors = new Color[37];


    public GameObject rouletteUI;
    public GameObject roulettePanel;
    public GameObject playerUI;
    public GameManager gameManager;
    public Button rouletteCloseButton;
    public TextMeshProUGUI yourBetText;
    public TextMeshProUGUI winningNumberText;

    void Start()
    {
        betButtons = roulettePanel.GetComponentsInChildren<Button>();
        rouletteUI.SetActive(false);
        List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();

        foreach (Transform child in roulettePanel.transform)
        {
            TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                texts.Add(text);
            }
        }

        rouletteNumbers = texts.ToArray();
        for(int i = 0; i < allNumberColors.Length; i++)
        {
            allNumberColors[i] = rouletteNumbers[i].color;
        }
    }

    public void StartRouletteGame(int playerBet)
    {
        for(int i = 0; i < allNumberColors.Length; i++)
        {
            rouletteNumbers[i].color = allNumberColors[i];
        }
        rouletteUI.SetActive(true);
        playerUI.SetActive(false);
        rouletteCloseButton.gameObject.SetActive(false);
        yourBetText.text = "";
        winningNumberText.text = "";
        betNumber = "";
        betAmount = playerBet;
        gameManager.RemoveMoneyFromCurrentPlayer(playerBet);
        if(PhotonNetwork.InRoom)
        {
            this.photonView.RPC("DisableBetButtonsForClients", RpcTarget.All);
        }
    }
        

    //start roulette game
    void RouletteGame(string bet)
    {
        yourBetText.text = $"Your bet: {bet}";
        chosenNumber = Convert.ToString(UnityEngine.Random.Range(0,37));
        Debug.Log($"RulettGame meghívása... {chosenNumber}, {chosenNumber.GetType()}, {bet}");

        StartRouletteBall();

        if(chosenNumber == "0")
        {
            gameManager.AddMoneyToCurrentPlayer(betAmount*100);
            Debug.Log($"nyertél! Te a {bet} számra kattintottál és a nyerőszám is {chosenNumber} volt");
        }
        else if(bet == chosenNumber)
        {
            gameManager.AddMoneyToCurrentPlayer(betAmount*35);
            Debug.Log($"nyertél! Te a {bet} számra kattintottál és a nyerőszám is {chosenNumber} volt");
        }
        else if(CheckWhich12(chosenNumber) == bet)
        {
            gameManager.AddMoneyToCurrentPlayer(betAmount*3);
            Debug.Log($"nyertél! Te a {bet} számra kattintottál és a nyerőszám is {chosenNumber} volt");
        }
        else if(CheckOddOrEven(chosenNumber) == bet)
        {
            gameManager.AddMoneyToCurrentPlayer(betAmount*2);
            Debug.Log($"nyertél! Te a {bet} számra kattintottál és a nyerőszám is {chosenNumber} volt");
        }
        else if(CheckWhichHalf(chosenNumber) == bet){
            gameManager.AddMoneyToCurrentPlayer(betAmount*2);
            Debug.Log($"nyertél! Te a {bet} számra kattintottál és a nyerőszám is {chosenNumber} volt");
        }
        else if(GetNumberColor(chosenNumber) == bet)
        {
            gameManager.AddMoneyToCurrentPlayer(betAmount*2);
            Debug.Log($"nyertél! Te a {bet} számra kattintottál és a nyerőszám is {chosenNumber} volt");
        }
    }

    //close roulette UI
    public void CloseRouletteUI()
    {
        if(PhotonNetwork.InRoom)
        {
            this.photonView.RPC("CloseRouletteUIAction", RpcTarget.All);
        }
        else
        {
            CloseRouletteUIAction();
        }
    }
    [PunRPC]
    public void CloseRouletteUIAction()
    {
        rouletteUI.SetActive(false);
        foreach(TextMeshProUGUI number in rouletteNumbers)
        {
            if(number.text == chosenNumber)
            {
                number.color = currentNumberColor;
            }
        }
        foreach(Button button in betButtons)
        {
            button.interactable = true;
        }
        rouletteCloseButton.gameObject.SetActive(false);
        playerUI.SetActive(true);
    }
    
    //checking win conditions
    public string CheckWhichHalf(string number)
    {
        int numberInt = Convert.ToInt32(number);

        if(numberInt > 18)
        {
            return "1-18";
        }
        return "19-36";
    }

    public string CheckOddOrEven(string number)
    {
        int numberInt = Convert.ToInt32(number);
        if(numberInt%2 == 0)
        {
            return "even";
        }
        return "odd";
    }

    public string CheckWhich12(string number)
    {
        int numberInt = Convert.ToInt32(number);
        if(numberInt<13 && numberInt != 0)
        {
            return "1st 12";
        }
        else if(numberInt>12 && numberInt < 25)
        {
            return "2nd 12";
        }
        else if (numberInt > 24)
        {
            return "3rd 12";
        }
        return "0";
    }
    public string GetNumberColor(string number)
    {
        if(number == "0") return "green";
        for (int i = 0; i < blackNumbers.Length; i++)
        {
            if(blackNumbers[i] == number)
            {
                return "black";
            }
        }
        return "red";
    }

    //press bet button
    public void PressButton(Button pressedButton)
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = pressedButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        RouletteGame(betNumber);
    }
    private IEnumerator Wait(int time)
    {
        yield return new WaitForSeconds(time);
    }
    public void StartRouletteBall()
    {
        if(PhotonNetwork.InRoom)
        {
            StartCoroutine(StartRouletteBallmp());
        }
        else
        {
            StartCoroutine(StartRouletteBallsp());
            
        }
    }

    //making random numbers white then back to original color
    private IEnumerator StartRouletteBallsp()
    {
        for (float i = 0; i < UnityEngine.Random.Range(0.4f, 0.5f); i += 0.01f)
        {
            int currentlyWhiteNumber = UnityEngine.Random.Range(0,36);
            currentNumberColor = rouletteNumbers[currentlyWhiteNumber].color;
            rouletteNumbers[currentlyWhiteNumber].color = Color.white;
            yield return new WaitForSeconds(i);
            rouletteNumbers[currentlyWhiteNumber].color = currentNumberColor;
        }
        foreach(TextMeshProUGUI number in rouletteNumbers)
        {
            if(number.text == chosenNumber)
            {
                currentNumberColor = number.color;
                number.color = Color.white;
            }
        }
        winningNumberText.text = $"Winning number {chosenNumber}";
        rouletteCloseButton.gameObject.SetActive(true);
        rouletteCloseButton.interactable = true;
    }
    
    private IEnumerator StartRouletteBallmp()
    {
        int previousWhiteNumber = -1;
        for (float i = 0.45f; i < UnityEngine.Random.Range(0.6f, 0.7f); i += 0.01f)
        {
            int currentlyWhiteNumber = UnityEngine.Random.Range(0,36);
            this.photonView.RPC("RouletteBallmpRPC", RpcTarget.All, currentlyWhiteNumber, chosenNumber, previousWhiteNumber, betNumber);
            previousWhiteNumber = currentlyWhiteNumber;
            yield return new WaitForSeconds(i);
        }
        
        this.photonView.RPC("RouletteBallWinRPC", RpcTarget.All, chosenNumber);
    }
    [PunRPC]
    private void DisableBetButtonsForClients()
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber - 1 != gameManager.GetPlayerIndex())
        {
            foreach(Button button in betButtons)
            {
                button.interactable = false;
            }
        }
        else
        {
            foreach(Button button in betButtons)
            {
                button.interactable = true;
            }
        }
    }
    [PunRPC]
    private void RouletteBallmpRPC(int currentlyWhiteNumber, string chosenNumber, int previousWhiteNumber, string betString)
    {
        
        currentNumberColor = rouletteNumbers[currentlyWhiteNumber].color;
        rouletteNumbers[currentlyWhiteNumber].color = Color.white;
        if(previousWhiteNumber != -1)
        {
            rouletteNumbers[previousWhiteNumber].color = prevMpColor;
        }
        prevMpColor = currentNumberColor;

        if(PhotonNetwork.LocalPlayer.ActorNumber - 1 == gameManager.GetPlayerIndex())
        {
            rouletteCloseButton.gameObject.SetActive(true);
            rouletteCloseButton.interactable = true; 
        }   
        else{
            rouletteCloseButton.gameObject.SetActive(true);
            rouletteCloseButton.interactable = false;
            yourBetText.text = $"Their bet: {betString}";
        }
    }
    [PunRPC]
    private void RouletteBallWinRPC(string chosenNumber)
    {
        for(int i = 0; i < allNumberColors.Length; i++)
        {
            rouletteNumbers[i].color = allNumberColors[i];
        }
        foreach(TextMeshProUGUI number in rouletteNumbers)
        {
            if(number.text == chosenNumber)
            {
                currentNumberColor = number.color;
                number.color = Color.white;
            }
        }
        
        winningNumberText.text = $"Winning number {chosenNumber}";
        if(PhotonNetwork.LocalPlayer.ActorNumber - 1 == gameManager.GetPlayerIndex())
        {
            rouletteCloseButton.gameObject.SetActive(true);
            rouletteCloseButton.interactable = true;            
        }   
        else{
            rouletteCloseButton.gameObject.SetActive(false);
            rouletteCloseButton.interactable = false;
        }

    }
    
}
