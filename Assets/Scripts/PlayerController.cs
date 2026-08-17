using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Starting pos is (-92, 0.7, 0)
    //THERE'S STILL A BUG WHERE WHEN YOU JUMP, YOU SOMETIMES MOVE LEFT OR RIGHT
    //States that control horz movement: move, jump, fall
    public StateControllerTommy stateController;
    public PlayerIdle idleState;
    public PlayerMove moveState;
    public PlayerJumping jumpState;
    public PlayerFalling fallState;
    public PlayerDucking duckState;
    public PlayerClimbing climbState;
    public PlayerDeploying deployState;
    public PlayerAttacking attackState;
    public PlayerHurt hurtState;

    public bool input;
    public InputActionReference movementIn;
    public InputActionReference jumpIn;
    public InputActionReference attackIn;
    public InputActionReference deployIn;

    public Rigidbody2D playerRigidbody;
    public Transform transform;
    public SpriteRenderer sprite;
    public CapsuleCollider2D collider;
    private AudioSource playerAudio;
    public AudioClip swingSFX;
    public AudioClip hurtSFX;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask damageLayer;
    [SerializeField] private LayerMask attackLayer;
    [SerializeField] private PhysicsMaterial2D floor;
    [SerializeField] private PhysicsMaterial2D airborne;

    //Speed/Movement
    public Vector2 movement;

    public float direction = 1f;
    private float speed = 1f;
    private float maxSpeed = 5f;
    public  float speedClimb = 4f;
    private float moveX;

    //Jump
    public float jump;

    public float jumpForce = 5f;
    public bool hasJumped = false;
    public bool releaseJump = false;
    public Transform groundCheck;
    private float groundCheckRadius = 0.05f;

    //Bool checks
    public bool isGrounded;
    public bool isAttacking;
    public bool isFacingUp;

    //Attack stuff
    public float attack;

    private int playerDamage = 6;
    //Change attackRadius to a vector2
    private const float attackRadius = 1.5f;
    //Fix player attack stuff later
    private Vector3 horizontalAttackOffset = new Vector2(0.9f,0);
    private Vector3 verticalAttackOffset = new Vector2(0, 1.25f);
    [SerializeField] private GameObject hitBox;
    private const float attackCooldown = 0.4f;
    private float timeTillNextAttack = 0f;

    //Hit stuff
    public int hitStrength;
    public int hitDirection;
    public float iFrames = 1f;
    public float timeTillDamageable = 0f;

    //Canvas
    public float deploy;

    [SerializeField] private GameObject canvas;
    public Vector3 canvasOffset;
    public float rayDistanceHorizontal = 0.8f;
    public float rayDistanceVertical = 1.01f;
    private bool hitsWall;
    private bool hitsFloor;

    void Awake()
    {
        stateController = new StateControllerTommy();

        idleState = new PlayerIdle(stateController, this);
        moveState = new PlayerMove(stateController, this);
        jumpState = new PlayerJumping(stateController, this);
        fallState = new PlayerFalling(stateController, this);
        duckState = new PlayerDucking(stateController, this);
        climbState = new PlayerClimbing(stateController, this);
        deployState = new PlayerDeploying(stateController, this);
        hurtState = new PlayerHurt(stateController, this);

        playerRigidbody = gameObject.GetComponent<Rigidbody2D>();
        transform = gameObject.GetComponent<Transform>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        collider = gameObject.GetComponent<CapsuleCollider2D>();
        playerAudio = gameObject.GetComponent<AudioSource>();
    }

    void Start()
    {
        stateController.StartState(idleState);
    }

    void Update()
    {
        stateController.currentState.Update();
        //I forget why but I moved isGrounded to line 108

        //SWITCH JUMP TO STATES
        if (Input.GetButtonDown("Jump") && (CanJump()))
        {
            hasJumped = true;
        }

        if (Input.GetButtonDown("Fire1") && Time.time > timeTillNextAttack && input)
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

        if (Time.time > timeTillDamageable && stateController.currentState != deployState)
        {
            sprite.color = new Color(1f, 0f, 0f);
        }
    }
    void FixedUpdate()
    {
        stateController.currentState.FixedUpdate();

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer | platformLayer);
        collider.sharedMaterial = isGrounded ? floor : airborne;
    }

    private bool CanJump()
    {
        if (!input) return false;
        return ((isGrounded || stateController.currentState == climbState) && stateController.currentState != duckState);
    }

    public bool CanDeploy()
    {
        if (!input) return false;
        canvasOffset = transform.position + new Vector3(rayDistanceHorizontal*direction,0,0);
        //Add tall raycast for tight spaces
        RaycastHit2D hitWall = Physics2D.Raycast(gameObject.transform.position, Vector2.right * direction, rayDistanceHorizontal+0.5f, groundLayer);
        RaycastHit2D hitFloor = Physics2D.Raycast(canvasOffset, Vector2.down, rayDistanceVertical, groundLayer | platformLayer);
        hitsWall = hitWall ? true : false;
        hitsFloor = hitFloor ? true : false;
        return (!hitsWall && hitsFloor);
    }

    public void UpdateInput()
    {
        //Updates all input
        //WASD movement + change direction when appropriate
        movement = movementIn.action.ReadValue<Vector2>();
        if (movement.x == 1) direction = 1f;
        else if (movement.x == -1) direction = -1f;

        //Other buttons
        jump = jumpIn.action.ReadValue<float>();

    }

    public void Movement()
    {
        float velX = playerRigidbody.linearVelocity.x;
        Vector2 horMovement = new Vector2(movement.x, 0f);
        if (Mathf.Abs(velX) < maxSpeed || velX / Mathf.Abs(velX) != movement.x / Mathf.Abs(movement.x))
        {
            playerRigidbody.AddForce(horMovement * speed, ForceMode2D.Impulse);
        }
        else
        {
            velX = 5f;
        }
    }

    public void DeployCanvas()
    {
        if (GameObject.FindWithTag("canvas") != null)
        {
            Destroy(GameObject.FindWithTag("canvas"));
        }
        Instantiate(canvas, transform.position + new Vector3(rayDistanceHorizontal*direction, 0.175f, 0f), canvas.transform.rotation);
    }

    private void Attack()
    {
        Collider2D[] hitTargets;
        if(isFacingUp == true)
        {
            hitTargets = Physics2D.OverlapBoxAll(gameObject.transform.position + verticalAttackOffset, new Vector2(1, 1) * attackRadius, 0, attackLayer);
            hitBox.transform.position = gameObject.transform.position + verticalAttackOffset;
        }

        else
        {
            hitTargets = Physics2D.OverlapBoxAll(gameObject.transform.position + horizontalAttackOffset*direction, new Vector2(1,1) * attackRadius, 0, attackLayer);
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
    public void DamagePlayer(int strength, int direction)
    {
        if (Time.time <= timeTillDamageable) return;

        hitStrength = strength;
        hitDirection = direction;
        stateController.ChangeState(hurtState);
    }
    IEnumerator FlashHitbox()
    {
        hitBox.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        hitBox.SetActive(false);
    }
}