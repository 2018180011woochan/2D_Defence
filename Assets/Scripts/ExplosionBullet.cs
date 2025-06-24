using System.Collections;
using UnityEngine;

public class ExplosionBullet : MonoBehaviour
{
    public float speed = 6f;
    public float damage;
    public float radius;
    public float maxLifetime = 2f;
    private float lifeTimer;
    private Vector3 targetPos;
    private bool isExploding = false;
    private Animator animator;
    private Collider2D col2d;

    public void Init(Vector3 targetWorldPos, float dmg, float splashRadius)
    {
        targetPos = targetWorldPos;
        damage = dmg;
        radius = splashRadius;
        isExploding = false;
        lifeTimer = maxLifetime;

        // 콜라이더 · 렌더러 다시 활성화
        if (col2d == null) col2d = GetComponent<Collider2D>();
        col2d.enabled = true;

        // 방향 회전
        Vector3 dir = (targetPos - transform.position).normalized;
        float angle = Vector2.SignedAngle(Vector2.right, dir);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        gameObject.SetActive(true);
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        col2d = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (isExploding) return;

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            PoolManager.instance.ReleaseExplosionBullet(gameObject);
            return;
        }

        Vector3 dir = (targetPos - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
       if (isExploding) return;
       if (!other.CompareTag("Enemy")) return;
       Explode();
    }

    public void OnBoomEnd()
    {
        PoolManager.instance.ReleaseExplosionBullet(gameObject);
    }
    private void Explode()
    {
        isExploding = true;
        col2d.enabled = false;              // 추가 데미지 방지
        animator.SetTrigger("Boom");        // 애니메이터 내 “Boom” 트리거

        // 스플래시 데미지 즉시
        var hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
            if (hit.CompareTag("Enemy"))
                hit.GetComponent<Enemy>()?.GetDamage(damage);

    }
}
