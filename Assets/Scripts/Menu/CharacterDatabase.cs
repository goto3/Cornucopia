using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class CharacterDatabase : ScriptableObject
{
    public Character[] characters;

    public int CharacterCount
    {
        get {
            return characters.Length;
        
        }
    }

    public Character GetCharacter(int index) {
        return characters[index];    
    }
   
}
