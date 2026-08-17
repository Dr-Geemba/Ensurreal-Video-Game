using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    public PlayerController playerCode;
    private Vector2 cameraPos;
    private Vector2 targetPos;
    private const float cameraSpeed = 15f;
    private bool lockedY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(player.transform.position.x, 2f, -10);
    }

    void FixedUpdate()
    {
        float direction = playerCode.direction;
        float xx = player.transform.position.x+direction;
        //if (!lockedY)
        //{
        float yy = player.transform.position.y+2f;
        //}
        targetPos = new Vector2(xx, yy);
        float distanceX = (targetPos.x-transform.position.x)/cameraSpeed;
        float distanceY = (targetPos.y-transform.position.y)/cameraSpeed;
        transform.position += new Vector3(distanceX, distanceY, 0);
    }
}
