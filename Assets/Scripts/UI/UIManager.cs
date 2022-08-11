using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Sprite[] livesSprites;
    public Image livesDisplayImage;

    public void UpdateLives(int currentLives)
    {
        livesDisplayImage.sprite = livesSprites[currentLives];
    }
}
