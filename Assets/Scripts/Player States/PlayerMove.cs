using UnityEngine;

public class PlayerMove : PlayerStates
{
    public PlayerMove(StateControllerTommy stateController, PlayerController player) : base(stateController, player) {}

    public override void Enter()
    {
        Debug.Log("Move");
        input = true;
    }

    public override void Update()
    {
        base.Update();

        player.isFacingUp = (Input.GetButton("Up")) ? true : false;

        if (HasJumped) stateController.ChangeState(player.jumpState);
        else if (!player.isGrounded) stateController.ChangeState(player.fallState);
        else if ((Input.GetButton("Deploy")) && (player.CanDeploy())) player.stateController.ChangeState(player.deployState);
        else if ((Input.GetButton("Down"))) stateController.ChangeState(player.duckState);
        else if (movement.x == 0) stateController.ChangeState(player.idleState);
    }

    public override void FixedUpdate()
    {
        player.Movement();
    }

    public override void Exit()
    {

    }
}
