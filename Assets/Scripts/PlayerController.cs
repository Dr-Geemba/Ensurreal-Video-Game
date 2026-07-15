using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Starting pos is (-92, 0.9, 0)
    //THERE'S STILL A BUG WHERE WHEN YOU JUMP, YOU SOMETIMES MOVE LEFT OR RIGHT
    private Rigidbody2D playerRigidbody;
    private SpriteRenderer sprite;
    private CapsuleCollider2D collider;
    private AudioSource playerAudio;
    public AudioClip swingSFX;
    public AudioClip hurtSFX;
    public int direction = 1;
    private const float speed = 1f;
    private const float speedClimb = 4f;
    private const float force = 7.5f;
    private bool hasJumped = false;
    public Transform groundCheck;
    private float groundCheckRadius = 0.05f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask damageLayer;
    [SerializeField] private PhysicsMaterial2D floor;
    [SerializeField] private PhysicsMaterial2D airborne;
    public bool isGrounded;
    public bool isClimbing;
    public bool isDucking;
    private bool isDeploying;
    //Change attackRadius to a vector2
    private const float attackRadius = 1.5f;
    [SerializeField] private LayerMask attackLayer;
    private Vector3 horizontalAttackOffset = new Vector2(0.8f, 0);
    private Vector3 verticalAttackOffset = new Vector2(0, 1.75f);
    [SerializeField] private GameObject hitBox;
    private const float attackCooldown = 0.4f;
    private float timeTillNextAttack = 0f;
    public float iFrames = 1f;
    public float timeTillDamageable = 0f;
    private bool isFacingUp;
    [SerializeField] private GameObject canvas;
    private float deployTime = 1f;
    private float timeTillFinishDeploy = 0f;
    private const float rayDistanceVertical = 1.01f;
    private const float rayDistanceHorizontal = 0.8f;
    private bool hitsWall;
    private bool hitsFloor;
    private Vector3 canvasOffset;
    private int playerDamage = 6;
    private const float maxSpeed = 5f;
    private float moveX;
    void Start()
    {
        playerRigidbody = gameObject.GetComponent<Rigidbody2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        collider = gameObject.GetComponent<CapsuleCollider2D>();
        playerAudio = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isDeploying)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                {
                    direction = -1;
                }
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                {
                    direction = 1;
                }
            if (!isDucking)
            {
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                {
                    isFacingUp = true;
                }
                else
                {
                    isFacingUp = false;
                }
            }
            if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && isGrounded && !isClimbing)
            {
                isDucking = true;
            }
            else
            {
                isDucking = false;
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
            if (Input.GetKey(KeyCode.E) && (isGrounded && !isClimbing) && (!hitsWall && hitsFloor))
            {
                isDucking = false;
                timeTillFinishDeploy = Time.time + deployTime;
                SpawnCanvas();
            }
        }
        else
        {
            if (!isGrounded)
            {
                ExitCanvas();
            }
            else if (Time.time > timeTillFinishDeploy)
            {
                ExitCanvas();
                if (GameObject.FindWithTag("canvas") != null)
                {
                    Destroy(GameObject.FindWithTag("canvas"));
                }
                Instantiate(canvas, transform.position + new Vector3(rayDistanceHorizontal*direction, 0.175f, 0f), canvas.transform.rotation);
            }
        }
        if (Time.time > timeTillDamageable && Time.time > timeTillFinishDeploy)
        {
            sprite.color = new Color(1f, 0f, 0f);
        }
    }
    void FixedUpdate()
    {
        canvasOffset = gameObject.transform.position + new Vector3(rayDistanceHorizontal*direction,0,0);
        //We can move this somewhere else, but it's here for testing purposes or smth
        //Also add tall raycast for tight spaces
        RaycastHit2D hitWall = Physics2D.Raycast(gameObject.transform.position, Vector2.right * direction, rayDistanceHorizontal+0.5f, groundLayer);
        RaycastHit2D hitFloor = Physics2D.Raycast(canvasOffset, Vector2.down, rayDistanceVertical, groundLayer | platformLayer);
        hitsWall = hitWall ? true : false;
        hitsFloor = hitFloor ? true : false;

        Debug.DrawLine(gameObject.transform.position, canvasOffset, Color.red);
        Debug.DrawLine(canvasOffset, canvasOffset + new Vector3(0, -rayDistanceVertical, 0), Color.green);
        if (isClimbing)
        {
            playerRigidbody.gravityScale = 0f;
            float verticalInput = Input.GetAxisRaw("Vertical");

            // Set the velocity directly for smooth movement
            playerRigidbody.linearVelocity = new Vector2(playerRigidbody.linearVelocity.x, verticalInput * speedClimb);
            if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && isGrounded)
            {
                isClimbing = false;
                playerRigidbody.gravityScale = 1f;
            }
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer | platformLayer);
        collider.sharedMaterial = isGrounded ? floor : airborne;
        if (hasJumped)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.AddForce(Vector3.up * force, ForceMode2D.Impulse);
            hasJumped= false;
        }
        if (isDucking)
        {
            collider.size = new Vector2(1, 1);
            collider.offset = new Vector2(collider.offset.x, -0.5f);
        }
        else
        {
            collider.size = new Vector2(1, 2);
            collider.offset = new Vector2(collider.offset.x, 0f);
        }
        moveX = Input.GetAxisRaw("Horizontal");
        Vector3 movementDirection = new Vector3(moveX, 0f, 0f).normalized;
        if ((!isClimbing) && !isDucking && !isDeploying)
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
        Collider2D[] hitTargets;
        if(isFacingUp == true)
        {
            hitTargets = Physics2D.OverlapBoxAll(gameObject.transform.position + verticalAttackOffset, new Vector2(1, 1.3f) * attackRadius, 0, attackLayer);
            hitBox.transform.position = gameObject.transform.position + verticalAttackOffset;
        }

        else
        {
            hitTargets = Physics2D.OverlapBoxAll(gameObject.transform.position + horizontalAttackOffset*direction, new Vector2(1.3f, 1) * attackRadius, 0, attackLayer);
            hitBox.transform.position = gameObject.transform.position + horizontalAttackOffset*direction;
        }

        foreach (Collider2D target in hitTargets) 
        {
            Debug.Log(target.name);
            IDamageable damageable = target.GetComponent<IDamageable>();
            if(damageable != null)
            {
                damageable.TakeDamage(playerDamage);
            }
        }
        StartCoroutine(FlashHitbox());
    }
    private void SpawnCanvas()
    {
        playerRigidbody.linearVelocity = Vector3.zero;
        isDeploying = true;
        sprite.color = new Color(0.5f, 0f, 0.5f);
        timeTillFinishDeploy = Time.time + deployTime;

    }
    private void ExitCanvas()
    {
        isDeploying = false;
        timeTillFinishDeploy = Time.time;
        sprite.color = new Color(1f, 0f, 0f);
    }
    public void DamagePlayer(int strength)
    {
        //NEW DAMAGE SYSTEM! Enemies have a strength number instead of a certain damage output
        //With this, we can give buffs/debuffs to enemies to change how much damage they do and stuff
        Dictionary<int, decimal> damage = new Dictionary<int, decimal>
        {
            [0] = 0.2m,
            [1] = 1m,
            [2] = 2m,
            [3] = 3m,
            [4] = 5m,
            [5] = 8m,
        };
        if (Time.time > timeTillDamageable)
        {
            if (strength > 5)
            {
                CurrentData.Instance.playerHealth = 0;
            }
            else if (strength >= 0)
            {
                if (strength != 0)
                {
                    CurrentData.Instance.playerHealth = System.Math.Ceiling(CurrentData.Instance.playerHealth);
                }
                CurrentData.Instance.playerHealth -= damage[strength];
            }
            timeTillDamageable = Time.time + iFrames;
            ExitCanvas();
            isClimbing = false;
            playerRigidbody.gravityScale = 1f;
            sprite.color = new Color(0.5f, 0f, 0f);
        }
    }
    IEnumerator FlashHitbox()
    {
        hitBox.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        hitBox.SetActive(false);
    }
}