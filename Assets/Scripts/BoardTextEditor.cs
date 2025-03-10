/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardTextManager : MonoBehaviour
{
    public static TextMeshPro[] moneyTexts = new TextMeshPro[4];

    void Start()
    {
        UpdateMoneyDisplay();
    }

    public static void UpdateMoneyDisplay()
    {
        for (int i = 0; i < PlayerMovement.money.Length; i++)
        {
            moneyTexts[i].text = "$" + PlayerMovement.money[i].ToString();
        }
    }

}
*/