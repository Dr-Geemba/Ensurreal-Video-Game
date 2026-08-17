using System.Collections;
using UnityEngine;

public class GoblinEnemy : ComplexEnemy
{
    //Goblin can swing hitbox around at last moment of attack
    private SpriteRenderer sprite;
    //I'm going to add an aggro system or something later where the enemies can switch targets to the canvas or other npcs.
    //It will be more complicated so I'm not going to implement it yet.
    //This decides if enemy has been idling too much and makes them return if the y pos is the same (they are on same platform)
    private float startMoveX;
    private float moveCooldown = 1f;
    private float shortMoveCooldown = 0.2f;
    private float timeTillNextMove = 0f;
    private float attackCooldown = 1f;
    private float timeTillCanAttack = 0f;
    private float attackTelegraph = 0.8f;
    private float attackDuration = 0.4f;
    private float timeTillAttack = 0f;
    //Distances determine what the goblin does next
    private const float midDistance = 6f;
    private const float shortDistance = 3f;
    //If you want to turn this back on, just let Dean know
    //private const float turnDeadZone = 0.2f;
    private float movementTime = 0f;
    //rayDistanceHorizontal = 0.6f;
    //rayDistanceVertical = 0.85f;
    private MovementStates move = MovementStates.idle;
    private Vector2 hitBox = new Vector2(1.3f, 1.2f);
    private Vector2 attackBox = new Vector2(1.3f, 1.7f);
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;
    public Rigidbody2D goblinRigidBody;
    [SerializeField] private GameObject attackVisual;

    void Start()
    {
        startPos = transform.position;
        goblinRigidBody = GetComponent<Rigidbody2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        PreSpriteDeath();
        if (!hostile) return;
        if (Time.time > timeTillChill && !(timeTillChill == 0f))
        {
            hostile = false;
            moveBack = true;
        }
        else if (move == MovementStates.idle)
        {
            FindDirection(target.position.x);
            if (Time.time > timeTillNextMove) ChooseMove();
        }
        else if (move == MovementStates.attacking)
        {
            AttackSequence();
        }
        if (Time.time > timeTillCanAttack && Physics2D.OverlapBox(gameObject.transform.position, attackBox, 0, playerLayer))
        {
            AttemptAttack();
        }
    }
    void FixedUpdate()
    {
        if (moveBack) MoveBack(gameObject.transform, goblinRigidBody);
        
        if (!hostile) return;
        if ((int)move < 2)
        {
            goblinRigidBody.linearVelocityX = 0;
        }
        else
        {
            //I kept this in FixedUpdate so that if the goblin is moving and wants to keep moving, it can
            if (Time.time > movementTime && Time.time > timeTillNextMove)
            {
                ChooseMove();
                if (!(move == MovementStates.idle))
                {
                    int initialDir = direction;
                    FindDirection(target.position.x);
                    if (initialDir != direction)
                    {
                        ShortStop();
                    }
                }
            }
            if ((int)move > 1) MovementControl(move);
        }
    }

    void ChooseMove()
    {
        if (Physics2D.OverlapCircle(gameObject.transform.position, shortDistance, playerLayer))
        {
            int choice = Random.Range(0, 8);
            move = (choice < 5) ? MovementStates.forwards : MovementStates.backwards;
        }
        else if (Physics2D.OverlapCircle(gameObject.transform.position, midDistance, playerLayer))
        {
            int choice = Random.Range(0, 6);
            move = (choice < 4) ? MovementStates.forwards : MovementStates.idle;
        }
        else
        {
            move = MovementStates.forwards;
        }
        timeTillNextMove = Time.time + moveCooldown;
        if ((int)move > 1)
        {
            movementTime = Time.time + movementLength;
        }
    }

    void MovementControl(MovementStates move)
    {
        int moveDir = MoveToDir(move);
        if (FloorWallRays.DrawRays(gameObject.transform.position, rayDistanceHorizontal, rayDistanceVertical, direction*moveDir))
        {
            if (timeTillChill == 0f) timeTillChill = Time.time + chillAmount;
            ShortStop();
        }
        else
        {
            timeTillChill = 0f;
            RaycastHit2D hitPlayer = Physics2D.Raycast(gameObject.transform.position, Vector2.right*direction*moveDir, rayDistanceHorizontal, playerLayer);
            goblinRigidBody.linearVelocityX = (hitPlayer) ? 0 : speed*direction*moveDir;
        }
    }

    void ShortStop()
    {
        move = MovementStates.idle;
        goblinRigidBody.linearVelocityX = 0;
        timeTillNextMove = Time.time + shortMoveCooldown;
    }

    void AttemptAttack()
    {
        int hit = Random.Range(0, 3);
        if (hit == 0)
        {
            //This initiates an attack
            move = MovementStates.attacking;
            FindDirection(target.position.x);
            timeTillAttack = Time.time + attackTelegraph;
        }
        else
        {
            timeTillCanAttack = Time.time + attackCooldown;
        }
    }

    void AttackSequence()
    {
        //Attack stuff
        if (Time.time > timeTillAttack + attackDuration)
        {
            move = MovementStates.idle;
            timeTillNextMove = Time.time;
            timeTillCanAttack = Time.time + attackCooldown;
        }
        else if (Time.time > timeTillAttack)
        {
            sprite.color = new Color(0f, 1f, 0.2f);
            PreformAttack();
        }
        else
        {
            timeTillChill = 0f;
            sprite.color = new Color(0f, 0.6f, 0.2f);
        }
    }

    void PreformAttack()
    {
        Vector2 finalAttackPosition = transform.position;
        finalAttackPosition += new Vector2(direction*1.15f, 0);
        Collider2D hitPlayer = Physics2D.OverlapBox(finalAttackPosition, hitBox, 0, playerLayer);
        StartCoroutine(Attack(finalAttackPosition));
        if (hitPlayer)
        {
            PlayerController player = hitPlayer.gameObject.GetComponent<PlayerController>();
            player.DamagePlayer(strength, direction);
        }
    }
    IEnumerator Attack(Vector2 attackPosition)
    {
        attackVisual.transform.position = attackPosition;
        attackVisual.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        attackVisual.SetActive(false);
    }
}

