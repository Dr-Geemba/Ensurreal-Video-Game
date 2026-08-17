using UnityEngine;
using System.Collections.Generic;

public class BasicEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] protected int hp;
    [SerializeField] protected int strength;
    [SerializeField] protected int money;
    [SerializeField] protected int direction = -1;
    [SerializeField] protected float speed;
    [SerializeField] protected float movementLength;
    [SerializeField] protected float rayDistanceHorizontal;
    [SerializeField] protected float rayDistanceVertical;

    protected virtual void gainMoney(int moneyGained)
    {
        CurrentData.Instance.playerMoney += moneyGained;
    }

    public virtual void TakeDamage(int damage)
    {
        hp -= damage;
    }

    protected virtual void PreSpriteDeath()
    {
        if(hp <= 0)
        {
            Destroy(gameObject);
            gainMoney(money);
        }
    }
}
