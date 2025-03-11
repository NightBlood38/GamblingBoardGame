using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private int diceRoll = 0;
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private bool isTurn = false;
    private float moveSpeed = 3.0f;
    private PlayerData playerData;
    public ShopManager shopManager;
    
    public string[] tileNames = {
        "start", "money500", "blackjack", "lucky card", "wheel of fortune", 
        "money1000", "roulette", "shop", "money1000", "blackjack", 
        "lucky card", "wheel of fortune", "money2000", "roulette", "shop", 
        "money2000", "blackjack", "lucky card", "wheel of fortune", 
        "money4000", "roulette", "shop", "money3000", "blackjack", 
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

        for (int i = 0; i < roll; i++)
        {
            int nextWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

            yield return StartCoroutine(SmoothMoveBetweenTwoWaypoints(
                playerTransform,
                waypoints[currentWaypointIndex].position,
                waypoints[nextWaypointIndex].position
            ));

            currentWaypointIndex = nextWaypointIndex;

            if (currentWaypointIndex < startWaypointIndex)
            {
                playerData.money += 2000;
                Debug.Log($"{gameObject.name} kapott +2000 pénzt! Jelenlegi pénz: {playerData.money}");
            }
        }

        HandleTileEffects();
        GameManager.Instance.EndTurn();
    }

    void HandleTileEffects()
    {
        string tile = tileNames[currentWaypointIndex];
        
        switch (tile)
        {
            case "money500": playerData.money += 500; Debug.Log($"{gameObject.name} kapott +500 pénzt! Jelenlegi pénz: {playerData.money}"); break;
            case "money1000": playerData.money += 1000; Debug.Log($"{gameObject.name} kapott +1000 pénzt! Jelenlegi pénz: {playerData.money}"); break;
            case "money2000": playerData.money += 2000; Debug.Log($"{gameObject.name} kapott +2000 pénzt! Jelenlegi pénz: {playerData.money}"); break;
            case "money3000": playerData.money += 3000; Debug.Log($"{gameObject.name} kapott +3000 pénzt! Jelenlegi pénz: {playerData.money}"); break;
            case "money5000": playerData.money += 5000; Debug.Log($"{gameObject.name} kapott +5000 pénzt! Jelenlegi pénz: {playerData.money}"); break;
            case "start": playerData.money += 2000; Debug.Log($"{gameObject.name} kapott +2000 pénzt! Jelenlegi pénz: {playerData.money}"); break;
            case "blackjack": Debug.Log($"{gameObject.name} blackjackra lépett!"); break;
            case "shop": GameManager.isShopOpen = true; shopManager.OpenShop(); break;
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
        if (waypoints.Length > 0 && waypoints[0] != null)
        Debug.Log($"Waypoints inicializálva, első waypoint: {waypoints[0].position}");
    else
        Debug.LogError("HIBA: waypoints tömb üres vagy nem inicializálódott megfelelően!");
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
