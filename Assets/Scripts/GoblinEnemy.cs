using UnityEngine;

public class GoblinEnemy : BasicEnemy
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
        }
    }
}
