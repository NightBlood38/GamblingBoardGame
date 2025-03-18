using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private bool isTurn = false;
    private float moveSpeed = 3.0f;
    private PlayerData playerData;

    public ShopManager shopManager;
    public BlackjackManager blackjackManager;
    public LuckyCardsManager luckyCardsManager;
    public static int blackjackBetAmount;
    public GameManager gameManager;
    public Button endTurnButton;
    public GameObject rollDiceUI;
    
    public string[] tileNames = {
        "start", "money500", "blackjack2000", "lucky card", "wheel of fortune", 
        "money1000", "roulette", "shop", "money1000", "blackjack3000", 
        "lucky card", "wheel of fortune", "money2000", "roulette", "shop", 
        "money2000", "blackjack4000", "lucky card", "wheel of fortune", 
        "money3000", "roulette", "shop", "money3000", "blackjack5000", 
        "lucky card", "wheel of fortune", "money5000", "roulette"
    };

    void Start()
    {
        playerData = GetComponent<PlayerData>();
        InitializeWaypoints();
        MovePlayerToWaypoint(0, instant: true);
    }

    public void StartTurn()
    {
        isTurn = true;
        Debug.Log($"{gameObject.name} körön van.");
    }

    public void EndTurn()
    {
        isTurn = false;
    }

    public void MovePlayerByDiceRoll(int roll)
    {
        if (!isTurn) return;
        
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("HIBA: A waypoints tömb nincs inicializálva!");
            return;
        }

        StartCoroutine(SmoothMovePlayerThroughWaypoints(roll));
    }

    IEnumerator SmoothMovePlayerThroughWaypoints(int roll)
    {
        Transform playerTransform = transform;
        int startWaypointIndex = currentWaypointIndex;
        bool addedMoneyOnce = false;
        endTurnButton.interactable = false;

        for (int i = 0; i < roll; i++)
        {
            int nextWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

            yield return StartCoroutine(SmoothMoveBetweenTwoWaypoints(
                playerTransform,
                waypoints[currentWaypointIndex].position,
                waypoints[nextWaypointIndex].position
            ));

            currentWaypointIndex = nextWaypointIndex;

            if (currentWaypointIndex < startWaypointIndex && !addedMoneyOnce)
            {
                addedMoneyOnce = true;
                gameManager.AddMoneyToCurrentPlayer(2000);
            }
        }

        HandleTileEffects();
        endTurnButton.interactable = true;
        rollDiceUI.SetActive(false);
    }

    public void HandleTileEffects()
    {
        string tile = tileNames[currentWaypointIndex];
        
        switch (tile)
        {
            case "money500":
                gameManager.AddMoneyToCurrentPlayer(500);
                break;
            case "money1000":
                gameManager.AddMoneyToCurrentPlayer(1000);
                break;
            case "money2000":
                gameManager.AddMoneyToCurrentPlayer(2000);
                break;
            case "money3000":
                gameManager.AddMoneyToCurrentPlayer(3000);
                break;
            case "money5000":
                gameManager.AddMoneyToCurrentPlayer(5000);
                break;
            case "start":
                gameManager.AddMoneyToCurrentPlayer(2000);
                break;
            case "blackjack2000": 
                if(playerData.money < 2000)
                {
                    blackjackManager.NotEnoughMoney();
                }
                else
                {
                    playerData.previousBetSum += 2000;
                    StartCoroutine(blackjackManager.StartNewGame(2000));
                }                
                break;
            case "blackjack3000": 
                if(playerData.money < 3000)
                {
                    blackjackManager.NotEnoughMoney();
                }
                else
                {
                    playerData.previousBetSum += 3000;
                    StartCoroutine(blackjackManager.StartNewGame(3000));
                }
                break;
            case "blackjack4000": 
                if(playerData.money < 4000)
                {
                    blackjackManager.NotEnoughMoney();
                }
                else
                {
                    playerData.previousBetSum += 4000;
                    StartCoroutine(blackjackManager.StartNewGame(4000));
                }
                break;
            case "blackjack5000": 
                if(playerData.money < 5000)
                {
                    blackjackManager.NotEnoughMoney();
                }
                else
                {
                    playerData.previousBetSum += 5000;
                    StartCoroutine(blackjackManager.StartNewGame(5000));
                }
                break;
            case "lucky card":
                luckyCardsManager.DrawLuckyCard();
                break;
            case "shop":
                shopManager.OpenShop();
                break;
        }
        
        Debug.Log($"{gameObject.name} lépett a(z) {tile} mezőre.");
    }

    IEnumerator SmoothMoveBetweenTwoWaypoints(Transform playerTransform, Vector3 start, Vector3 end)
    {
        float elapsedTime = 0f;
        float journeyLength = Vector3.Distance(start, end);

        while (elapsedTime < journeyLength / moveSpeed)
        {
            playerTransform.position = Vector3.Lerp(start, end, (elapsedTime * moveSpeed) / journeyLength);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerTransform.position = end;
    }

    void MovePlayerToWaypoint(int waypointIndex, bool instant = false)
    {
        if (instant)
        {
            transform.position = waypoints[waypointIndex].position;
            currentWaypointIndex = waypointIndex;
        }
    }

    void InitializeWaypoints()
    {
        waypoints = new Transform[28];
        Vector3 currentPosition = transform.position;

        for (int i = 0; i < waypoints.Length; i++)
        {
            GameObject waypointObject = new GameObject($"{tileNames[i]}");
            waypointObject.transform.position = GetWaypointPosition(i, ref currentPosition);
            waypoints[i] = waypointObject.transform;
        }
    }

    Vector3 GetWaypointPosition(int index, ref Vector3 currentPosition)
    {
        if (index > 0)
        {
            if (index < 8) currentPosition.x -= 2;
            else if (index < 15) currentPosition.z += 2;
            else if (index < 22) currentPosition.x += 2;
            else currentPosition.z -= 2;
        }

        return new Vector3(currentPosition.x, 15.75f, currentPosition.z);
    }
}
