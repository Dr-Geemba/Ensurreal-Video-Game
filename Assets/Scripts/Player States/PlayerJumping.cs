using UnityEngine;

public class PlayerJumping : PlayerStates
{
    public PlayerJumping(StateControllerTommy stateController, PlayerController player) : base(stateController, player) {}

    private float jumpHeight = 2.5f;
    private float initHeight;

    public override void Enter()
    {
        Debug.Log("Jump");
        input = true;
        HasJumped = false;
        rb.linearVelocity = Vector3.zero;
        initHeight = transform.position.y;
    }

    public override void Update()
    {
        base.Update();

        player.isFacingUp = (player.movement.y == 1) ? true : false;

        if (player.releaseJump)
        {
            rb.AddForce(Vector3.up * 0f, ForceMode2D.Impulse);
        }
        else
        {
            if (Input.GetButtonUp("Jump") || transform.position.y >= initHeight + jumpHeight) player.releaseJump = true;
            rb.linearVelocityY = player.jumpForce;
        }
        if (rb.linearVelocityY < 0)
        {
            stateController.ChangeState(player.fallState);
        }
    }

    public override void FixedUpdate()
    {
        player.Movement();
    }
}
