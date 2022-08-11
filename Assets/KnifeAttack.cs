using UnityEngine;

public class KnifeAttack : MonoBehaviour
{

    public GameObject knifePrefab;
    internal Animator animator;
    int attack = 0;
    float attackTimer = -9999f;
    public Transform attackPoint;
    public float attackRange;
    public LayerMask enemyLayers;
    public int ammo = 10;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        if (Input.GetButtonDown("Fire1") && (ammo > 0))
        {
            startAttack();
            
        }

        void startAttack()
        {

            attackTimer = 0.5f;
            attack++;
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                Debug.Log("We hit" + enemy.name);
            }

            switch (attack)
            {
                case 3:
                    animator.SetBool("thirdAttack", true);
                    ammo--;
                    break;
                case 2:
                    animator.SetBool("secondAttack", true);
                    ammo--;
                    break;
                case 1:
                    animator.SetBool("firstAttack", true);
                    ammo--;
                    break;
                default:
                    finishAttack();
                    break;
            }

        }

        if (attackTimer <= 0f)
        {
            finishAttack();
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }


        void finishAttack()
        {
            animator.SetTrigger("Not_Attack");
            attack = 0;
            attackTimer = 0f;
            animator.SetBool("thirdAttack", false);
            animator.SetBool("secondAttack", false);
            animator.SetBool("firstAttack", false);
        }

    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }


}

