using Assets.Scripts.Multiplayer.Mechanics.Weapons;
using MP.Platformer.Mechanics;
using Photon.Pun;
using UnityEngine;

public class KnifeMechanics : WeaponMechanics
{
    // Start is called before the first frame update

    public float moveSpeed = 5f;
    public float moving = 3;
    public float atkSpd = 0.15f;
    public int damage = 10;

    public Rigidbody2D rb;
    internal Animator animator;

    public Transform attackPoint;
    public float attackRange;
    public LayerMask enemyLayers;

    int attack = 0;
    float attackTimer = -9999f;
    private float lastAttack;

    private Vector2 mousePos;
    private bool aimingLeft = false;
    private Camera cam;
    private PlayerController PlayerController;
    private SpriteRenderer sprite;
    private RectTransform hitPoint;

    void Start()
    {
        animator = GetComponent<Animator>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        PlayerController = GetComponentInParent<PlayerController>();
        sprite = GetComponent<SpriteRenderer>();
        hitPoint = GetComponentInChildren<RectTransform>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            lastAttack += Time.deltaTime;

            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            aimingLeft = mousePos.x < PlayerController.transform.position.x;

            if (aimingLeft)
            {
                hitPoint.transform.localPosition = new Vector2(-0.1f, 0);
                gameObject.transform.localPosition = new Vector2(-0.5f, 0.1f);
            }
            else
            {
                hitPoint.transform.localPosition = new Vector2(0.1f, 0);
                gameObject.transform.localPosition = new Vector2(0.5f, 0.1f);
            }
            animator.SetBool("facingLeft", aimingLeft);
        }

        aimingLeft = animator.GetBool("facingLeft");
        sprite.flipX = aimingLeft;
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        if (attackTimer <= 0f)
            finishAttack();
        else
            attackTimer -= Time.deltaTime;
    }
    public override void Attack(PlayerController player)
    {
        if (lastAttack > atkSpd)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                if (!enemy.gameObject.GetComponent<PhotonView>().IsMine)
                    enemy.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
            }

            lastAttack = 0;
            attackTimer = 0.5f;
            attack++;
            switch (attack)
            {
                case 3:
                    animator.SetBool("thirdAttack", true);
                    break;
                case 2:
                    animator.SetBool("secondAttack", true);
                    break;
                case 1:
                    animator.SetBool("firstAttack", true);
                    break;
                default:
                    finishAttack();
                    break;
            }
        }
    }
    void finishAttack()
    {
        attack = 0;
        animator.SetBool("thirdAttack", false);
        animator.SetBool("secondAttack", false);
        animator.SetBool("firstAttack", false);
    }

    public override Vector3 InstancePosition()
    {
        return new Vector3(0.5f, 0.1f, 0);
    }

    [PunRPC]
    private void SetEnable(bool condition)
    {
        gameObject.SetActive(condition);
    }

    public override void ChargeAttack(PlayerController player, bool isCharging, bool isShooting)
    {
    }
}
