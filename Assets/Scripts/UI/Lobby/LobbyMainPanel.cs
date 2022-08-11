using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MP.Platformer.UI.Lobby
{
    public class LobbyMainPanel : MonoBehaviourPunCallbacks
    {
        private const string PLAYER_AVATAR = "avatar";
        public const string PLAYER_READY = "IsPlayerReady";
        public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";

        public float startDelay;

        [Header("Home")]
        public GameObject HomePanel;

        [Header("Selection Panel")]
        public GameObject SelectionPanel;
        public InputField PlayerNameInput;

        [Header("Create Room Panel")]
        public GameObject CreateRoomPanel;
        public InputField RoomNameInputField;
        public InputField MaxPlayersInputField;

        [Header("Join Random Room Panel")]
        public GameObject JoinRandomRoomPanel;

        [Header("Room List Panel")]
        public GameObject RoomListPanel;

        public GameObject RoomListContent;
        public GameObject RoomListEntryPrefab;

        [Header("Room Panel")]
        public GameObject RoomPanel;
        public GameObject RoomPlayerList;
        public GameObject CharacterSelectionPanel;

        public Button StartGameButton;
        public GameObject PlayerListEntryPrefab;

        private float StartTimer = 0;
        private bool DelayConcluded = false;

        private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, GameObject> roomListEntries;
        private Dictionary<int, GameObject> playerListEntries = new Dictionary<int, GameObject>();

        private PageController PageController;

        #region UNITY

        public void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();

            PageController = GetComponent<PageController>();

            cachedRoomList = new Dictionary<string, RoomInfo>();
            roomListEntries = new Dictionary<string, GameObject>();

            RoomNameInputField.placeholder.GetComponent<Text>().text = "Sala " + Random.Range(1, 1000);
            var randomName = "Jugador " + Random.Range(1, 1000);
            PlayerNameInput.placeholder.GetComponent<Text>().text = randomName;
            PhotonNetwork.NickName = randomName;
        }

        public void Update()
        {
            if (!DelayConcluded)
            {
                StartTimer += Time.deltaTime;
                if (StartTimer > startDelay)
                {
                    DelayConcluded = true;
                    this.GoToPage(1);
                }
            }
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnConnectedToMaster()
        {
            //this.SetActivePanel(HomePanel.name);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRoomListView();

            UpdateCachedRoomList(roomList);
            UpdateRoomListView();
        }

        public override void OnJoinedLobby()
        {
            // whenever this joins a new lobby, clear any previous room lists
            cachedRoomList.Clear();
            ClearRoomListView();
        }

        // note: when a client joins / creates a room, OnLeftLobby does not get called, even if the client was in a lobby before
        public override void OnLeftLobby()
        {
            cachedRoomList.Clear();
            ClearRoomListView();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            this.GoToPage(2);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            this.GoToPage(2);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            /*string roomName = "Room " + Random.Range(1000, 10000);
            RoomOptions options = new RoomOptions { MaxPlayers = 8 };
            PhotonNetwork.CreateRoom(roomName, options, null);*/
        }

        public override void OnJoinedRoom()
        {
            // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
            cachedRoomList.Clear();
            this.GoToPage(4);
            if (playerListEntries == null) playerListEntries = new Dictionary<int, GameObject>();

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                GameObject entry = Instantiate(PlayerListEntryPrefab);
                entry.transform.SetParent(RoomPlayerList.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                }

                object playerAvatar;
                if (p.CustomProperties.TryGetValue(PLAYER_AVATAR, out playerAvatar))
                {
                    entry.GetComponent<PlayerListEntry>().UpdateAvatar((int)playerAvatar);
                }

                playerListEntries.Add(p.ActorNumber, entry);
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());

            Hashtable props = new Hashtable
            {
                {PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public override void OnLeftRoom()
        {
            this.GoToPage(2);

            foreach (GameObject entry in playerListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            playerListEntries.Clear();
            playerListEntries = null;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(RoomPlayerList.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

            playerListEntries.Add(newPlayer.ActorNumber, entry);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
            playerListEntries.Remove(otherPlayer.ActorNumber);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                StartGameButton.gameObject.SetActive(CheckPlayersReady());
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (playerListEntries == null) playerListEntries = new Dictionary<int, GameObject>();

            GameObject entry;
            if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                object isPlayerReady;
                if (changedProps.TryGetValue(PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                }
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        #endregion

        #region UI CALLBACKS

        public void OnNicknameChange()
        {
            PhotonNetwork.NickName = PlayerNameInput.text;
        }

        public void OnCreateRoomButtonClicked()
        {
            string roomName = RoomNameInputField.text;
            roomName = (roomName.Equals(string.Empty)) ? RoomNameInputField.placeholder.GetComponent<Text>().text : roomName;

            byte maxPlayers;
            byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
            maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 4);

            RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };
            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        public void OnJoinRandomRoomButtonClicked()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public void OnLeaveRoomButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
            this.GoToPage(2);
        }

        public void OnStartGameButtonClicked()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            PhotonNetwork.LoadLevel("GameMP");
        }

        #region UI TRANSITIONS

        public void OnQuitButtonClicked()
        {
            Application.Quit();
        }

        #endregion

        #endregion

        private bool CheckPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient) return false;

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(PLAYER_READY, out isPlayerReady))
                {
                    if (!(bool)isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void ClearRoomListView()
        {
            foreach (GameObject entry in roomListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            roomListEntries.Clear();
        }

        public void LocalPlayerPropertiesUpdated()
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public void GoToPage(int page)
        {
            PageController.GoToPage(page);
            switch (page)
            {
                case 1:
                    if (PhotonNetwork.InLobby)
                        PhotonNetwork.LeaveLobby();
                    HomePanel.SetActive(true);
                    SelectionPanel.SetActive(false);
                    CreateRoomPanel.SetActive(false);
                    JoinRandomRoomPanel.SetActive(false);
                    RoomListPanel.SetActive(false);
                    CharacterSelectionPanel.SetActive(false);
                    RoomPanel.SetActive(false);
                    break;
                case 2:
                    if (!PhotonNetwork.InLobby)
                        PhotonNetwork.JoinLobby();
                    HomePanel.SetActive(false);
                    SelectionPanel.SetActive(true);
                    CreateRoomPanel.SetActive(false);
                    JoinRandomRoomPanel.SetActive(false);
                    RoomListPanel.SetActive(true);
                    CharacterSelectionPanel.SetActive(false);
                    RoomPanel.SetActive(false);
                    break;
                case 3:
                    if (!PhotonNetwork.InLobby)
                        PhotonNetwork.JoinLobby();
                    HomePanel.SetActive(false);
                    SelectionPanel.SetActive(false);
                    CreateRoomPanel.SetActive(true);
                    JoinRandomRoomPanel.SetActive(false);
                    RoomListPanel.SetActive(true);
                    CharacterSelectionPanel.SetActive(false);
                    RoomPanel.SetActive(false);
                    break;
                case 4:
                    if (PhotonNetwork.InLobby)
                        PhotonNetwork.LeaveLobby();
                    HomePanel.SetActive(false);
                    SelectionPanel.SetActive(false);
                    CreateRoomPanel.SetActive(false);
                    JoinRandomRoomPanel.SetActive(false);
                    RoomListPanel.SetActive(false);
                    CharacterSelectionPanel.SetActive(true);
                    RoomPanel.SetActive(true);
                    break;
            }
        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }

        private void UpdateRoomListView()
        {
            foreach (RoomInfo info in cachedRoomList.Values)
            {
                GameObject entry = Instantiate(RoomListEntryPrefab);
                entry.transform.SetParent(RoomListContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

                roomListEntries.Add(info.Name, entry);
            }
        }
    }
}