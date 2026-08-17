using UnityEngine;

public class PlayerFalling : PlayerStates
{
    public PlayerFalling(StateControllerTommy stateController, PlayerController player) : base(stateController, player) {}

    private float maxFallSpeed = -8f;
    public override void Enter()
    {
        Debug.Log("Fall");
        input = true;
        player.releaseJump = false;
    }

    public override void Update()
    {
        base.Update();
        
        player.isFacingUp = (player.movement.y == 1) ? true : false;
        if (rb.linearVelocity.y < maxFallSpeed) rb.linearVelocityY = maxFallSpeed;

        if (player.isGrounded) stateController.ChangeState(player.idleState);
    }

    public override void FixedUpdate()
    {
        player.Movement();
    }
}
