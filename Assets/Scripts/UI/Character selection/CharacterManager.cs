using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CharacterManager : MonoBehaviour
{
    private const string PLAYER_AVATAR = "avatar";
    private const string PLAYER_SPRITE_NAME = "characterSpriteName";

    public CharacterDatabase characterDB;
    public Text nameText;
    public Image characterImage;
    private int selectedOption = 0;

    void Start()
    {
        if (!PlayerPrefs.HasKey("selectedCharacter"))
        {
            selectedOption = 0;
            Save();
        }
        else
        {
            Load();
        }
        UpdateSelection();
    }

    private void OnEnable()
    {
        UpdateSelection();
    }

    public void NextOption()
    {
        selectedOption++;
        if (selectedOption >= characterDB.CharacterCount)
        {
            selectedOption = 0;
        }

        UpdateSelection();
        Save();
    }

    public void BackOption()
    {
        selectedOption--;
        if (selectedOption < 0)
        {
            selectedOption = characterDB.CharacterCount - 1;
        }

        UpdateSelection();
        Save();
    }

    private void UpdateSelection()
    {
        Character character = characterDB.GetCharacter(selectedOption);
        characterImage.sprite = character.characterSprite;
        nameText.text = character.characterName;

        Hashtable initialProps = new Hashtable() { { PLAYER_AVATAR, selectedOption }, { PLAYER_SPRITE_NAME, character.characterName } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
    }

    private void Load()
    {
        selectedOption = PlayerPrefs.GetInt("selectedCharacter");
    }

    private void Save()
    {
        PlayerPrefs.SetInt("selectedCharacter", selectedOption);
    }
}
