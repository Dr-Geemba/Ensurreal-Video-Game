using UnityEngine;

public class CrawlEnemy : BasicEnemy, IDamageable
{
    [SerializeField] private LayerMask groundLayer;
    private const float rayDistanceHorizontal = .1f;
    private const float rayDistanceVertical = 1f;
    private const float enemySpeed = 3f;
    private bool isFacingRight = true;
    private float timeTillHitAgain = 0f;
    public float enemyHp = 15f;
    // Update is called once per frame
    void Update()
    {
        float direction = isFacingRight ? .3f:-.3f;
        gameObject.transform.position += new Vector3(direction * enemySpeed * Time.deltaTime,0,0);        
        
        Vector3 enemyHead = gameObject.transform.position + new Vector3(direction,0,0);
        
        RaycastHit2D hitWall = Physics2D.Raycast(enemyHead, Vector2.right * direction, rayDistanceHorizontal, groundLayer);
        RaycastHit2D hitEdge = Physics2D.Raycast(enemyHead, Vector2.down, rayDistanceVertical, groundLayer);
        Debug.DrawRay(enemyHead, Vector2.right * direction * rayDistanceHorizontal, Color.red);
        Debug.DrawRay(enemyHead, Vector2.down * rayDistanceVertical, Color.green);
        if (hitWall || !hitEdge)
        {
            isFacingRight = !isFacingRight;
        }

        if(enemyHp <= 0)
        {
            gainMoney(5);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            dealDamage(1);
            timeTillHitAgain = Time.time + iFrames;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(Time.time > timeTillHitAgain && collision.gameObject.CompareTag("Player"))
        {
            dealDamage(1);
            timeTillHitAgain = Time.time + iFrames;
        }
    }

    public void TakeDamage(int damage)
    {
        enemyHp -= damage;
    }
}
