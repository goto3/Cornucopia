using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceKnife : MonoBehaviour
{
    public Transform playerPosition; 
    public GameObject knifePrefab;
    public Animator playerSpriteAnimator;
    

    void Start()
    {
        /*EN EL MOMENTO QUE SE COLISIONA CON EL ARMA MODIFICAR ANIMACION DE PERSONAJE A SIN BRAZOS*/
        playerSpriteAnimator = GameObject.FindGameObjectWithTag("TutorialRequirement").GetComponent<Animator>();
        
    }

    void Update()
    {        

        if(Input.GetKeyDown("k"))
        {
            InstanceKnifeMelee();
        }

    }

    void InstanceKnifeMelee()
    {
        GameObject Knife = Instantiate(knifePrefab, playerPosition.position, Quaternion.identity);
        //Knife.transform.SetParent(playerPosition.transform);
        playerSpriteAnimator.SetLayerWeight(1,1);
    }
}
