using System.Collections;
using UnityEngine;

public class ExplosionBullet : MonoBehaviour
{
    public float speed = 6f;
    public float damage;
    public float radius;

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
        // 폭발 중이면 이동 중단
        if (isExploding) return;

        // 목표 방향으로 직진
        Vector3 dir = (targetPos - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // 목표 근처 도달 시 폭발
        if (Vector3.Distance(transform.position, targetPos) < 0.2f)
            Explode();
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

        // 애니 길이만큼 기다렸다가 반납
        float boomDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        StartCoroutine(ReleaseAfter(boomDuration));
    }

    private IEnumerator ReleaseAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        PoolManager.instance.ReleaseExplosionBullet(gameObject);
    }

    // 씬 뷰에서 폭발 반경 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
