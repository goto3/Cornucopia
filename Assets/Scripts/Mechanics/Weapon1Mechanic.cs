using UnityEngine;

public class Weapon1Mechanic : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float moving = 3;

    public Rigidbody2D rb;
    public Camera cam;
    internal Animator animator;
    public SpriteRenderer playerSpriteRender;
    public Animator playerSpriteAnimator;
    public Transform target;

    Vector2 movement;
    Vector2 mousePos;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("TutorialRequirement").GetComponent<Transform>();
        playerSpriteRender = GameObject.FindGameObjectWithTag("TutorialRequirement").GetComponent<SpriteRenderer>();
        playerSpriteAnimator = GameObject.FindGameObjectWithTag("TutorialRequirement").GetComponent<Animator>();

    }

    void Update()
    {

        transform.position = Vector2.MoveTowards(transform.position, target.position, moving * Time.fixedDeltaTime);

        movement.x = transform.position.x;//Input.GetAxisRaw("Horizontal"); 
        movement.y = transform.position.y;//Input.GetAxisRaw("Vertical");

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if(mousePos.x > 3.8){
           //Debug.Log("Derecha: " + mousePos.x );
           playerSpriteRender.flipX = false;
           if (Input.GetKey("right")){
                playerSpriteAnimator.SetLayerWeight(1,1); 
           }else{
               playerSpriteAnimator.SetLayerWeight(2,1); 
           }

        }else{
            playerSpriteRender.flipX = true;
           //Debug.Log("Izquierda: " + mousePos.x );
            if (Input.GetKey("right")){
               playerSpriteAnimator.SetLayerWeight(2,1); 
           }else{
               playerSpriteAnimator.SetLayerWeight(1,1); 
           }
       
         }   
            
        /*if((movement.x == transform.position.x)&&(movement.y == transform.position.y)) {
            moveSpeed = 1f;
            moving = 1;
        }else{
            moveSpeed = 5f;
            moving = 3;
        }*/
    }

    void FixedUpdate()
    {

        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        /*Vector2 lookDir = mousePos - rb.position;
        lookDir.Normalize();
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        rb.rotation = angle;*/

        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        Vector3 dir = pos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
