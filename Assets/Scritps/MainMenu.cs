using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject findOpponentPanel = null;
    [SerializeField] private GameObject waitingStatusPanel = null;
    [SerializeField] private TextMeshProUGUI waitingStatusText = null;

    private bool isConnecting = false;

    private const string GameVersion = "0.1";
    private const int MaxPlayerPerRoom = 2;

    private void Awake() => PhotonNetwork.AutomaticallySyncScene = true;

    public void FindOpponent()
    {
        isConnecting = true;

        findOpponentPanel.SetActive(false);
        waitingStatusPanel.SetActive(true);

        waitingStatusText.text = "Searching...";

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conneted to Master");

        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        findOpponentPanel.SetActive(true);
        waitingStatusPanel.SetActive(false);

        Debug.Log($"Disconneted due to: {cause}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No cliens are waitting for an opponent, creating a new room");

        PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = MaxPlayerPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Client successfully joined a room");

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if(playerCount != MaxPlayerPerRoom)
        {
            waitingStatusText.text = "Waiting For Opponent";
            Debug.Log("Client is waiting for an opponent");
        }
        else
        {
            waitingStatusText.text = "Opponent Found";
            Debug.Log("Mach is ready to begin");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayerPerRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;

            waitingStatusText.text = "Opponent Found";
            Debug.Log("Mach is ready to begin");

            PhotonNetwork.LoadLevel("Scene_Main");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
