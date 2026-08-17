using UnityEngine;

public class ComplexEnemy : BasicEnemy
{
    protected bool hostile = false;
    protected bool moveBack;
    protected Transform target;
    protected Vector3 startPos;
    protected float chillAmount = 6f;
    protected float timeTillChill = 0f;
    protected enum MovementStates
    {
        idle,
        attacking,
        forwards,
        backwards
    }

    protected virtual void MoveBack(Transform center, Rigidbody2D rigidBody)
    {
        if (Mathf.Abs(startPos.y - center.position.y) < 0.1f)
        {
            float distance = FindDirection(startPos.x);
            //I dunno if making distance = 0 will actually work
            if (!(Mathf.Abs(distance) < 0.1f))
            {
                rigidBody.linearVelocityX = speed*direction;
            }
            else
            {
                moveBack = false;
            }
        }
    }

    //Converts move into a direction
    protected virtual int MoveToDir(MovementStates move)
    {
        return (move == MovementStates.forwards) ? 1 : -1;
    }

    protected virtual float FindDirection(float player)
    {
        float distanceX = player - transform.position.x;
        if (distanceX != 0f)
        {
            direction = (Mathf.Abs(distanceX)/distanceX == -1) ? -1 : 1;
        }
        return distanceX;
    }

    public virtual void EnterHostileRange(Transform player)
    {
        target = player.transform;
        FindDirection(target.position.x);
        hostile = true;
        timeTillChill = 0;
    }

    public virtual void LeaveChillRange()
    {
        hostile = false;
        moveBack = true;
    }
}
