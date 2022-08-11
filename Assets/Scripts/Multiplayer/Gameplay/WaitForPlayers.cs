using ExitGames.Client.Photon;
using MP.Platformer.Core;
using MP.Platformer.Mechanics;
using MP.Platformer.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitForPlayers : MonoBehaviourPunCallbacks
{

    private const string PLAYER_SPAWN_POINT = "SpawnPoint";
    private const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";

    public static WaitForPlayers Instance { get; private set; }

    public GameObject player;
    public Text CountdownText;

    #region UNITY

    public void Start()
    {
        Hashtable props = new Hashtable
            {
                {PLAYER_LOADED_LEVEL, true}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnEnable()
    {
        base.OnEnable();

        Instance = this;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        if (Instance == this) Simulation.Tick();
    }

    #endregion

    #region PUN CALLBACKS

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (changedProps.ContainsKey(PLAYER_LOADED_LEVEL))
        {
            if (CheckAllPlayerLoadedLevel())
                StartTimer();
            else
                CountdownText.text = "Waiting for other players...";
        }
    }

    public void StartTimer()
    {
        int startTimestamp;
        bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);
        if (!startTimeIsSet)
        {
            List<int> usedSpawnPoints = new List<int>();
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                var randomSpawnPoint = GetRandomSpawnPoint(usedSpawnPoints);
                usedSpawnPoints.Add(randomSpawnPoint);
                Hashtable spawnPoint = new Hashtable();
                spawnPoint.Add(PLAYER_SPAWN_POINT, randomSpawnPoint);
                p.SetCustomProperties(spawnPoint);
            }
            GetComponent<CountdownTimer>().photonView.RPC("Enable", RpcTarget.All);
            GetComponent<CountdownTimer>().SetStartTime();
        }
    }

    private static int GetRandomSpawnPoint(List<int> usedSpawnPoints)
    {
        var random = Random.Range(0, GameManager.Instance.CurrentMap.PlayerSpawnPoints.Count - 1);
        if (usedSpawnPoints.Contains(random)) return GetRandomSpawnPoint(usedSpawnPoints);
        return random;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Startup");
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    #endregion

    private bool CheckAllPlayerLoadedLevel()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(PLAYER_LOADED_LEVEL, out playerLoadedLevel))
            {
                if ((bool)playerLoadedLevel)
                {
                    continue;
                }
            }

            return false;
        }

        return true;
    }
}
