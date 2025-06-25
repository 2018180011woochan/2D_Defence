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
            Explode();
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
        col2d.enabled = false;              
        animator.SetTrigger("Boom");         

        // 스플래시 데미지 즉시
        var hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;
            var enemy = hit.GetComponent<Enemy>();
            if (enemy == null) continue;

            enemy.GetDamage(damage);

            Vector3 worldPos = hit.transform.position + Vector3.up * 0.5f;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            GameObject txt = PoolManager.instance.GetDamageText(Vector3.zero);
            var canvas = GameObject.Find("Canvas_MainUI").transform;
            txt.transform.SetParent(canvas, false);

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenPos, null, out Vector2 localPos);

            var rt = txt.GetComponent<RectTransform>();
            float xOffset = 150f; 
            rt.anchoredPosition = localPos + new Vector2(-xOffset, 0f);

            txt.GetComponent<DamageText>().Show(damage);
        }
    }
}
