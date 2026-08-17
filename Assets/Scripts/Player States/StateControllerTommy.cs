using UnityEngine;

public class StateControllerTommy
{
    public PlayerStates currentState;

    public void StartState(PlayerStates newState)
    {
        currentState = newState;
        currentState.Enter();
    }

    public void ChangeState(PlayerStates newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
