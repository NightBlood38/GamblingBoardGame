using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField joinCodeInput;
    public TextMeshProUGUI generatedCodeText, multiplayerButtonText;
    public StartMenuController startMenuController;
    public Button multiplayerButton;

    private string roomCode;


    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    public void CreateRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log($"PhotonNetwork.IsConnected: {PhotonNetwork.IsConnected}");
            Debug.Log($"PhotonNetwork.InLobby: {PhotonNetwork.InLobby}");
            Debug.Log($"PhotonNetwork.InRoom: {PhotonNetwork.InRoom}");

            roomCode = Random.Range(100000, 999999).ToString();
            PhotonNetwork.CreateRoom(roomCode);
            Debug.Log("Room created with code: " + roomCode);
            startMenuController.UpdatePlayerList();
            
        }
        else
        {
            Debug.LogError("Not connected to Photon server. Cannot create room.");
        }
    }

    public void JoinRoom()
    {
        string code = joinCodeInput.text;
        
        JoinRoom(code);
    }
    public void JoinRoom(string code)
    {        
        
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.InLobby)
            {
                generatedCodeText.text = "Room Code: " + roomCode;
                Debug.Log("Attempting to join room: " + code);
                PhotonNetwork.JoinRoom(code);
            }
            else
            {
                Debug.LogError("Client is not in the lobby yet. Cannot join room.");
            }
        }
        else
        {
            Debug.LogError("Not connected to Photon server. Cannot join room.");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " has joined the room.");
        startMenuController.UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " has left the room.");
        startMenuController.UpdatePlayerList();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined room: " + PhotonNetwork.CurrentRoom.Name);
        startMenuController.SetPlayerName(); // sets the player's name
        startMenuController.OnSuccessfulJoin();
    }
    public override void OnConnected()
    {
        Debug.Log("Connected to Photon Server");
    }
    public override void OnJoinedLobby()
    {
        multiplayerButtonText.text = "Multiplayer";
        multiplayerButton.interactable = true;
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        startMenuController.OnJoinFailed();
    }
}
