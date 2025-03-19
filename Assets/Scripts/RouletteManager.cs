using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RouletteManager : MonoBehaviour
{
    private Button[] betButtons;
    private TextMeshProUGUI[] rouletteNumbers;
    private Button pressedButton;
    private string betNumber;
    private int betAmount;
    private string[] blackNumbers = {"2","4","6","8","10","11","13","15","17","20","22","24","26","28","29","31","33","35"};
    private Color currentNumberColor;
    private string chosenNumber;


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
    }

    public void StartRouletteGame(int playerBet)
    {
        rouletteUI.SetActive(true);
        playerUI.SetActive(false);
        rouletteCloseButton.gameObject.SetActive(false);
        yourBetText.text = "";
        winningNumberText.text = "";
        betNumber = "";
        betAmount = playerBet;
    }


    void RouletteGame(string bet)
    {
        yourBetText.text = $"Your bet: {bet}";
        chosenNumber = Convert.ToString(UnityEngine.Random.Range(0,37));
        Debug.Log($"RulettGame meghívása... {chosenNumber}, {chosenNumber.GetType()}, {bet}");

        StartCoroutine(StartRouletteBall());

        if(bet == "0")
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

    private IEnumerator StartRouletteBall()
    {
        for (float i = 0; i < UnityEngine.Random.Range(0.5f, 0.6f); i += 0.01f)
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
    }
    public void CloseRouletteUI()
    {
        rouletteUI.SetActive(false);
        foreach(TextMeshProUGUI number in rouletteNumbers)
        {
            if(number.text == chosenNumber)
            {
                number.color = currentNumberColor;
            }
        }
        rouletteCloseButton.gameObject.SetActive(false);
    }
    
    public string CheckWhichHalf(string number)
    {
        int numberInt = Convert.ToInt32(number);

        if(numberInt > 18)
        {
            return "1to18";
        }
        return "19to36";
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
            return "1st12";
        }
        else if(numberInt>12 && numberInt < 25)
        {
            return "2nd12";
        }
        else if (numberInt > 24)
        {
            return "3rd12";
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

    public void PressButton0()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "0";
        RouletteGame(betNumber);
    }
    public void PressButton1()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "1";
        RouletteGame(betNumber);
    }
    public void PressButton2()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "2";
        RouletteGame(betNumber);
    }
    public void PressButton3()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "3";
        RouletteGame(betNumber);
    }
    public void PressButton4()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "4";
        RouletteGame(betNumber);
    }
    public void PressButton5()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "5";
        RouletteGame(betNumber);
    }
    public void PressButton6()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "6";
        RouletteGame(betNumber);
    }
    public void PressButton7()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "7";
        RouletteGame(betNumber);
    }
    public void PressButton8()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "8";
        RouletteGame(betNumber);
    }
    public void PressButton9()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "9";
        RouletteGame(betNumber);
    }
    public void PressButton10()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "10";
        RouletteGame(betNumber);
    }
    public void PressButton11()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "11";
        RouletteGame(betNumber);
    }
    public void PressButton12()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "12";
        RouletteGame(betNumber);
    }
    public void PressButton13()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "13";
        RouletteGame(betNumber);
    }
    public void PressButton14()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "14";
        RouletteGame(betNumber);
    }
    public void PressButton15()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "15";
        RouletteGame(betNumber);
    }
    public void PressButton16()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "16";
        RouletteGame(betNumber);
    }
    public void PressButton17()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "17";
        RouletteGame(betNumber);
    }
    public void PressButton18()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "18";
        RouletteGame(betNumber);
    }
    public void PressButton19()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "19";
        RouletteGame(betNumber);
    }
    public void PressButton20()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "20";
        RouletteGame(betNumber);
    }
    public void PressButton21()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "21";
        RouletteGame(betNumber);
    }
    public void PressButton22()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "22";
        RouletteGame(betNumber);
    }
    public void PressButton23()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "23";
        RouletteGame(betNumber);
    }
    public void PressButton24()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "24";
        RouletteGame(betNumber);
    }
    public void PressButton25()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "25";
        RouletteGame(betNumber);
    }
    public void PressButton26()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "26";
        RouletteGame(betNumber);
    }
    public void PressButton27()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "27";
        RouletteGame(betNumber);
    }
    public void PressButton28()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "28";
        RouletteGame(betNumber);
    }
    public void PressButton29()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "29";
        RouletteGame(betNumber);
    }
    public void PressButton30()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "30";
        RouletteGame(betNumber);
    }
    public void PressButton31()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "31";
        RouletteGame(betNumber);
    }
    public void PressButton32()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "32";
        RouletteGame(betNumber);
    }
    public void PressButton33()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "33";
        RouletteGame(betNumber);
    }
    public void PressButton34()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "34";
        RouletteGame(betNumber);
    }
    public void PressButton35()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "35";
        RouletteGame(betNumber);
    }
    public void PressButton36()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "36";
        RouletteGame(betNumber);
    }
    public void PressButtonRed()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "red";
        RouletteGame(betNumber);
    }
    public void PressButtonBlack()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "black";
        RouletteGame(betNumber);
    }
    public void PressButtonEven()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "even";
        RouletteGame(betNumber);
    }
    public void PressButtonOdd()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "odd";
        RouletteGame(betNumber);
    }
    public void PressButton1to18()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "1to18";
        RouletteGame(betNumber);
    }
    public void PressButton19to36()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "19to36";
        RouletteGame(betNumber);
    }
    public void PressButton1st12()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "1st12";
        RouletteGame(betNumber);
    }
    public void PressButton2nd12()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "2nd12";
        RouletteGame(betNumber);
    }
    public void PressButton3rd12()
    {
        foreach(Button button in betButtons)
        {
            button.interactable = false;
        }
        betNumber = "3rd12";
        RouletteGame(betNumber);
    }



}
