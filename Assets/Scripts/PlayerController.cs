using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Starting pos is (-70, 0.7, 0)
    //THERE'S STILL A BUG WHERE WHEN YOU JUMP, YOU SOMETIMES MOVE LEFT OR RIGHT
    private Rigidbody2D playerRigidbody;
    private SpriteRenderer sprite;
    private AudioSource playerAudio;
    public AudioClip swingSFX;
    public AudioClip hurtSFX;
    private const float speed = 1f;
    private const float speedClimb = 4f;
    private const float force = 7.5f;
    private bool hasJumped = false;
    //Mark mode makes you able to go sideways on ladders 
    public bool markMode = false;
    public Transform groundCheck;
    private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;
    private bool isGrounded;
    public bool isClimbing;
    private bool isFacingRight;
    private const float attackRadius = 1.5f;
    [SerializeField] private LayerMask enemyLayer;
    private Vector3 horizontalAttackOffset = new Vector2(1,0);
    private Vector3 verticalAttackOffset = new Vector2(0, 1.25f);
    [SerializeField] private GameObject hitBox;
    private const float attackCooldown = 0.3f;
    private float timeTillNextAttack = 0f;
    private bool isFacingUp;
    [SerializeField] private GameObject canvas;
    private const float canvasCooldown = 3.5f;
    private float timeTillNextCanvas = 0;
    private Vector3 canvasOffset = Vector3.right;
    private const int playerDamage = 8;
    private const float maxSpeed = 5f;
    private float moveX;
    void Start()
    {
        playerRigidbody = gameObject.GetComponent<Rigidbody2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        playerAudio = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            isFacingRight = false;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            isFacingRight = true;
        }
        //Mark Mode
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!markMode)
            {
                sprite.color = new Color(1f, 1f, 0f);
            }
            else
            {
                sprite.color = new Color(1f, 0f, 0f);
            }
            markMode = !markMode;
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            isFacingUp = true;
        }
        else
        {
            isFacingUp = false;
        }
        //I forget why but I moved isGrounded to line 108

        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space)) && (isGrounded || isClimbing))
        {
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {

            }
            else
            {
                hasJumped = true;
            }
        }

        if (Input.GetButtonDown("Fire1") && Time.time > timeTillNextAttack)
        {
            Attack();
            playerAudio.PlayOneShot(swingSFX, 1.0f);
            timeTillNextAttack = Time.time + attackCooldown;
        }
        if(CurrentData.Instance.playerHealth == 0)
        {
            Debug.Log("player died");
            Destroy(gameObject);
        }
        if (Input.GetKey(KeyCode.E) && timeTillNextCanvas < Time.time)
        {
            timeTillNextCanvas = Time.time + canvasCooldown;
            SpawnCanvas();
        }
    }
    void FixedUpdate()
    {
        if (isClimbing)
        {
            playerRigidbody.gravityScale = 0f;
            float verticalInput = Input.GetAxisRaw("Vertical");

            // Set the velocity directly for smooth movement
            playerRigidbody.linearVelocity = new Vector2(playerRigidbody.linearVelocity.x, verticalInput * speedClimb);
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer | platformLayer);
        if (hasJumped)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.AddForce(Vector3.up * force, ForceMode2D.Impulse);
            hasJumped= false;
        }
        moveX = Input.GetAxisRaw("Horizontal");
        Vector3 movementDirection = new Vector3(moveX, 0f, 0f).normalized;
        if (markMode || !isClimbing)
        {
            if (movementDirection != Vector3.zero)
            {
                Vector3 horizontalVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0f, 0f);
                if (horizontalVelocity.magnitude < maxSpeed)
                {
                    playerRigidbody.AddForce(movementDirection * speed, ForceMode2D.Impulse);
                }
            }
            else
            {
                playerRigidbody.linearVelocity = new Vector3(0f, playerRigidbody.linearVelocity.y, 0f);
            }
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
    private void SpawnCanvas()
    {
        if (GameObject.FindWithTag("canvas") != null)
        {
            Destroy(GameObject.FindWithTag("canvas"));
        }
        if (isFacingRight)
        {
            Instantiate(canvas, transform.position + canvasOffset, canvas.transform.rotation);
        }
        else
        {
            Instantiate(canvas, transform.position - canvasOffset, canvas.transform.rotation);
        }    
    }
    IEnumerator FlashHitbox()
    {
        hitBox.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        hitBox.SetActive(false);
    }
}