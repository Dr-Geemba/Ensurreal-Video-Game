using UnityEngine;

public class CrawlEnemy : BasicEnemy
{
    // Image's reduce size is 12% of og
    [SerializeField] private SpriteRenderer spriteDirection;
    [SerializeField] private Animator animation;
    [SerializeField] private LayerMask groundLayer;
    //rayDistanceHorizontal = 0.76f;
    //rayDistanceVertical = 0.5f;
    private bool isDead = false;
    //NO DAMAGE TAKEN IF RUN INTO ENEMY FROM THE BACK
    void Start()
    {
        spriteDirection = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //Maybe we could move this to BasicEnemy.cs
        if(hp <= 0)
        {
            if (isDead == false)
            {
                gainMoney(money);
                animation.SetBool("isDead", true);
                isDead = true;
            }
            if (transform.position.y < -50.0f)
            {
                Destroy(gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead == false)
        {
            gameObject.transform.position += new Vector3(direction * speed * Time.deltaTime,0,0);        
            //Change ledgeCheckPos to an offset variable like in PlayerController at some point for consistancy
            //Could move raycasts to basic enemy since most enemies will need some raycasts for ledges and walls

            //Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + new Vector3(rayDistanceHorizontal*direction, 0, 0), Color.red);
            //Debug.DrawLine(ledgeCheckPos, ledgeCheckPos + new Vector3(0, -rayDistanceVertical, 0), Color.green);
            if (FloorWallRays.DrawRays(gameObject.transform.position, rayDistanceHorizontal, rayDistanceVertical, direction))
            {
                direction *= -1;
                animation.SetBool("isTurn", true);
            }
        }
    }

    public void death()
    {
        Destroy(GetComponent<Collider2D>());
    }

    public void turn()
    {
        spriteDirection.flipX = !spriteDirection.flipX;
        animation.SetBool("isAttack", false);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isDead)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.DamagePlayer(strength, direction);
            animation.SetBool("isAttack", true);
        }
    }
}
