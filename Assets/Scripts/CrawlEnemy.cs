using UnityEngine;

public class CrawlEnemy : BasicEnemy, IDamageable
{
    // Image's reduce size is 12% or og
    [SerializeField] private SpriteRenderer spriteDirection;
    [SerializeField] private Animator animation;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;
    private const float rayDistanceHorizontal = .75f;
    private const float rayDistanceVertical = 1f;
    private const float speed = 1.3f;
    public bool isFacingRight = false;
    private bool isDead = false;
    private float timeTillHitAgain = 0f;
    public float hp = 15f;
    public int money = 5;
    //FIX INVISIBILITY IF YOU DON'T LEAVE ENEMY HIT BOX
    void Start()
    {
        spriteDirection = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
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
            float direction = isFacingRight ? 1f:-1f;
            float distanceHead = direction*0.75f;
            gameObject.transform.position += new Vector3(direction * speed * Time.deltaTime,0,0);        
            
            Vector3 enemyHead = gameObject.transform.position + new Vector3(distanceHead,0,0);
            
            RaycastHit2D hitWall = Physics2D.Raycast(gameObject.transform.position, Vector2.right * direction, rayDistanceHorizontal, groundLayer);
            RaycastHit2D hitFloor = Physics2D.Raycast(enemyHead, Vector2.down, rayDistanceVertical, groundLayer);

            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + new Vector3(rayDistanceHorizontal*direction, 0, 0), Color.red);
            Debug.DrawLine(enemyHead, enemyHead + new Vector3(0, -rayDistanceVertical, 0), Color.green);
            if (hitWall || !hitFloor)
            {
                isFacingRight = !isFacingRight;
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isDead)
        {
            dealDamage(1);
            timeTillHitAgain = Time.time + iFrames;
            animation.SetBool("isAttack", true);
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
    }
}
