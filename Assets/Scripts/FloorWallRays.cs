using UnityEngine;

public static class FloorWallRays
{
    public static readonly LayerMask groundLayer = LayerMask.GetMask("Ground", "Platform", "Breakable Wall");

    public static bool DrawRays(Vector3 center, float rayDistanceHorizontal, float rayDistanceVertical, int direction)
    {
        Vector3 ledgeCheckPos = center + new Vector3(rayDistanceHorizontal*direction,0,0);
        //Debug Lines
        Debug.DrawLine(center, ledgeCheckPos, Color.red);
        Debug.DrawLine(ledgeCheckPos, ledgeCheckPos + new Vector3(0, -rayDistanceVertical, 0), Color.green);

        RaycastHit2D hitWall = Physics2D.Raycast(center, Vector2.right*direction, rayDistanceHorizontal, groundLayer);
        RaycastHit2D hitFloor = Physics2D.Raycast(ledgeCheckPos, Vector2.down, rayDistanceVertical, groundLayer);
        return (hitWall || !hitFloor) ? true : false;
    }
}
