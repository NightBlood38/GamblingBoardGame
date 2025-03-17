using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public Sprite[] cardSprites; // Az összes kártyakép tárolása

    void Awake()
    {
        // Betölti az összes kártya sprite-ot a mappából
        cardSprites = Resources.LoadAll<Sprite>("Standard 52 Cards/Standard Rounded Cards/Cards");
        
        if (cardSprites.Length == 0)
        {
            Debug.LogError("Nem találhatóak a kártyák! Ellenőrizd az elérési utat.");
        }
    }

    public Sprite GetCardSprite(int index)
    {
        if (index < 0 || index >= cardSprites.Length)
        {
            Debug.LogError("Érvénytelen kártya index: " + index);
            return null;
        }
        return cardSprites[index];
    }
}
