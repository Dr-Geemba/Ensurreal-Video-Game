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
            Rigidbody2D playerRigid = other.GetComponent<Rigidbody2D>();
            //ADD TIME AFTER W TO NOT SNAP BACK TO LADDER->SMALL COOLDOWN
            if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && !player.isClimbing)
            {
                player.isClimbing = true;
                playerRigid.linearVelocity = Vector3.zero;
                if (!player.markMode)
                {
                    Vector3 playerPos = other.transform.position;
                    other.transform.position = new Vector3(ladderCenter, playerPos.y, playerPos.z);
                }
            }
            //space gets off the ladder and ends climbing state
            //You can do or I can: ADD STOPCLIMB IF HIT BY ENEMY
            if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space))
            {
                stopClimb(playerRigid, player);
            }
        }
    }

    void stopClimb(Rigidbody2D playerRigid, PlayerController player)
    {
        player.isClimbing = false;
        playerRigid.gravityScale = 1f;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //Also exits if you're off the ladder
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            Rigidbody2D playerRigid = other.GetComponent<Rigidbody2D>();
            stopClimb(playerRigid, player);
        }
    }
}
