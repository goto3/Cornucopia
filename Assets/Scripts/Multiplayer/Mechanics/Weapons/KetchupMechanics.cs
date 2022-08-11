using Assets.Scripts.Multiplayer.Mechanics.Weapons;
using Photon.Pun;
using UnityEngine;

namespace MP.Platformer.Mechanics
{
    public class KetchupMechanics : WeaponMechanics
    {
        public Transform firePoint;
        public GameObject bulletPrefab;
        public float bulletForce = 20f;
        public float atkSpd = 0.15f;

        public Animator animator;
        public Health health;

        private float lastAttack;

        void Start()
        {
            this.animator = GetComponent<Animator>();
            this.health = GetComponent<Health>();
        }

        void Update()
        {
        }

        void FixedUpdate()
        {
            if (photonView.IsMine)
            {
                lastAttack += Time.deltaTime;

                Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                Vector3 dir = pos - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        public override void Attack(PlayerController player)
        {
            if (lastAttack > atkSpd && ammo > 0)
            {
                
                lastAttack = 0;
                ammo--;
                if(ammo==0){
                    this.animator.SetBool("NoAmmo",true);
                }else{
                    this.animator.SetBool("NoAmmo",false);    
                }
                ChargeAttack(player, false, true);
                PhotonView photonView = PhotonView.Get(this);
                photonView.RPC("Attack", RpcTarget.All);
                
            }else if(ammo==0){
                ChargeAttack(player, false, true);
            }
            
        }

        public override Vector3 InstancePosition()
        {
            return new Vector3(0, 0, 0);
        }

        [PunRPC]
        void Attack()
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            //bullet.transform.parent = gameObject.transform;
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
        }

        [PunRPC]
        private void SetEnable(bool condition)
        {
            gameObject.SetActive(condition);
        }

        public override void ChargeAttack(PlayerController player, bool isCharging, bool isShooting)
        {
            animator.SetBool("isCharging", isCharging);
            animator.SetBool("isShooting", isShooting);
        }
    }
}
