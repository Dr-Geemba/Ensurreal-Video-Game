using UnityEngine;

public class Chest : MonoBehaviour, IDamageable
{
    private bool isDead = false;
    public float hp = 30f;
    public int money = 15;
    
    void Update()
    {
        if(hp <= 0)
        {
            CurrentData.Instance.playerMoney += money;
            isDead = true;
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
    }
}
