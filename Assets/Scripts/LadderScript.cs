using Unity.VisualScripting;
using UnityEngine;

public class LadderScript : MonoBehaviour
{
    private const float climbSpeed = 4f;
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRigidbody = other.gameObject.GetComponent<Rigidbody2D>();
            playerRigidbody.gravityScale = 0f;
            float verticalInput = Input.GetAxisRaw("Vertical");

            // Set the velocity directly for smooth movement
            playerRigidbody.linearVelocity = new Vector2(playerRigidbody.linearVelocity.x, verticalInput * climbSpeed);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRigidbody = other.gameObject.GetComponent<Rigidbody2D>();
            playerRigidbody.gravityScale = 1f;
        }
    }
}
