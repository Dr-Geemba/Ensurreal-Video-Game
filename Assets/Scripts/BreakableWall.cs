using UnityEngine;

public class BreakableWall : MonoBehaviour, IDamageable
{
    private bool isDead = false;
    public float hp = 18f;

    void Update()
    {
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
