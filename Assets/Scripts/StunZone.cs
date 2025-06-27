using UnityEngine;

public class StunZone : MonoBehaviour
{
    public float stunDuration = 3f;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            var enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Stun(stunDuration);
            }
        }
    }
}
