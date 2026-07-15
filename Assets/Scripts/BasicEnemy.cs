using UnityEngine;
using System.Collections.Generic;

public class BasicEnemy : MonoBehaviour
{
    [SerializeField] protected int hp;
    [SerializeField] protected int strength;
    [SerializeField] protected int money;

    public virtual void gainMoney(int moneyGained)
    {
        CurrentData.Instance.playerMoney += moneyGained;
    }
}
