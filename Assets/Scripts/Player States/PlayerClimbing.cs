using UnityEngine;

public class PlayerClimbing : PlayerStates
{
    public PlayerClimbing(StateControllerTommy stateController, PlayerController player) : base(stateController, player) {}

    public override void Enter()
    {
        Debug.Log("Climb");
        input = true;
        player.playerRigidbody.linearVelocity = Vector3.zero;
        player.playerRigidbody.gravityScale = 0f;
    }

    public override void Update()
    {
        base.Update();

        player.isFacingUp = (Input.GetButton("Up")) ? true : false;
        if (HasJumped)
        {
            if (Input.GetButton("Down"))
            {
                HasJumped = false;
                stateController.ChangeState(player.fallState);
            }
            else
            {
                stateController.ChangeState(player.jumpState);
            }
        }
        if (player.isGrounded && (Input.GetButton("Down"))) stateController.ChangeState(player.idleState);
    }

    public override void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, movement.y * player.speedClimb);
    }

    public override void Exit()
    {
        player.playerRigidbody.gravityScale = 1.2f;
    }
}