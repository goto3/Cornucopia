using UnityEngine;
using UnityEngine.UI;

namespace MP.Platformer.UI
{
    public class UIManager : MonoBehaviour
    {
        public Sprite[] livesSprites;
        public Image livesDisplayImage;

        public void UpdateLives(int currentLives)
        {
            livesDisplayImage.sprite = livesSprites[currentLives];
        }
    }
}

