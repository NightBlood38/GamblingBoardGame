using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class WheelOfFortuneManager : MonoBehaviourPun
{
    
    public GameObject wheelOfFortuneUI, wheelOfFortunePanel, playerUI;
    public Button wheelOfFortuneSpinButton, wheelOfFortuneCloseButton;
    public GameManager gameManager;
    public TextMeshProUGUI[] textElements = new TextMeshProUGUI[12];
    
    private float radius = 300f;
    private Vector2 screenCenter;

    private void Start()
    {
        //getting text elements of wofUIPanel
        for(int i = 0; i < textElements.Length; i++)
        {
            textElements[i] = wheelOfFortunePanel.transform.GetChild(i).GetComponent<TextMeshProUGUI>();
        }
        screenCenter = new Vector2(0,0);

        //setting positions of text elements
        for (int i = 0; i < textElements.Length; i++)
        {
            float angle = i * (360f / textElements.Length);
            float radians = angle * Mathf.Deg2Rad;

            float x = screenCenter.x + radius * Mathf.Cos(radians);
            float y = screenCenter.y + radius * Mathf.Sin(radians);

            textElements[i].rectTransform.anchoredPosition = new Vector2(x, y);
        }
    }

    //starting wof game
    public void StartWheelOfFortuneGame()
    {
        wheelOfFortuneUI.SetActive(true);
        playerUI.SetActive(false);
        wheelOfFortuneCloseButton.interactable = false;
        wheelOfFortuneSpinButton.interactable = true;
        for(int i = 0; i < textElements.Length; i++)
        {
            textElements[i].color = Color.white;
        }
        if(PhotonNetwork.InRoom)
        {
            if(PhotonNetwork.LocalPlayer.ActorNumber - 1 == gameManager.GetPlayerIndex())
            {
                wheelOfFortuneSpinButton.interactable = true;
            }
            else
            {
                wheelOfFortuneSpinButton.interactable = false;
            }
        }
    }

    public void SpinButtonPressed()
    {
        if(PhotonNetwork.InRoom)
        {
            StartCoroutine(Spinmp());
        }
        else
        {
            StartCoroutine(Spinsp());
        }
    }

    [PunRPC]
    private void CloseWheelOfFortuneUIAction()
    {
        wheelOfFortuneUI.SetActive(false);
        playerUI.SetActive(true);
    }
    public void CloseWheelOfFortune()
    {
        if(PhotonNetwork.InRoom)
        {
            this.photonView.RPC("CloseWheelOfFortuneUIAction", RpcTarget.All);
        }
        else
        {
            CloseWheelOfFortuneUIAction();
        }
    }
    [PunRPC]
    private void SpinmpRPCChangeNumberColorToBlack(int blackNumber)
    {
        textElements[blackNumber].color = Color.black;
    }
    [PunRPC]
    private void SpinmpRPCChangeNumberColorToWhite(int whiteNumber)
    {
        textElements[whiteNumber].color = Color.white;
    }
    private IEnumerator Spinmp()
    {
        wheelOfFortuneSpinButton.interactable = false;
        int currentlyBlackNumber = 11;
        for (float i = 0; i < Random.Range(0.2f, 0.7f); i += 0.01f)
        {
            this.photonView.RPC("SpinmpRPCChangeNumberColorToBlack", RpcTarget.All, currentlyBlackNumber);
            
            yield return new WaitForSeconds(i);
            this.photonView.RPC("SpinmpRPCChangeNumberColorToWhite", RpcTarget.All, currentlyBlackNumber);
            currentlyBlackNumber--;
            if(currentlyBlackNumber == -1)
            {
                currentlyBlackNumber = 11;
            }
        }
        this.photonView.RPC("SpinmpRPCChangeNumberColorToBlack", RpcTarget.All, currentlyBlackNumber);
        switch(textElements[currentlyBlackNumber].text)
        {
            case "$500":
                gameManager.AddMoneyToCurrentPlayer(500);
                break;
            case "$1000":
                gameManager.AddMoneyToCurrentPlayer(1000);
                break;
            case "$1500":
                gameManager.AddMoneyToCurrentPlayer(1500);
                break;
            case "$2000":
                gameManager.AddMoneyToCurrentPlayer(2000);
                break;
            case "$2500":
                gameManager.AddMoneyToCurrentPlayer(2500);
                break;
            case "$4000":
                gameManager.AddMoneyToCurrentPlayer(4000);
                break;
            case "$5000":
                gameManager.AddMoneyToCurrentPlayer(5000);
                break;
            case "$7000":
                gameManager.AddMoneyToCurrentPlayer(7000);
                break;
            case "$10000":
                gameManager.AddMoneyToCurrentPlayer(10000);
                break;
        }
        if(PhotonNetwork.LocalPlayer.ActorNumber - 1 == gameManager.GetPlayerIndex())
        {
            wheelOfFortuneCloseButton.interactable = true;
        }
        else
        {
            wheelOfFortuneCloseButton.interactable = false;
        }
    }
    

    private IEnumerator Spinsp()
    {
        wheelOfFortuneSpinButton.interactable = false;
        int currentlyBlackNumber = 11;
        for (float i = 0; i < Random.Range(0.2f, 0.7f); i += 0.01f)
        {
            textElements[currentlyBlackNumber].color = Color.black;
            yield return new WaitForSeconds(i);
            textElements[currentlyBlackNumber].color = Color.white;
            currentlyBlackNumber--;
            if(currentlyBlackNumber == -1)
            {
                currentlyBlackNumber = 11;
            }
        }
        textElements[currentlyBlackNumber].color = Color.black;
        switch(textElements[currentlyBlackNumber].text)
        {
            case "$500":
                gameManager.AddMoneyToCurrentPlayer(500);
                break;
            case "$1000":
                gameManager.AddMoneyToCurrentPlayer(1000);
                break;
            case "$1500":
                gameManager.AddMoneyToCurrentPlayer(1500);
                break;
            case "$2000":
                gameManager.AddMoneyToCurrentPlayer(2000);
                break;
            case "$2500":
                gameManager.AddMoneyToCurrentPlayer(2500);
                break;
            case "$4000":
                gameManager.AddMoneyToCurrentPlayer(4000);
                break;
            case "$5000":
                gameManager.AddMoneyToCurrentPlayer(5000);
                break;
            case "$7000":
                gameManager.AddMoneyToCurrentPlayer(7000);
                break;
            case "$10000":
                gameManager.AddMoneyToCurrentPlayer(10000);
                break;
        }
        wheelOfFortuneCloseButton.interactable = true;
    }
}
