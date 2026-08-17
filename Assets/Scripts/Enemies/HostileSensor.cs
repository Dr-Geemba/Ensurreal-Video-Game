using UnityEngine;

public class HostileSensor : MonoBehaviour
{
    public enum ZoneType
    {
        AlertZone,
        StopHostileZone
    }
    [SerializeField] private ComplexEnemy enemyScript;
    [SerializeField] private ZoneType zone;
    [SerializeField] int ype;


    private void Awake()
    {
        enemyScript = GetComponentInParent<ComplexEnemy>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && zone == ZoneType.AlertZone)
        {
            enemyScript.EnterHostileRange(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && zone == ZoneType.StopHostileZone)
        {
            enemyScript.LeaveChillRange();
        }
    }
}
