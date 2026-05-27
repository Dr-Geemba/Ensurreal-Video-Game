using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PhaseObject : MonoBehaviour
{
    private PlatformEffector2D effector;
    private LayerMask ogMask;
    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
        ogMask = effector.colliderMask;
    }
    void Update()
    {
        if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space)))
        {
            StartCoroutine(Drop());
        }
    }
    private IEnumerator Drop()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        effector.colliderMask &= ~(1 << playerLayer);
        yield return new WaitForSeconds(0.4f);
        effector.colliderMask = ogMask;
    }
}