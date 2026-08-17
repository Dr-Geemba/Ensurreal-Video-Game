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
            if (player.movement.y == 1f && player.stateController.currentState != player.climbState)
            {
                player.stateController.ChangeState(player.climbState);
                other.transform.position = StartClimb(other.transform.position, ladderCenter);
            }
            //space gets off the ladder and ends climbing state
            //You can do or I can: ADD STOPCLIMB IF HIT BY ENEMY
        }
    }

    Vector3 StartClimb(Vector3 playerPos, float ladderCenter)
    {
        return new Vector3(ladderCenter, playerPos.y, playerPos.z);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //Also exits if you're off the ladder
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            Rigidbody2D playerRigid = other.GetComponent<Rigidbody2D>();
            player.stateController.ChangeState(player.fallState);
        }
    }
}
