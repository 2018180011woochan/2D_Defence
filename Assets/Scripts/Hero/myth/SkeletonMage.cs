using System.Collections;
using UnityEngine;

public class SkeletonMage : BaseHero
{
    public GameObject hitEffectPrefab;

    public float hitDelay = 1.0f;

    [Header("스킬1: 아군 버프")]
    [Range(0, 1)] public float buffChance = 0.2f;          // 발동 확률 
    public int buffMultiplier = 2;                   // 공격력 배수
    public float buffDuration = 3f;                     // 버프 지속 시간
    //public GameObject buffEffectPrefab;                 // 버프 이펙트 프리팹
    public float buffAnimDelay = 2f;

    [Header("스킬2: 스턴 존")]
    [Range(0f, 1f)] public float stunChance = 0.2f;         // 20%
    public GameObject stunZonePrefab;                       // 스턴 존 표시 이펙트 (3초 유지)
    public float stunDuration = 3f;                         // 3초
    public float stunAnimDelay = 2f;

    public AudioClip BoneSound;
    public AudioClip BuffSound;
    public AudioClip StunSound;

    protected override void Start()
    {
        base.Start();
        SFXManager.instance.PlaySFX(BoneSound);
    }
    protected override IEnumerator ShootAfterDelay(GameObject target)
    {
        if (target != null)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            float sign = dir.x < 0 ? 1f : -1f;
            transform.localScale = new Vector3(_origScale.x * sign,
                                               _origScale.y,
                                               _origScale.z);
        }

        float r = Random.value;
        if (r < buffChance)
        {
            animator.SetTrigger("BuffTrigger");
            SFXManager.instance.PlaySFX(BuffSound);
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
                        buffMultiplier, buffDuration
                    );
                }
            }

            yield return new WaitForSeconds(buffAnimDelay);
            yield break;
        }
        else if (r < stunChance + buffChance)
        {
            animator.SetTrigger("StunTrigger");
            SFXManager.instance.PlaySFX(StunSound);
            Vector3 zonePos = target.transform.position;
            var zone = Instantiate(stunZonePrefab, zonePos, Quaternion.identity);
            yield return new WaitForSeconds(stunAnimDelay);
            // 3초 뒤 자동 제거
            Destroy(zone, stunDuration);
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
