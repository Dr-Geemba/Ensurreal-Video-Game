using System.Collections;
using UnityEngine;

public class GoblinEnemy : BasicEnemy, IDamageable
{
    private bool isPlayerClose = false;
    private bool isPlayerInAttackRange = false;
    private const int visionRadius = 8;
    private const float attackRadius = 1.5f;
    private const float turnDeadZone = 0.2f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform player;
    private const float goblinSpeed = 4;
    private Rigidbody2D goblinRigidBody;
    [SerializeField] private GameObject attackVisual;
    private int goblinAttackCooldown = 2;
    private float goblinTimeTillNextAttack = 0;
    private Vector2 hitBoxRadius = new Vector2(1.5f,1.5f);
    private Vector2 attackPoint;
    private int hp = 35;
    void Start()
    {
        goblinRigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isPlayerClose = Physics2D.OverlapCircle(gameObject.transform.position, visionRadius, playerLayer);
        Debug.Log(isPlayerClose);
        if(isPlayerClose == true)
        {
            isPlayerInAttackRange = Physics2D.OverlapCircle(gameObject.transform.position, attackRadius, playerLayer);
            Debug.Log(isPlayerInAttackRange);
        }
        if(hp <= 0)
        {
            Destroy(gameObject);
            gainMoney(10);
        }
    }
    void FixedUpdate()
    {
        if (isPlayerInAttackRange == false)
        {
            float xDistance = player.position.x - transform.position.x;
            
            if(Mathf.Abs(xDistance) > turnDeadZone)
            {
                if (player.position.x < transform.position.x)
                {
                    goblinRigidBody.linearVelocityX = -goblinSpeed;
                }
                else
                {
                    goblinRigidBody.linearVelocityX = goblinSpeed;

                }
            }
        }
        else
        {
            goblinRigidBody.linearVelocityX = 0;
            if(goblinTimeTillNextAttack < Time.time)
            {
                goblinTimeTillNextAttack = Time.time + goblinAttackCooldown;
                PreformAttack();
            }
        }
    }
    void PreformAttack()
    {
        Vector2 finalAttackPosition = transform.position;
        if(player.position.x < transform.position.x)
        {
            finalAttackPosition += Vector2.left;
        }
        else
        {
            finalAttackPosition += Vector2.right;
        }
        Collider2D hitPlayer = Physics2D.OverlapBox(finalAttackPosition, hitBoxRadius, 0, playerLayer);
        StartCoroutine(Attack(finalAttackPosition));
        if (hitPlayer)
        {
            dealDamage(1);
        }
    }
    IEnumerator Attack(Vector2 attackPosition)
    {
        attackVisual.transform.position = attackPosition;
        attackVisual.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        attackVisual.SetActive(false);
    }
    public void TakeDamage(int damage)
    {
        hp -= damage;
    }
}

