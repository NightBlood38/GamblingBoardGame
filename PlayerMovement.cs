using UnityEngine;
using System.Collections;
//using Blackjack;

public class PlayerMovement : MonoBehaviour
{
    private int currentPlayerIndex = 0;
    private int diceRoll = 0;
    private GameObject[] players;
    private Transform[] waypoints;
    private int[] playerWaypointIndices; // Track each player's current waypoint index
    private bool isCurrentPlayerTurnOver = false;
    private float moveSpeed = 3.0f; // Speed of movement

    private bool waypointsInitialized = false; // Flag to ensure initialization happens only once

    public string[] tileNames = {"start", "money500", "blackjack", "lucky card", "wheel of fortune", "money1000", "roulette", "shop", "money1000", "blackjack", "lucky card", "wheel of fortune", "money2000", "roulette", "shop", "money2000", "blackjack", "lucky card", "wheel of fortune", "money4000", "roulette", "shop", "money3000", "blackjack", "lucky card", "wheel of fortune", "money5000", "roulette"};

    void Start()
    {
        Debug.Log("Start() called");
        // Initialize players and waypoints only if they haven't been initialized yet
        if (!waypointsInitialized)
        {
            players = FindAllPlayers();
            InitializeWaypoints();
            waypointsInitialized = true; // Set the flag to true to prevent reinitialization
        }

        playerWaypointIndices = new int[players.Length]; // Initialize waypoint indices for all players
        Debug.Log($"Found {players.Length} players.");

        // Move all players to the starting waypoint (index 0)
        for (int i = 0; i < players.Length; i++)
        {
            MovePlayerToWaypoint(i, 0, instant: true);
        }

        // Start the first turn
        SwitchTurn();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            diceRoll = ThrowDice();
            Debug.Log($"Dice rolled: {diceRoll}");

            // Move the current player
            MovePlayerByDiceRoll(currentPlayerIndex, diceRoll);

            // End the turn
            isCurrentPlayerTurnOver = true;
        }

        if (isCurrentPlayerTurnOver)
        {
            SwitchTurn();
        }
    }

    int ThrowDice()
    {
        return Random.Range(1, 5); // Simulate a dice roll (1-4)
    }

    void SwitchTurn()
    {
        isCurrentPlayerTurnOver = false;
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        Debug.Log($"Player {currentPlayerIndex + 1}'s turn.");
    }

    void MovePlayerByDiceRoll(int playerIndex, int roll)
    {
        if (playerIndex < 0 || playerIndex >= players.Length) return;

        // Get the current waypoint index
        int currentWaypointIndex = playerWaypointIndices[playerIndex];

        // Start moving to the next waypoint one by one
        StartCoroutine(SmoothMovePlayerThroughWaypoints(playerIndex, currentWaypointIndex, roll));
    }

    IEnumerator SmoothMovePlayerThroughWaypoints(int playerIndex, int startWaypointIndex, int roll)
    {
        if (playerIndex < 0 || playerIndex >= players.Length) yield break;

        Transform playerTransform = players[playerIndex].transform;
        int currentWaypointIndex = startWaypointIndex;

        // Move one waypoint at a time based on the dice roll
        int startCheck = 1;

        for (int i = 0; i < roll; i++)
        {
            int nextWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

            // Smoothly move to the next waypoint
            yield return StartCoroutine(SmoothMoveBetweenTwoWaypoints(
                playerTransform,
                waypoints[currentWaypointIndex].position,
                waypoints[nextWaypointIndex].position
            ));
            startCheck = currentWaypointIndex;
            // Update the current waypoint index
            currentWaypointIndex = nextWaypointIndex;
            Debug.Log(startCheck+1);
            if (currentWaypointIndex < startCheck){
            Debug.Log("megyen");
        }
            
        }
        
        
        // Final position correction
        playerWaypointIndices[playerIndex] = currentWaypointIndex;

        if (tileNames[currentWaypointIndex] == "blackjack")
            {
                //Blackjack.Blackjack.ActivateBlackjack();
                Debug.Log("bj");
            }
        if (tileNames[currentWaypointIndex] == "money500")
        {
            Debug.Log("500");
        }
        if (tileNames[currentWaypointIndex] == "money1000")
        {
            Debug.Log("1000");
        }
        if (tileNames[currentWaypointIndex] == "money2000")
        {
            Debug.Log("2000");
        }
        if (tileNames[currentWaypointIndex] == "money3000")
        {
            Debug.Log("3000");
        }
        if (tileNames[currentWaypointIndex] == "money5000")
        {
            Debug.Log("5000");
        }
        if (tileNames[currentWaypointIndex] == "lucky card")
        {
        Debug.Log("lc");
        }
        if (tileNames[currentWaypointIndex] == "wheel of fortune")
        {
        Debug.Log("wof");
        }
        if (tileNames[currentWaypointIndex] == "shop")
        {
        Debug.Log("shop");
        }
        if (tileNames[currentWaypointIndex] == "start")
        {
            Debug.Log("start");
        }
        if (tileNames[currentWaypointIndex] == "roulette")
        {
        Debug.Log("roulette");
        }
    }

    IEnumerator SmoothMoveBetweenTwoWaypoints(Transform playerTransform, Vector3 start, Vector3 end)
    {
        float elapsedTime = 0f;
        float journeyLength = Vector3.Distance(start, end);

        while (elapsedTime < journeyLength / moveSpeed)
        {
            playerTransform.position = Vector3.Lerp(start, end, (elapsedTime * moveSpeed) / journeyLength);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Snap to the final position
        playerTransform.position = end;
    }

    void MovePlayerToWaypoint(int playerIndex, int waypointIndex, bool instant = false)
    {
        if (instant)
        {
            players[playerIndex].transform.position = waypoints[waypointIndex].position;
            playerWaypointIndices[playerIndex] = waypointIndex;
        }
    }

    GameObject[] FindAllPlayers()
    {
        return GameObject.FindGameObjectsWithTag("Player");
    }

    void InitializeWaypoints()
{
    // Log to check if it's being called multiple times
    Debug.Log("Initializing Waypoints...");

    waypoints = new Transform[28]; // Only create 28 waypoints

    // Starting position (use the first player's position as the reference)
    Vector3 startPosition = players[0].transform.position;

    // Current position for placing waypoints
    Vector3 currentPosition = startPosition;

    // Generate waypoints with the specified pattern

    for (int i = 0; i < waypoints.Length; i++)
    {
        GameObject waypointObject = new GameObject($"{tileNames[i]}");

        // Assign the position based on the current waypoint index
        waypointObject.transform.position = GetWaypointPosition(i, ref currentPosition);

        waypoints[i] = waypointObject.transform; // Store the waypoint in the array
    }
}
    public Transform GetPlayerCurrentWaypoint(int playerIndex)
    {
        if (playerIndex >= 0 && playerIndex < players.Length)
        {
            int waypointIndex = playerWaypointIndices[playerIndex]; // Get player's waypoint index
            return waypoints[waypointIndex]; // Return the corresponding waypoint
        }
        return null;
    }




    Vector3 GetWaypointPosition(int index, ref Vector3 currentPosition)
    {
        // Determine the movement pattern
        if (index == 0)
        {
        }
        else if (index < 8)
        {
            // First 7 waypoints: Move -2 on the X-axis
            currentPosition.x -= 2;
        }
        else if (index < 15)
        {
            // Next 7 waypoints: Move +2 on the Z-axis
            currentPosition.z += 2;
        }
        else if (index < 22)
        {
            // Next 7 waypoints: Move +2 on the X-axis
            currentPosition.x += 2;
        }
        else if (index < 28)
        {
            // Final 6 waypoints: Move -2 on the Z-axis
            currentPosition.z -= 2;
        }

        // Return the updated position
        return new Vector3(currentPosition.x, 15.75f, currentPosition.z); // Adjust Y as needed
    }
}
