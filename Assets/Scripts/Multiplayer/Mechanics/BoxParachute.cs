using Photon.Pun;
using UnityEngine;

namespace MP.Platformer.Mechanics
{
    public class BoxParachute : MonoBehaviourPunCallbacks, IPooledObject
    {
        public float upForce = 0.1f;
        public float sideForce = 0.1f;
        internal Animator animator;
        internal Rigidbody2D rb;
        public Vector2 pos;
        bool instance;
        float liveTime = 0;

        void Start()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
        }

        public void OnObjectSpawn()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            liveTime = 0;
            GetComponent<Rigidbody2D>().gravityScale = 1;
            GetComponent<Collider2D>().enabled = true;
            GetComponent<Animator>().SetBool("BoxOpen", false);
            GetComponent<Animator>().SetBool("Idle", true);
            float xForce = Random.Range(-sideForce, sideForce);
            float yForce = Random.Range(upForce / 2f, upForce);
            instance = true;
            Vector2 force = new Vector2(xForce, yForce);

            GetComponent<Rigidbody2D>().velocity = force;
        }

        void Update()
        {
            pos = transform.position;

            if (pos.y > 10.3) // Free fall
            {
                animator.SetBool("BoxOpen", false);
                animator.SetBool("Idle", true);
                GetComponent<Rigidbody2D>().rotation = 0;
            }
            if (pos.y > 7.5 && pos.y < 10.3) // Parachute
            {
                GetComponent<Rigidbody2D>().drag = 12;
                GetComponent<Rigidbody2D>().rotation = 0;
                animator.SetBool("Parachute", true);
            }
            if (pos.y <= 7.5) // Free fall
            {
                GetComponent<Rigidbody2D>().drag = 0;
                animator.SetBool("Parachute", false);
                animator.SetBool("Idle", true);
                GetComponent<Rigidbody2D>().rotation = 0;
            }
            if (pos.x > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x - 1, rb.velocity.y);
            }
            if (pos.x < -20)
            {
                rb.velocity = new Vector2(rb.velocity.x + 1, rb.velocity.y);
            }
        }

        void FixedUpdate()
        {            
            if (!PhotonNetwork.IsMasterClient) return;

            liveTime += Time.deltaTime;
            if (rb.velocity.magnitude < 0.5 && instance)
            {
                animator.SetBool("BoxOpen", true);
                var animationStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (animationStateInfo.IsName("BoxOpenFloor") && animationStateInfo.normalizedTime > 1)
                {
                    instance = false;
                    photonView.RPC("DisableCollision", RpcTarget.All);
                    GameManager.Instance.SpawnRandomItem(pos);
                }
            }

            if (transform.position.y < -4 && liveTime > 2)
            {
                GameManager.Instance.CurrentMap.GetComponent<PhotonView>().RPC("OnItemPickupOrDestroy", RpcTarget.MasterClient);
                photonView.RPC("Enable", RpcTarget.All, false); 
            }
            
        }

        [PunRPC]
        void DisableCollision()
        {
            GetComponent<Collider2D>().enabled = false;
            rb.gravityScale = 0;
            rb.velocity = new Vector2(0, 0);
        }


        [PunRPC]
        void Enable(bool value)
        {
            gameObject.SetActive(value);
        }

    }
}

