using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    internal Animator animator;
    public float bulletForce = 20f;
    bool isShooting;

    void Awake()
        {            
            animator = GetComponent<Animator>();
        }

    void Update()
    {

        
        if(Input.GetMouseButtonDown(0))
        {         
            changeWeaponState(true,false);           
        }

        if(Input.GetMouseButtonUp(0))
        {                      
            Shoot();
        }


    }

    void changeWeaponState(bool isCharginValue, bool isShootingValue){
        
            animator.SetBool("isCharging", isCharginValue);
            animator.SetBool("isShooting", isShootingValue);  
    }

    void Shoot()
    {
        changeWeaponState(false,true);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
        
    }
}
