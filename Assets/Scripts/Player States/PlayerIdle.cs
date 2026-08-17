using UnityEngine;

public class PlayerIdle : PlayerStates
{
    public PlayerIdle(StateControllerTommy stateController, PlayerController player) : base(stateController, player) {}

    public override void Enter()
    {
        Debug.Log("Idle");
        input = true;
        rb.linearVelocity = Vector3.zero;
    }

    public override void Update()
    {
        base.Update();
        
        player.isFacingUp = (player.movement.y == 1) ? true : false;

        if (HasJumped)
            stateController.ChangeState(player.jumpState);
        else if (!player.isGrounded)
            stateController.ChangeState(player.fallState);
        else if ((Input.GetButton("Deploy")) && (player.CanDeploy()))
            stateController.ChangeState(player.deployState);
        else if ((Input.GetButton("Down")))
            stateController.ChangeState(player.duckState);
        else if (player.movement.x != 0)
            stateController.ChangeState(player.moveState);
    }

    public override void Exit()
    {

    }
}