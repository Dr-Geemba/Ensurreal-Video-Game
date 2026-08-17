using UnityEngine;

public abstract class PlayerStates
{
    protected PlayerController player;
    //fix this.rb
    protected StateControllerTommy stateController;

    protected bool input {get => player.input; set => player.input = value;}
    protected Rigidbody2D rb {get => player.playerRigidbody; set => player.playerRigidbody = value;}
    protected Transform transform {get => player.transform; set => player.transform = value;}
    protected bool HasJumped {get => player.hasJumped; set => player.hasJumped = value;}
    protected Vector2 movement {get => player.movement; set => player.movement = value;}
    protected int strength {get => player.hitStrength; set => player.hitStrength = value;}

    public PlayerStates(StateControllerTommy stateController, PlayerController player)
    {
        this.stateController = stateController;
        this.player = player;
    }

   public virtual void Enter() {}
   public virtual void Exit() {}
   public virtual void Update() {player.UpdateInput();}
   public virtual void FixedUpdate() {}
}
