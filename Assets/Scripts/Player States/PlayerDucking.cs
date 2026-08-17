using UnityEngine;

public class PlayerDucking : PlayerStates
{
    public PlayerDucking(StateControllerTommy stateController, PlayerController player) : base(stateController, player) {}

    public override void Enter()
    {
        Debug.Log("Duck");
        input = true;
        player.collider.size = new Vector2(1, 1);
        player.collider.offset = new Vector2(player.collider.offset.x, -0.5f);
    }

    public override void Update()
    {
        base.Update();

        if ((Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))) stateController.ChangeState(player.idleState);
        else if (!player.isGrounded) stateController.ChangeState(player.fallState);
        else if ((Input.GetButton("Deploy")) && (player.CanDeploy())) player.stateController.ChangeState(player.deployState);
    }

    public override void Exit()
    {
        player.collider.size = new Vector2(1, 2);
        player.collider.offset = new Vector2(player.collider.offset.x, 0f);
    }
}
