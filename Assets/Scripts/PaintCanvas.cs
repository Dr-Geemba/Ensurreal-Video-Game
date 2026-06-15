using UnityEngine;

public class PaintCanvas : MonoBehaviour, IDamageable
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask playerAttackLayer;
    private Collider2D canvasCollider2D;
    private int hp = 40;
    private int timesHit = 0;
    void Start()
    {
        canvasCollider2D = GetComponent<Collider2D>();
        LayerMask collisionLayers = groundLayer | platformLayer | playerAttackLayer;
        if(canvasCollider2D != null)
        {
            canvasCollider2D.excludeLayers = ~collisionLayers;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack") && CurrentData.Instance.playerMana >= 3 && timesHit < 3)
        {
            CurrentData.Instance.playerMana -= 3;
            timesHit += 1;
        }
        if(timesHit == 3)
        {
            if(CurrentData.Instance.playerHealth < CurrentData.Instance.maxHealth)
            {
                CurrentData.Instance.playerHealth += 1;
                timesHit += 1;
            }
        }
        if(hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void TakeDamage(int damage)
    {
        hp -= damage;
    }
}
