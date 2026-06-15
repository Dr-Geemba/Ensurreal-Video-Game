using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    protected const float iFrames = 1f;
    public virtual void dealDamage(int damageDelt)
    {
        CurrentData.Instance.playerHealth -= damageDelt;
    }

    public virtual void gainMoney(int moneyGained)
    {
        CurrentData.Instance.playerMoney += moneyGained;
    }
}
