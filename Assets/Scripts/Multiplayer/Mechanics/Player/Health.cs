using MP.Platformer.Gameplay;
using MP.Platformer.UI;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using static MP.Platformer.Core.Simulation;

namespace MP.Platformer.Mechanics
{
    public class Health : MonoBehaviourPun
    {
        public int maxLives = 3;
        public int maxHP = 100;
        public HealthBar healthBar;
        public bool IsAlive => currentHP > 0;
        public bool CanRevive => currentLives > 0;
        private PlayerUIManager playerUIManager;

        public int currentHP;
        private int currentLives;
        private const string PLAYER_DATA_WINS = "WINS";

        void Start()
        {
            playerUIManager = gameObject.GetComponentInParent(typeof(PlayerUIManager)) as PlayerUIManager;
            healthBar.SetMaxHealth(maxHP);
            SetLives(maxLives);
            RefillHealth();
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.IsLocal)
                {
                    object playerCurrentWins = 0;
                    p.CustomProperties.TryGetValue(PLAYER_DATA_WINS, out playerCurrentWins);
                    Debug.Log($"Desde health las current wins son: {playerCurrentWins}");
                    playerUIManager.RunChangeWins(PhotonNetwork.NickName, (int)playerCurrentWins);
                }
            }
        }

        public void RefillHealth()
        {
            SetHealth(maxHP);
        }

        public int GetHealth()
        {
            return currentHP;
        }

        private void SetHealth(int health)
        {
            currentHP = health;
            healthBar.SetHealth(health);
        }

        private void SetLives(int lives)
        {
            currentLives = lives;
            playerUIManager.RunUpdateLives(PhotonNetwork.NickName, lives);
        }

        [PunRPC]
        void TakeDamage(int amount)
        {
            if (currentHP > 0)
            {
                SetHealth(Mathf.Clamp(currentHP - amount, 0, maxHP));

                if (currentHP == 0)
                    Die();
            }
        }

        public void Die()
        {
            currentLives--;
            if (photonView.IsMine)
            {
                SetLives(currentLives);
            }

            var death = Schedule<PlayerDeath>();
            death.playerController = GetComponent<PlayerController>();
            GetComponent<PlayerInventory>().UnequipWeapons();

            GameManager.Instance.SetPlayerLives(GetComponent<PhotonView>(), currentLives);
        }
    }
}
