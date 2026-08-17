using UnityEngine;
using System.Collections.Generic;

public class PlayerHurt : PlayerStates
{
    public PlayerHurt(StateControllerTommy stateController, PlayerController player) : base(stateController, player) {}

    private float stunDuration = 0.3f;
    private float timeTillUnstun;

    private Dictionary<int, decimal> damage = new Dictionary<int, decimal>
    {
        [0] = 0.2m,
        [1] = 1m,
        [2] = 2m,
        [3] = 3m,
        [4] = 5m,
        [5] = 8m,
    };

    public override void Enter()
    {
        Debug.Log("Ouchies");
        input = false;

        //Knockback
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(new Vector2(player.hitDirection * 2f, 2f), ForceMode2D.Impulse);

        //DAMAGE SYSTEM! Enemies have a strength number instead of a certain damage output
        //With this, we can give buffs/debuffs to enemies to change how much damage they do and stuff
        if (strength > 5)
        {
            CurrentData.Instance.playerHealth = 0;
        }
        else if (strength >= 0)
        {
            if (strength != 0)
            {
                CurrentData.Instance.playerHealth = System.Math.Ceiling(CurrentData.Instance.playerHealth);
            }
            CurrentData.Instance.playerHealth -= damage[strength];
        }
        player.timeTillDamageable = Time.time + player.iFrames;
        timeTillUnstun = Time.time + stunDuration;
        player.sprite.color = new Color(0.5f, 0f, 0f);
    }

    public override void Update()
    {
        if (Time.time > timeTillUnstun) stateController.ChangeState(player.fallState);
    }

    public override void Exit()
    {
        
    }
}
