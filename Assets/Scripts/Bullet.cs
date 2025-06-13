using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 8f;
    private Transform target;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    private void Update()
    {
        if (target == null)
        {
            PoolManager.instance.ReleaseBullet(gameObject);
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && collision.transform == target)
        {
            PoolManager.instance.ReleaseBullet(gameObject);
        }
    }
}
