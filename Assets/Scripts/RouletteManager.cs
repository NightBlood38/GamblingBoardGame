using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RouletteManager : MonoBehaviour
{
    public Button[] betButtons;
    public TextMeshProUGUI[] rouletteNumbers;
    public GameObject rouletteUI;
    public GameObject roulettePanel;
    public GameObject playerUI;

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

        rouletteNumbers = texts.ToArray(); // Lista átalakítása tömbbé

        Debug.Log($"Talált közvetlen TextMeshProUGUI elemek száma: {rouletteNumbers.Length}");
    }

    void StartRouletteGame()
    {
        rouletteUI.SetActive(true);
        playerUI.SetActive(false);
    }
}
