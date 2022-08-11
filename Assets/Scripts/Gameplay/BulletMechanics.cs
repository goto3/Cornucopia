using UnityEngine;

public class BulletMechanics : MonoBehaviour
{

    public GameObject hitEffect;

    private float time = 0f;

    private void Update()
    {
        time += Time.deltaTime;
        if (time > 2) Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);

        Destroy(effect, 5f);
        Destroy(gameObject);

    }
}
