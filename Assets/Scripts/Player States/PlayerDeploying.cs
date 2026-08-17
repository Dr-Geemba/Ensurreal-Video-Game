using UnityEngine;

public class PlayerDeploying : PlayerStates
{
    public PlayerDeploying(StateControllerTommy stateController, PlayerController player) : base(stateController, player) {}

    private float deployTime = 1f;
    private float timeTillFinishDeploy = 0f;

    public override void Enter()
    {
        Debug.Log("Deploy");
        input = false;
        rb.linearVelocity = Vector3.zero;
        player.sprite.color = new Color(0.5f, 0f, 0.5f);

        timeTillFinishDeploy = Time.time + deployTime;
    }

    public override void Update()
    {
        if (Time.time > timeTillFinishDeploy)
        {
            //I've heard rumerrs on the web:
            //Deleting the canvas each time will fill up the trash
            //We should find a method to store and reuse them
            player.DeployCanvas();
            stateController.ChangeState(player.idleState);
        }
        else if (!player.isGrounded) stateController.ChangeState(player.fallState);
    }

    public override void FixedUpdate()
    {
        Debug.DrawLine(player.transform.position, player.canvasOffset, Color.red);
        Debug.DrawLine(player.canvasOffset, player.canvasOffset + new Vector3(0, -player.rayDistanceVertical, 0), Color.green);
    }

    public override void Exit()
    {
        player.sprite.color = new Color(1f, 0f, 0f);
    }
}
