using Photon.Pun;
using UnityEngine;
using TMPro;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField joinCodeInput;
    public TextMeshProUGUI generatedCodeText;

    private string roomCode;

    public void CreateRoom()
    {
        roomCode = Random.Range(100000, 999999).ToString();
        PhotonNetwork.CreateRoom(roomCode);
        generatedCodeText.text = "Room Code: " + roomCode;
        Debug.Log("Room created with code: " + roomCode);
    }

    public void JoinRoom()
    {
        string code = joinCodeInput.text;
        PhotonNetwork.JoinRoom(code);
        if(!PhotonNetwork.JoinRoom(code))
        {
            Debug.Log("Wrong code");
        }
        else{
            Debug.Log("Connection successful");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        //PhotonNetwork.LoadLevel("GameScene"); // Betölti a játékmenetet
    }
}
