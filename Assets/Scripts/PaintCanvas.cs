using System;
using UnityEngine;

public class PaintCanvas : MonoBehaviour, IDamageable
{
    private SpriteRenderer sprite;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask playerAttackLayer;
    private Collider2D canvasCollider2D;
    private int hp = 40;
    private int timesHit = 0;
    void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
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
            sprite.color = new Color(1f, 1f-(timesHit*0.3f), 1f-(timesHit*0.3f));
        }
        if(timesHit == 3)
        {
            if(CurrentData.Instance.playerHealth < CurrentData.Instance.maxHealth)
            {
                CurrentData.Instance.playerHealth = System.Math.Ceiling(CurrentData.Instance.playerHealth)+1m;
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
