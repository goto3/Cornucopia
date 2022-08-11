using ExitGames.Client.Photon;
using MP.Platformer.Core;
using MP.Platformer.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace MP.Platformer.Mechanics
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        private const string PLAYER_SPAWN_POINT = "SpawnPoint";
        private const string PLAYER_DATA_LIVES = "LIVES";
        private const string PLAYER_DATA_WINS = "WINS";
        private const string GAME_DATA_CURRENT_ROUND = "CURRENT_ROUND";

        [Header("Game configuration")]
        public int mapsToWin;

        [Header("Map configuration")]
        public List<GameObject> Maps;
        public MapController CurrentMap;

        public GameObject MapResultScreen;
        public GameObject MatchResultScreen;
        public GameObject PlayerHUD;

        [Header("Items")]
        public List<GameObject> itemsConsumibles;

        private int RoundCounter = 0;
        private bool gameStarted = false;

        public static GameManager Instance { get; private set; }

        #region UNITY

        public void Start()
        {
            CurrentMap = Maps[0].GetComponent<MapController>();
            if (PhotonNetwork.IsMasterClient)
            {
                Hashtable initialGameData = new Hashtable()
                {
                    {GAME_DATA_CURRENT_ROUND, 1}
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(initialGameData);
            }
            Hashtable newLives = new Hashtable()
                {
                    {PLAYER_DATA_LIVES, 3},
                    {PLAYER_DATA_WINS, 0}
                };
            PhotonNetwork.LocalPlayer.SetCustomProperties(newLives);
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

        public void StartGame()
        {
            gameStarted = true;
            object spawnPoint;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PLAYER_SPAWN_POINT, out spawnPoint);
            var spawnPoints = CurrentMap.PlayerSpawnPoints;

            string spriteName = (string)PhotonNetwork.LocalPlayer.CustomProperties["characterSpriteName"];            
            PhotonNetwork.Instantiate(spriteName, spawnPoints[(int)spawnPoint].transform.position, Quaternion.identity);
        }

        public GameObject GetRandomSpawnPoint()
        {
            return CurrentMap.PlayerSpawnPoints[Random.Range(0, CurrentMap.PlayerSpawnPoints.Count)];
        }

        internal void SetPlayerLives(PhotonView photonView, int currentLives)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Hashtable newLives = new Hashtable()
                {
                    {PLAYER_DATA_LIVES, currentLives}
                };
                photonView.Controller.SetCustomProperties(newLives);
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey(PLAYER_DATA_LIVES) && gameStarted)
            {
                object newlives;
                changedProps.TryGetValue(PLAYER_DATA_LIVES, out newlives);
                CheckWinCondition();
            }
        }

        private void CheckWinCondition()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            List<Player> alivePlayers = new List<Player>();
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object lives;
                p.CustomProperties.TryGetValue(PLAYER_DATA_LIVES, out lives);
                if ((int)lives > 0) alivePlayers.Add(p);
            }
            if (alivePlayers.Count == 1 && PhotonNetwork.PlayerList.Length > 1)
            {
                gameStarted = false;
                object playerCurrentWins;
                alivePlayers[0].CustomProperties.TryGetValue(PLAYER_DATA_WINS, out playerCurrentWins);
                
                Hashtable newWins = new Hashtable()
                    {
                        {PLAYER_DATA_WINS, (int)playerCurrentWins + 1}
                    };
                alivePlayers[0].SetCustomProperties(newWins);
                
                if ((int)playerCurrentWins + 1 == mapsToWin)
                {
                    photonView.RPC("SwitchToMatchResult", RpcTarget.All, alivePlayers[0].NickName);
                }
                else
                {
                    photonView.RPC("SwitchToMapResult", RpcTarget.All, alivePlayers[0].NickName);
                }
            }
        }

        [PunRPC]
        private void SwitchToMatchResult(string nickname)
        {
            CurrentMap.gameObject.SetActive(false);
            PlayerHUD.SetActive(false);

            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p in players)
            {
                p.SetActive(false);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.DestroyAll();
            }

            MatchResultScreen.GetComponent<MatchResultController>().Show();
            MatchResultScreen.GetComponent<MatchResultController>().SetWinnerName(nickname);
        }

        [PunRPC]
        private void SwitchToMapResult(string nickname)
        {
            CurrentMap.gameObject.SetActive(false);
            PlayerHUD.SetActive(false);

            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p in players)
            {
                p.SetActive(false);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.DestroyAll();
            }
           
            
            MapResultScreen.GetComponent<MapResultController>().Show();
            MapResultScreen.GetComponent<MapResultController>().SetWinnerName(nickname);
        }

        public void LoadNextMap()
        {
            RoundCounter++;
            var nextMapIndex = RoundCounter % Maps.Count;
            CurrentMap = Maps[nextMapIndex].GetComponent<MapController>();
            PlayerHUD.SetActive(true);
            CurrentMap.gameObject.SetActive(true);

            //Start timeout
            if (PhotonNetwork.IsMasterClient)
            {
                GetComponent<WaitForPlayers>().StartTimer();
            }

        }

        public void SpawnRandomItem(Vector2 pos)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            var itemToSpawn = itemsConsumibles[Random.Range(0, itemsConsumibles.Count)];
            var spriteName = itemToSpawn.name;
            PhotonNetwork.Instantiate(spriteName, pos, Quaternion.identity);
        }
    }

}