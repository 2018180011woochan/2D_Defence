using TMPro;
using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    public float speed = 8f;
    private Transform target;
    private float damage;

    private void OnEnable()
    {
        Invoke(nameof(AutoRelease), 0.4f);
    }


    private void AutoRelease()
    {
        PoolManager.instance.ReleaseBullet(gameObject);
    }

    public void SetTarget(Transform t, float heroDamage)
    {
        target = t;
        damage = heroDamage;
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
        if (!collision.CompareTag("Enemy")) return;

        float minDmg = damage - 10f;
        float maxDmg = damage + 10f;

        float randDamage = Random.Range(minDmg, maxDmg);
        damage = randDamage;

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.GetDamage(damage);
            OnHit(enemy);
        }

        Vector3 worldPos = collision.transform.position + Vector3.up * 0.5f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        GameObject damageTextObj = PoolManager.instance.GetDamageText(Vector3.zero);

        Transform canvasTransform = GameObject.Find("Canvas_MainUI").transform;
        damageTextObj.transform.SetParent(canvasTransform, false);

        RectTransform canvasRect = canvasTransform.GetComponent<RectTransform>();
        RectTransform damageRect = damageTextObj.GetComponent<RectTransform>();

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, null, out localPos
        );

        // 위치 보정
        float xOffset = 150f;
        damageRect.anchoredPosition = localPos + new Vector2(-xOffset, 0);

        damageTextObj.GetComponent<DamageText>().Show(damage);

        PoolManager.instance.ReleaseBullet(gameObject);

    }

    protected virtual void OnHit(Enemy enemy) { }
}
