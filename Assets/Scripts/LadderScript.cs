using Unity.VisualScripting;
using UnityEngine;

public class LadderScript : MonoBehaviour
{
    //Wheee, note: ladders have x scale of 0.7!
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Snaps player to middle of ladder and puts the player in the climbing state if w clicked
            float ladderCenter = GetComponent<Collider2D>().bounds.center.x;
            PlayerController player = other.GetComponent<PlayerController>();
            //ADD TIME AFTER W TO NOT SNAP BACK TO LADDER->SMALL COOLDOWN
            if (Input.GetKey(KeyCode.W) && !player.isClimbing)
            {
                player.isClimbing = true;
                if (!player.markMode)
                {
                    Vector3 playerPos = other.transform.position;
                    other.transform.position = new Vector3(ladderCenter, playerPos.y, playerPos.z);
                }
            }
            //space gets off the ladder and ends climbing state
            //You can do or I can: ADD STOPCLIMB IF HIT BY ENEMY
            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                stopClimb(other);
            }
        }
    }

    void stopClimb(Collider2D other)
    {
        Rigidbody2D playerRigidbody = other.GetComponent<Rigidbody2D>();
        PlayerController player = other.GetComponent<PlayerController>();
        player.isClimbing = false;
        playerRigidbody.gravityScale = 1f;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //Also exits if you're off the ladder
        if (other.gameObject.CompareTag("Player"))
        {
            stopClimb(other);
        }
    }
}
