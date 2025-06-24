using UnityEngine;

public class BlastBullet : MonoBehaviour
{
    public float speed;
    private Vector3 dir;
    private float damage;
    private float life = 2f;

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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        var enemy = other.GetComponent<Enemy>();
        if (enemy != null)
            enemy.GetDamage(damage);

        Vector3 worldPos = other.transform.position + Vector3.up * 0.5f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        var txt = PoolManager.instance.GetDamageText(Vector3.zero);
        var canvas = GameObject.Find("Canvas_MainUI").transform;
        txt.transform.SetParent(canvas, false);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPos, null, out Vector2 localPos);
        float xOffset = 150f;
        txt.GetComponent<RectTransform>().anchoredPosition = localPos + new Vector2(-xOffset, 0);
        txt.GetComponent<DamageText>().Show(damage);

        PoolManager.instance.ReleaseBlastBullet(gameObject);
    }
}
