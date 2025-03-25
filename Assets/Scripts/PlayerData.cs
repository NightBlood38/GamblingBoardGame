using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int money = 0;
    public bool[] haveItems = {false,false,false,false,false};
    public int goldenTicketAmount = 0;
    public int previousBetSum = 0;
    public string playerName = "PlaceHolder";

    public bool SpendMoney(int amount)
    {
    Debug.Log($"Trying to spend {amount}. Current money: {money}");

    if (money >= amount)
    {
        money -= amount;
        Debug.Log($"Purchase successful! Remaining money: {money}");
        return true;
    }
    Debug.Log("Purchase failed! Not enough money.");
    return false;
    }
}

