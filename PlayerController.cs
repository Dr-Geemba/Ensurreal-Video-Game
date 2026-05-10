using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private const float speed = 5f;
    private const float force = 7f;
    private bool hasJumped = false;
    public Transform groundCheck;
    private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    private bool isFacingRight;
    private const float attackRadius = 1.5f;
    [SerializeField] private LayerMask enemyLayer;
    private Vector3 horizontalAttackOffset = new Vector2(1,0);
    private Vector3 verticalAttackOffset = new Vector2(0, 1.25f);
    [SerializeField] private GameObject hitBox;
    private const float attackCooldown = 0.3f;
    private float timeTillNextAttack = 0f;
    private bool isFacingUp;
    private const int playerDamage = 8;
    void Start()
    {
        playerRigidbody = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.position += Vector3.right * speed * Time.deltaTime;
            isFacingRight = true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.position += -Vector3.right * speed * Time.deltaTime;
            isFacingRight = false;
        }

        if (Input.GetKey(KeyCode.W))
        {
            isFacingUp = true;
        }
        else
        {
            isFacingUp = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            hasJumped = true;
        }

        if (Input.GetButtonDown("Fire1") && Time.time > timeTillNextAttack)
        {
            Attack();
            timeTillNextAttack = Time.time + attackCooldown;
        }
        if(CurrentData.Instance.playerHealth == 0)
        {
            Debug.Log("player died");
            Destroy(gameObject);
        }
    }
    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (hasJumped)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.AddForce(Vector3.up * force, ForceMode2D.Impulse);
            hasJumped= false;
        }
    }
    private void Attack()
    {
        Collider2D[] hitEnemys;
        if(isFacingUp == true)
        {
            hitEnemys = Physics2D.OverlapBoxAll(gameObject.transform.position + verticalAttackOffset, new Vector2(1, 1) * attackRadius, 0, enemyLayer);
            hitBox.transform.position = gameObject.transform.position + verticalAttackOffset;
        }

        else if(isFacingRight == true)
        {
            hitEnemys = Physics2D.OverlapBoxAll(gameObject.transform.position + horizontalAttackOffset, new Vector2(1,1) * attackRadius, 0, enemyLayer);
            hitBox.transform.position = gameObject.transform.position + horizontalAttackOffset;
        }
        else
        {
            hitEnemys = Physics2D.OverlapBoxAll(gameObject.transform.position - horizontalAttackOffset, new Vector2(1, 1) * attackRadius, 0, enemyLayer);
            hitBox.transform.position = gameObject.transform.position - horizontalAttackOffset;

        }

        foreach (Collider2D enemy in hitEnemys) 
        {
            Debug.Log(enemy.name);
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if(damageable != null)
            {
                damageable.TakeDamage(playerDamage);
            }
        }
        StartCoroutine(FlashHitbox());
    }
    IEnumerator FlashHitbox()
    {
        hitBox.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        hitBox.SetActive(false);
    }
}
