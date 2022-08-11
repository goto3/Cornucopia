using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace MP.Platformer.UI.Lobby
{
    public class PlayerListEntry : MonoBehaviourPunCallbacks
    {
        public CharacterDatabase characterDB;

        private const string PLAYER_AVATAR = "avatar";
        private const string PLAYER_READY = "IsPlayerReady";
        public const string PLAYER_LIVES = "PlayerLives";
        public const int PLAYER_MAX_LIVES = 3;

        [Header("UI References")]
        public Text PlayerNameText;
        public Image PlayerAvatarImage;
        public Button PlayerReadyButton;
        public Sprite PlayerReadyImageSprite;
        public Sprite PlayerNotReadyImageSprite;

        private int ownerId;
        private bool isPlayerReady;

        public void Start()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == ownerId)
            {
                Hashtable initialProps = new Hashtable() { { PLAYER_READY, isPlayerReady }, { PLAYER_LIVES, PLAYER_MAX_LIVES } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
                PhotonNetwork.LocalPlayer.SetScore(0);

                PlayerReadyButton.onClick.AddListener(() =>
                {
                    isPlayerReady = !isPlayerReady;
                    SetPlayerReady(isPlayerReady);

                    Hashtable props = new Hashtable() { { PLAYER_READY, isPlayerReady } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                    if (PhotonNetwork.IsMasterClient)
                    {
                        FindObjectOfType<LobbyMainPanel>().LocalPlayerPropertiesUpdated();
                    }
                });
            }
        }

        public void Initialize(int playerId, string playerName)
        {
            ownerId = playerId;
            PlayerNameText.text = playerName;
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey(PLAYER_AVATAR))
            {
                object avatarIndex;
                changedProps.TryGetValue(PLAYER_AVATAR, out avatarIndex);

                if (targetPlayer.ActorNumber == ownerId && avatarIndex != null)
                {
                    UpdateAvatar((int)avatarIndex);
                }
            }
        }

        public void UpdateAvatar(int index)
        {
            var sprite = characterDB.GetCharacter(index).characterSprite;
            PlayerAvatarImage.sprite = sprite;
        }

        public void SetPlayerReady(bool playerReady)
        {
            if (playerReady)
                PlayerReadyButton.image.sprite = PlayerReadyImageSprite;
            else
                PlayerReadyButton.image.sprite = PlayerNotReadyImageSprite;
        }

    }
}