using System.Collections;
using UnityEngine;

public class PhaseObject : MonoBehaviour
{
    private PlatformEffector2D effector;
    private Collider2D platformcollider;
    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
        platformcollider = GetComponent<Collider2D>();
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.S))
        {
            effector.rotationalOffset = 180;
        }
        else
        {
            effector.rotationalOffset = 0;
        }
    }
    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && Input.GetKey(KeyCode.S))
        {
            Collider2D playerCollider = other.gameObject.GetComponent<Collider2D>();
            StartCoroutine(NoTeleport(playerCollider));
        }
    }
    private IEnumerator NoTeleport(Collider2D playerCollider)
    {
        Physics2D.IgnoreCollision(playerCollider, platformcollider,true);
        while(playerCollider.bounds.min.y > platformcollider.bounds.max.y - 0.1f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        Physics2D.IgnoreCollision(playerCollider, platformcollider, false);

    }

}
