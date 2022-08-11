using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;

public class MenuMP : MonoBehaviourPunCallbacks
{
    public TMP_Text roomID;
    public TMP_Text playerName;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void CreateRoom()
    {
        var playername = String.IsNullOrEmpty(playerName.text) ? "Player" : playerName.text;
        PhotonNetwork.NickName = playername;

        var roomOptions = new RoomOptions() { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(roomID.text, roomOptions);
    }

    public void JoinRoom()
    {
        var playername = String.IsNullOrEmpty(playerName.text) ? "Player" : playerName.text;
        PhotonNetwork.NickName = playername;

        var result = PhotonNetwork.JoinRoom(roomID.text);

        Debug.Log(result);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions());
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameMP");
    }

}
