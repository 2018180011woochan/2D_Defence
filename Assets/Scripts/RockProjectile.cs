using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    float baseAttack;
    float multiplier;
    public GameObject impactEffectPrefab;
    float areaRange;

    public void Init(float baseAttack, float multiplier, float areaRange)
    {
        this.baseAttack = baseAttack;
        this.multiplier = multiplier;
        this.areaRange = areaRange;

        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // 타겟(또는 바닥)에 부딪히면
        if (!col.CompareTag("Enemy")) return;

        if (impactEffectPrefab != null)
            Instantiate(impactEffectPrefab,
                        transform.position,
                        Quaternion.identity);

        // 2) 범위 데미지
        var hits = Physics2D.OverlapCircleAll(transform.position, areaRange);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;
            float dmg = baseAttack * multiplier;
            hit.GetComponent<Enemy>()?.GetDamage(dmg);
            UIManager.instance.ShowDamageTMP((int)dmg, hit.gameObject);
        }

        Destroy(gameObject);
    }
}
