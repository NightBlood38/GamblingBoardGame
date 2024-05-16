using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float moveDistance = 2.0f;
    private float xMin = -7.0f;
    private float xMax = 7.0f;
    private float zMin = -10.0f;
    private float zMax = 4.0f;

    private int moveCount = 0;
    private int currentPlayerNumber = 0;
    private GameObject[] players;

    private int currentPlayerIndex = 0;
    private bool isCurrentPlayerTurnOver;
    public Transform[] waypoints;
    
    void Start()
    {
        Awake();
        MovePlayerToWaypoint(currentPlayerIndex);
        players = FindAllPlayers();
        SwitchTurn();
    }
    void Update()
    {
        
        
        if (isCurrentPlayerTurnOver)
        {
            SwitchTurn();
            
        }
        Vector3 movement = Vector3.zero;

        for(int i = 0; i < waypoints.Length; i++){
            MovePlayerToWaypoint(i);
        }

        if(Input.GetKeyDown(KeyCode.H)){
            currentPlayerNumber = ThrowDice();
            Debug.Log(currentPlayerNumber);
        }
        
    }

    int ThrowDice(){
        int num = Random.Range(1,9);
        isCurrentPlayerTurnOver = true;
        return num;
    }
    void SwitchTurn()
    {
        isCurrentPlayerTurnOver = false;

    // Switch to the next player's turn
        currentPlayerIndex = (currentPlayerIndex + 1) % waypoints.Length;

        // Check if currentPlayerIndex is within bounds
        if (currentPlayerIndex < waypoints.Length)
        {
            MovePlayerToWaypoint(currentPlayerIndex);
            Debug.Log("Player " + (currentPlayerIndex + 1) + "'s turn");
        }
        else
        {
            Debug.LogError("Error: currentPlayerIndex out of bounds");
        }
    }
    GameObject[] FindAllPlayers()
    {
        //Összes player megtalálása a player taggel
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(players.Length);

        // DEBUG
        foreach (GameObject player in players)
        {
            Debug.Log("Found Player: " + player.name);
        }
        return players;
    }
    void MovePlayerToWaypoint(int index)
    {
        // Ensure the index is within the bounds of the waypoints array
        if (index >= 0 && index < waypoints.Length)
        {
            // Move the current player to the specified waypoint position
            Transform playerTransform = GetCurrentPlayerTransform();
            playerTransform.position = waypoints[index].position;
            // Update the currentPlayerIndex to reflect the new position
            currentPlayerIndex = index;
        }
    }

    Transform GetCurrentPlayerTransform()
    {
        // Example: You might have a separate script or component that manages players
        // In this example, we'll just find the player GameObject by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        return player != null ? player.transform : null;
    }
    void Awake()
    {
        waypoints = new Transform[28];

    // Create GameObjects for each waypoint
        for (int i = 0; i < waypoints.Length; i++)
        {
            GameObject waypointObject = new GameObject("Waypoint" + (i + 1)); // Create a new empty GameObject
            waypointObject.transform.position = GetWaypointPosition(i); // Set its position based on the index
            waypoints[i] = waypointObject.transform; // Assign its transform to the waypoints array
        }          
                
    }

    Vector3 GetWaypointPosition(int index)
    {
        float x = (index % 7) * 2 - 7; // X position alternates between -7 and 7
        float z = -10 + (index / 14) * 2; // Z position increases by 2 every 14 waypoints
        return new Vector3(x, 15.52f, z);
    }
}

