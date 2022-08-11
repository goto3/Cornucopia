using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceWeapon : MonoBehaviour
{
    public Transform playerPosition; 
    public GameObject weaponPrefab;
    public Animator playerSpriteAnimator;

    void Start()
    {
        /*EN EL MOMENTO QUE SE COLISIONA CON EL ARMA MODIFICAR ANIMACION DE PERSONAJE A SIN BRAZOS*/
        playerSpriteAnimator = GameObject.FindGameObjectWithTag("TutorialRequirement").GetComponent<Animator>();
        
    }

    void Update()
    {
        

        if(Input.GetButtonDown("Fire2"))
        {
            InstanceW1();
        }

    }

    void InstanceW1()
    {
        GameObject Weapon = Instantiate(weaponPrefab, playerPosition.position, Quaternion.identity);
        Weapon.transform.parent = GameObject.Find("Player").transform;
        playerSpriteAnimator.SetLayerWeight(1,1);
    }
}
