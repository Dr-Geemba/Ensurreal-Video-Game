using Unity.VisualScripting;
using UnityEngine;

public class LadderScript : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            float ladderCenter = GetComponent<Collider2D>().bounds.center.x;
            PlayerController climber = other.GetComponent<PlayerController>();
            if (Input.GetKey(KeyCode.W))
            {
                climber.isClimbing = true;
                Vector3 playerPos = other.transform.position;
                other.transform.position = new Vector3(ladderCenter, playerPos.y, playerPos.z);
            }
            if (Input.GetKey(KeyCode.Space))
            {
                stopClimb(other);
            }
        }
    }

    void stopClimb(Collider2D other)
    {
        Rigidbody2D playerRigidbody = other.GetComponent<Rigidbody2D>();
        PlayerController climber = other.GetComponent<PlayerController>();
        climber.isClimbing = false;
        playerRigidbody.gravityScale = 1f;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            stopClimb(other);
        }
    }
}
