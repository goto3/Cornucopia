using Photon.Pun;
using UnityEngine;
namespace MP.Platformer.Gameplay
{
    public class BulletMechanics : MonoBehaviourPun
    {

        public GameObject hitEffect;

        private float time = 0f;

        private static readonly int damage = 5;

        private void Update()
        {
            time += Time.deltaTime;
            if (time > 2) Destroy(gameObject);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Player" && !collision.gameObject.GetComponent<PhotonView>().IsMine)
            {
                var player = collision.gameObject.GetComponent<PhotonView>();
                player.RPC("TakeDamage", RpcTarget.All, damage);
            }

            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(effect, 5f);
            Destroy(gameObject);
        }
    }
}

