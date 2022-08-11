using System;
using Platformer.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Represebts the current vital statistics of some game entity.
    /// </summary>
    public class Health : MonoBehaviour
    {
        public int maxLives = 3;
        public int maxHP = 1;
        public HealthBar healthBar;
        public bool IsAlive => currentHP > 0;
        public bool CanRevive => currentLives > 0;
        private UIManager uiManager;
        
        int currentHP;
        private int currentLives;

        private void Start()
        {
            uiManager = GameObject.Find("UIPlayer").GetComponent<UIManager>();
            healthBar.SetMaxHealth(maxHP);            
            SetLives(maxLives);
            RefillHealth();
        }
        
        public void RefillHealth()
        {
            SetHealth(maxHP);
        }

        public void Decrement(int amount)
        {
            SetHealth(Mathf.Clamp(currentHP - amount, 0, maxHP));
            
            if (currentHP == 0)
                Die();
        }

        public void Die()
        {
            SetLives(currentLives - 1);
            Schedule<PlayerDeath>();
        }

        private void SetHealth(int health)
        {
            currentHP = health;
            healthBar.SetHealth(health);
        }

        private void SetLives(int lives)
        {
            currentLives = lives;
            uiManager.UpdateLives(lives);
        }
    }
}
