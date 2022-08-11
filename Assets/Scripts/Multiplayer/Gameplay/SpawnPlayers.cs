using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject player;
    public Transform[] spawnPoints;

    private void Start()
    {
        var numberInRoom = 0;
        var allPlayers = PhotonNetwork.PlayerList;
        foreach (Player p in allPlayers)
            if (p != PhotonNetwork.LocalPlayer)
                numberInRoom++;

        PhotonNetwork.Instantiate(player.name, spawnPoints[numberInRoom].position, Quaternion.identity);
    }
}
