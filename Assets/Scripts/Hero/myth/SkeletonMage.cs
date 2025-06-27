using System.Collections;
using UnityEngine;

public class SkeletonMage : BaseHero
{
    public GameObject hitEffectPrefab;

    public float hitDelay = 1.0f;

    [Header("스킬1: 아군 버프")]
    [Range(0, 1)] public float buffChance = 0.2f;          // 발동 확률 
    public int buffMultiplier = 2;                   // 공격력 배수
    public float buffDuration = 3.0f;                     // 버프 지속 시간
    public GameObject buffEffectPrefab;                 // 버프 이펙트 프리팹
    public float buffAnimDelay = 2.0f;

    protected override IEnumerator ShootAfterDelay(GameObject target)
    {
        // 1) 스킬 발동 판정
        if (Random.value < buffChance)
        {
            animator.SetTrigger("BuffTrigger");  

            int heroLayerMask = LayerMask.GetMask("Hero");
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position,
                heroData.range,
                heroLayerMask);

            foreach (var col in hits)
            {
                var allyGO = col.gameObject;
                var ally = allyGO.GetComponent<BaseHero>();
                if (ally != null)
                {
                    Vector3 effectPos = ally.transform.position;
                    ally.ApplyAttackBuff(
                        buffMultiplier,
                        buffDuration,
                        buffEffectPrefab,
                        effectPos
                    );
                }
            }

            yield return new WaitForSeconds(buffAnimDelay);
            yield break;
        }

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(hitDelay);

        if (target != null && target.activeInHierarchy)
        {
            if (hitEffectPrefab != null)
            {
                Instantiate(
                    hitEffectPrefab,
                    target.transform.position,
                    Quaternion.identity
                );
            }

            var enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.GetDamage(heroData.attack);
                UIManager.instance.ShowDamageTMP(
                    (int)heroData.attack,
                    target
                );
            }
        }

    }
}
