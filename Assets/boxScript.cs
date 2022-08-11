using UnityEngine;

public class boxScript : MonoBehaviour, IPooledObject
{
    public float upForce = 1f;
    public float sideForce = .1f;
    internal Animator animator;
    internal Rigidbody2D rb;
    public Vector2 pos;
    public GameObject myPrefab;
    bool instance;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }


    public void OnObjectSpawn()
    {
        float xForce = Random.Range(-sideForce, sideForce);
        float yForce = Random.Range(upForce / 2f, upForce);
        instance = true;
        Vector2 force = new Vector2(xForce, yForce);

        GetComponent<Rigidbody2D>().velocity = force;
    }

    void Update()
    {
        pos = transform.position;

        if (pos.y > 7)
        {
            animator.SetBool("BoxOpen", false);
            animator.SetBool("Idle", true);
            GetComponent<Rigidbody2D>().rotation = 0;
        }

        if (pos.y > 3 && pos.y < 7)
        {
            GetComponent<Rigidbody2D>().drag = 12;
            GetComponent<Rigidbody2D>().rotation = 0;
            animator.SetBool("Parachute", true);
        }

        if (pos.y <= 3)
        {
            GetComponent<Rigidbody2D>().drag = 0;
            animator.SetBool("Parachute", false);
            animator.SetBool("Idle", true);
            GetComponent<Rigidbody2D>().rotation = 0;
            //rb.velocity = new Vector3(0, 0, 0);
        }

        if (pos.x > 13)
        {
            Debug.Log("SE FUE DER");
            rb.velocity = new Vector2(rb.velocity.x - 1, rb.velocity.y);
        }

        if (pos.x < -5)
        {
            Debug.Log("SE FUE IZQ");
            rb.velocity = new Vector2(rb.velocity.x + 1, rb.velocity.y);
        }

    }

    void FixedUpdate()
    {
        if (pos.y <= 3)
        {
            if (rb.velocity.y < 0f)
            {
                animator.SetBool("BoxOpen", true);
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("BoxOpenFloor"))
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                    {
                        if (instance)
                        {
                            myPrefab = GameObject.FindGameObjectWithTag("Player");
                            Instantiate(myPrefab, new Vector2(pos.x, pos.y + 1), Quaternion.identity);
                            instance = false;
                        }
                    }
                }
            }
            else
            {
                animator.SetBool("Idle", true);
            }
        }
    }
}
