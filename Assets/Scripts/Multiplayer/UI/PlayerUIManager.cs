using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace MP.Platformer.UI
{
    public class PlayerUIManager : MonoBehaviourPun
    {
        // Improvement -> Wrap in specific object, instead of having all the elements scattered
        public Sprite[] livesSpritesPlayer1;
        private Image livesDisplayImagePlayer1;
        private Image avatarImagePlayer1;
        private Text textNickNamePlayer1;
        private Text textplayer1Wins;

        public Sprite[] livesSpritesPlayer2;
        private Image livesDisplayImagePlayer2;
        private Image avatarImagePlayer2;
        private Text textNickNamePlayer2;
        private Text textplayer2Wins;

        public Sprite[] livesSpritesPlayer3;
        private Image livesDisplayImagePlayer3;
        private Image avatarImagePlayer3;
        private Text textNickNamePlayer3;
        private Text textplayer3Wins;

        public Sprite[] livesSpritesPlayer4;
        private Image livesDisplayImagePlayer4;
        private Image avatarImagePlayer4;
        private Text textNickNamePlayer4;
        private Text textplayer4Wins;

        public Sprite suhiAvatar;
        public Sprite panchoAvatar;
        public Sprite matingoAvatar;
        public Sprite bjronAvatar;

        private Dictionary<string, int> characterUIs;
        private const string PLAYER_DATA_LIVES = "LIVES";

        public void RunAddPlayer(string nickname, string chosenCharacter)
        {
            Debug.Log("Agregar a un usuario");
            photonView.RPC("AddPlayer", RpcTarget.All, nickname, chosenCharacter);
        }

        [PunRPC]
        public void AddPlayer(string nickName, string chosenCharacter)
        {
            textNickNamePlayer1 = GameObject.Find("textNickNamePlayer1").GetComponent<Text>();
            textNickNamePlayer2 = GameObject.Find("textNickNamePlayer2").GetComponent<Text>();
            textNickNamePlayer3 = GameObject.Find("textNickNamePlayer3").GetComponent<Text>();
            textNickNamePlayer4 = GameObject.Find("textNickNamePlayer4").GetComponent<Text>();
            
            textplayer1Wins = GameObject.Find("textplayer1Wins").GetComponent<Text>();
            textplayer2Wins = GameObject.Find("textplayer2Wins").GetComponent<Text>();
            textplayer3Wins = GameObject.Find("textplayer3Wins").GetComponent<Text>();
            textplayer4Wins = GameObject.Find("textplayer4Wins").GetComponent<Text>();

            avatarImagePlayer1 = GameObject.Find("avatarImagePlayer1").GetComponent<Image>();
            avatarImagePlayer2 = GameObject.Find("avatarImagePlayer2").GetComponent<Image>();
            avatarImagePlayer3 = GameObject.Find("avatarImagePlayer3").GetComponent<Image>();
            avatarImagePlayer4 = GameObject.Find("avatarImagePlayer4").GetComponent<Image>();

            livesDisplayImagePlayer1 = GameObject.Find("livesDisplayImagePlayer1").GetComponent<Image>();
            livesDisplayImagePlayer2 = GameObject.Find("livesDisplayImagePlayer2").GetComponent<Image>();
            livesDisplayImagePlayer3 = GameObject.Find("livesDisplayImagePlayer3").GetComponent<Image>();
            livesDisplayImagePlayer4 = GameObject.Find("livesDisplayImagePlayer4").GetComponent<Image>();


            int i = 1;
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object spriteName;
                p.CustomProperties.TryGetValue("characterSpriteName", out spriteName);

                switch (i)
                {
                    case 1:
                        livesDisplayImagePlayer1.enabled = true;
                        avatarImagePlayer1.enabled = true;
                        textNickNamePlayer1.enabled = true;
                        textplayer1Wins.enabled = true;
                        textNickNamePlayer1.text = p.NickName;
                        switch (spriteName)
                        {
                            case "Uwu Makki":
                                avatarImagePlayer1.sprite = suhiAvatar;
                                break;
                            case "Big Frank":
                                avatarImagePlayer1.sprite = panchoAvatar;
                                break;
                            case "El Matingo":
                                avatarImagePlayer1.sprite = matingoAvatar;
                                break;
                            case "Bjornströmming":
                                avatarImagePlayer1.sprite = bjronAvatar;
                                break;
                        }
                        break;
                    case 2:
                        livesDisplayImagePlayer2.enabled = true;
                        avatarImagePlayer2.enabled = true;
                        textNickNamePlayer2.enabled = true;
                        textplayer2Wins.enabled = true;
                        textNickNamePlayer2.text = p.NickName;
                        switch (spriteName)
                        {
                            case "Uwu Makki":
                                avatarImagePlayer2.sprite = suhiAvatar;
                                break;
                            case "Big Frank":
                                avatarImagePlayer2.sprite = panchoAvatar;
                                break;
                            case "El Matingo":
                                avatarImagePlayer2.sprite = matingoAvatar;
                                break;
                            case "Bjornströmming":
                                avatarImagePlayer2.sprite = bjronAvatar;
                                break;
                        }
                        break;
                    case 3:
                        livesDisplayImagePlayer3.enabled = true;
                        avatarImagePlayer3.enabled = true;
                        textNickNamePlayer3.enabled = true;
                        textplayer3Wins.enabled = true;
                        textNickNamePlayer3.text = p.NickName;
                        switch (spriteName)
                        {
                            case "Uwu Makki":
                                avatarImagePlayer3.sprite = suhiAvatar;
                                break;
                            case "Big Frank":
                                avatarImagePlayer3.sprite = panchoAvatar;
                                break;
                            case "El Matingo":
                                avatarImagePlayer3.sprite = matingoAvatar;
                                break;
                            case "Bjornströmming":
                                avatarImagePlayer3.sprite = bjronAvatar;
                                break;
                        }
                        break;
                    case 4:
                        livesDisplayImagePlayer4.enabled = true;
                        avatarImagePlayer4.enabled = true;
                        textNickNamePlayer4.enabled = true;
                        textplayer4Wins.enabled = true;
                        textNickNamePlayer4.text = p.NickName;
                        switch (spriteName)
                        {
                            case "Uwu Makki":
                                avatarImagePlayer4.sprite = suhiAvatar;
                                break;
                            case "Big Frank":
                                avatarImagePlayer4.sprite = panchoAvatar;
                                break;
                            case "El Matingo":
                                avatarImagePlayer4.sprite = matingoAvatar;
                                break;
                            case "Bjornströmming":
                                avatarImagePlayer4.sprite = bjronAvatar;
                                break;
                        }
                        break;
                }

                i++;
            }
        }

        public void RunUpdateLives(string nickname, int currentLives)
        {
            Debug.Log("Actualizar las vidas");
            photonView.RPC("UpdateLives", RpcTarget.All, nickname, currentLives);
        }

        [PunRPC]
        public void UpdateLives(string nickname, int currentLives)
        {            
            int i = 1;
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (nickname == p.NickName)
                {
                    switch (i)
                    {
                        case 1:
                            livesDisplayImagePlayer1.sprite = livesSpritesPlayer1[currentLives];
                            break;
                        case 2:
                            livesDisplayImagePlayer2.sprite = livesSpritesPlayer2[currentLives];
                            break;
                        case 3:
                            livesDisplayImagePlayer3.sprite = livesSpritesPlayer3[currentLives];
                            break;
                        case 4:
                            livesDisplayImagePlayer4.sprite = livesSpritesPlayer4[currentLives];
                            break;
                    }
                }

                i++;
            }
        }

        public void RunChangeWins(string nickname, int wins)
        {
            Debug.Log("Actualizar las wins");
            photonView.RPC("ChangeWins", RpcTarget.All, nickname, wins);
        }
        
        [PunRPC]
        public void ChangeWins(string nickname, int wins)
        {
            int playerNumber = -1;

            int i = 1;
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (nickname == p.NickName)
                {
                    switch (i)
                    {
                        case 1:
                            textplayer1Wins.text = $"Wins {wins.ToString()}";
                            break;
                        case 2:
                            textplayer2Wins.text = $"Wins {wins.ToString()}";
                            break;
                        case 3:
                            textplayer3Wins.text = $"Wins {wins.ToString()}";
                            break;
                        case 4:
                            textplayer4Wins.text = $"Wins {wins.ToString()}";
                            break;
                    }
                }

                i++;
            }
        }
    }
}
