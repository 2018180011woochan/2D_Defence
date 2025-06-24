using UnityEngine;

public class BlastBullet : MonoBehaviour
{
    public float speed;
    private Vector3 dir;
    private float damage;
    private float life = 1f;

    public void Init(Vector3 direction, float dmg)
    {
        dir = direction.normalized;
        damage = dmg;
        life = 1f;
        transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir));
        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
        life -= Time.deltaTime;
        if (life <= 0f) PoolManager.instance.ReleaseBlastBullet(gameObject);
    }

}
