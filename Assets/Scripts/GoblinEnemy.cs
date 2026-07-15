using System.Collections;
using UnityEngine;

public class GoblinEnemy : BasicEnemy, IDamageable
{
    //Add time after which goblin goes back
    //Add ledges/walls
    //Add stop before player
    private SpriteRenderer sprite;
    //I'm going to add an aggro system or something later where the enemies can switch targets to the canvas or other npcs.
    //It will be more complicated so I'm not going to implement it yet.
    private bool hostile = false;
    //This decides if enemy has been idling too much and makes them return if the y pos is the same (they are on same platform)
    private float timeTillChill = 0f;
    private float angryDuration = 6f;
    private bool moveBack;
    //Maybe later add a smaller range box if goblin stops being hostile due to timeTillChill if you're still too close but not that close
    private Vector2 alertBox = new Vector2(12f, 4f);
    private Vector3 startPos;
    private float startMoveX;
    [SerializeField] private int direction = -1;
    private float moveCooldown = 1f;
    private float shortMoveCooldown = 0.2f;
    private float timeTillNextMove = 0f;
    private float attackCooldown = 1f;
    private float timeTillCanAttack = 0f;
    private float attackTelegraph = 0.8f;
    private float attackDuration = 0.4f;
    private float timeTillAttack = 0f;
    //Long distance is used to determine if the goblin stops being aggresive
    //Others are for determine what move the goblin makes next
    private const float longDistance = 12f;
    private const float midDistance = 6f;
    private const float shortDistance = 3f;
    //If you want to turn this back on, just let Dean know
    //private const float turnDeadZone = 0.2f;
    [SerializeField] private const float speed = 2.5f;
    private float movementLength = 3f/speed;
    private float movementTime = 0f;
    private const float rayDistanceVertical = 0.85f;
    private const float rayDistanceHorizontal = 0.6f;
    //Move dictates what the goblin is doing: 0 = Idle, 1 = Forwards, 2 = Backwards, 3 = Attacking
    private int move = 0;
    private Vector2 hitBox = new Vector2(1.3f, 1.2f);
    private Vector2 attackBox = new Vector2(1.3f, 1.7f);
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D goblinRigidBody;
    [SerializeField] private GameObject attackVisual;

    void Start()
    {
        startPos = transform.position;
        goblinRigidBody = GetComponent<Rigidbody2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!hostile)
        {
            hostile = Physics2D.OverlapBox(gameObject.transform.position, alertBox, 0, playerLayer);
            if (hostile)
            {
                timeTillChill = 0;
            }
        }
        else
        {
            if (!Physics2D.OverlapCircle(gameObject.transform.position, longDistance, playerLayer) || (Time.time > timeTillChill && !(timeTillChill == 0f)))
            {
                hostile = false;
                moveBack = true;
            }
            else
            {
                if (Time.time > timeTillNextMove && move == 0)
                {
                    ChooseMove();
                }
                if (Physics2D.OverlapBox(gameObject.transform.position, attackBox, 0, playerLayer) && Time.time > timeTillCanAttack)
                {
                    int hit = Random.Range(0, 3);
                    if (hit == 0)
                    {
                        //This initiates an attack
                        move = 3;
                        FindDirection(player.position.x);
                        timeTillAttack = Time.time + attackTelegraph;
                    }
                    else
                    {
                        timeTillCanAttack = Time.time + attackCooldown;
                    }
                }
            }
        }
        if(hp <= 0)
        {
            Destroy(gameObject);
            gainMoney(money);
        }
    }
    void FixedUpdate()
    {
        if (!hostile)
        {
            if (Mathf.Abs(startPos.y - gameObject.transform.position.y) < 0.1f && moveBack)
            {
                float distance = FindDirection(startPos.x);
                Debug.Log($"start {distance}, move {move}, vel {goblinRigidBody.linearVelocityX}");
                //I dunno if making distance = 0 will actually work
                if (!(Mathf.Abs(distance) < 0.1f))
                {
                    goblinRigidBody.linearVelocityX = speed*direction;
                }
                else
                {
                    moveBack = false;
                }
            }
        }
        else
        {
            Vector3 ledgeCheckPos = gameObject.transform.position + new Vector3(rayDistanceHorizontal*direction,0,0);
            Vector3 backLedgeCheckPos = gameObject.transform.position + new Vector3(-rayDistanceHorizontal*direction,0,0);
            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + new Vector3(rayDistanceHorizontal*direction, 0, 0), Color.red);
            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + new Vector3(-rayDistanceHorizontal*direction, 0, 0), Color.red);
            Debug.DrawLine(ledgeCheckPos, ledgeCheckPos + new Vector3(0, -rayDistanceVertical, 0), Color.green);
            Debug.DrawLine(backLedgeCheckPos, backLedgeCheckPos + new Vector3(0, -rayDistanceVertical, 0), Color.green);
            if (move == 0 || move == 3)
            {
                goblinRigidBody.linearVelocityX = 0;
                if (move == 3)
                {
                    //Attack stuff
                    if (Time.time > timeTillAttack + attackDuration)
                    {
                        move = 0;
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
                else
                {
                    FindDirection(player.position.x);
                }
            }
            else
            {
                if (Time.time > movementTime && Time.time > timeTillNextMove)
                {
                    ChooseMove();
                    if (!(move == 0))
                    {
                        int initialDir = direction;
                        FindDirection(player.position.x);
                        if (initialDir != direction)
                        {
                            ShortStop();
                        }
                    }
                }
                if (move == 1)
                {
                    RaycastHit2D hitFrontWall = Physics2D.Raycast(gameObject.transform.position, Vector2.right * direction, rayDistanceHorizontal, groundLayer | playerLayer);
                    RaycastHit2D hitFrontFloor = Physics2D.Raycast(ledgeCheckPos, Vector2.down, rayDistanceVertical, groundLayer);
                    if (hitFrontWall || !hitFrontFloor)
                    {
                        Chill();
                        ShortStop();
                    }
                    else
                    {
                        timeTillChill = 0f;
                        goblinRigidBody.linearVelocityX = speed*direction;
                    }
                }
                else if (move == 2)
                {
                    RaycastHit2D hitBackWall = Physics2D.Raycast(gameObject.transform.position, -Vector2.left * direction, rayDistanceHorizontal, groundLayer | playerLayer);
                    RaycastHit2D hitBackFloor = Physics2D.Raycast(backLedgeCheckPos, Vector2.down, rayDistanceVertical, groundLayer);
                    if (hitBackWall || !hitBackFloor)
                    {
                        Chill();
                        ShortStop();
                    }
                    else
                    {
                        timeTillChill = 0f;
                        goblinRigidBody.linearVelocityX = -speed*direction;
                    }
                }
            }
        }
    }

    float FindDirection(float target)
    {
        float distanceX = target - transform.position.x;
        if (distanceX != 0f)
        {
            direction = (Mathf.Abs(distanceX)/distanceX == -1) ? -1 : 1;
        }
        return distanceX;
    }

    void ChooseMove()
    {
        if (Physics2D.OverlapCircle(gameObject.transform.position, shortDistance, playerLayer))
        {
            int choice = Random.Range(0, 8);
            move = (choice < 5) ? 1 : 2;
        }
        else if (Physics2D.OverlapCircle(gameObject.transform.position, midDistance, playerLayer))
        {
            int choice = Random.Range(0, 6);
            move = (choice < 4) ? 1 : 0;
        }
        else
        {
            move = 1;
        }
        timeTillNextMove = Time.time + moveCooldown;
        if (move > 0 && move < 3)
        {
            movementTime = Time.time + movementLength;
        }
    }

    void ShortStop()
    {
        move = 0;
        goblinRigidBody.linearVelocityX = 0;
        timeTillNextMove = Time.time + shortMoveCooldown;
    }

    void Chill()
    {
        if (timeTillChill == 0f)
        {
            moveBack = true;
            timeTillChill = Time.time + angryDuration;
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
            player.DamagePlayer(strength);
        }
    }
    IEnumerator Attack(Vector2 attackPosition)
    {
        attackVisual.transform.position = attackPosition;
        attackVisual.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        attackVisual.SetActive(false);
    }
    public void TakeDamage(int damage)
    {
        hp -= damage;
    }
}

