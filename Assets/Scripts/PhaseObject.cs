using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PhaseObject : MonoBehaviour
{
    //MAKE isGrounded true TO FALL, SO THAT YOU CAN'T PRESS IT EARLY TO FALL THROUGH
    //ADD DOWNWARD FORCE SO WE CAN DECREASE WaitForSeconds AND OTHER GAMES DO IT
    private PlatformEffector2D effector;
    private LayerMask ogMask;
    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
        ogMask = effector.colliderMask;
    }
    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            Rigidbody2D playerRigid = other.gameObject.GetComponent<Rigidbody2D>();
            if ((player.stateController.currentState == player.duckState) && (Input.GetButton("Jump")))
            {
                StartCoroutine(Drop());
                playerRigid.AddForce(Vector3.down * 3f, ForceMode2D.Impulse);
            }
        }
    }
    private IEnumerator Drop()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        effector.colliderMask &= ~(1 << playerLayer);
        yield return new WaitForSeconds(0.2f);
        effector.colliderMask = ogMask;
    }
}