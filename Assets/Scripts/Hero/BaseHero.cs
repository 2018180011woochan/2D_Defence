using System.Collections;
using UnityEngine;

public abstract class BaseHero : MonoBehaviour
{
    public HeroData heroData;

    protected Animator animator;
    protected float attackCooldown;
    protected SpriteRenderer spriteRenderer;


    public bool isAttackBuff = false;
    protected float buffTimer = 0f;                        // 남은 버프 시간
    private int originalAttack;                          // 원래 공격력
    private GameObject buffEffectInstance;                 // 버프 이펙트 인스턴스

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackCooldown = heroData.attackSpeed;

        originalAttack = heroData.attack;
    }

    protected virtual void Update()
    {
        // 스켈레톤 법사에 의해 버프가 걸려잇을 때
        if (isAttackBuff)
        {
            buffTimer -= Time.deltaTime;
            if (buffTimer <= 0f)
            {
                // 버프 해제
                isAttackBuff = false;
                heroData.attack = originalAttack;

                if (buffEffectInstance != null)
                    Destroy(buffEffectInstance);
            }
        }

        attackCooldown -= Time.deltaTime;

        GameObject target = FindNearestEnemy();
        if (target == null) return;

        if (attackCooldown <= 0f)
        {
            animator.SetTrigger("Attack");
            StartCoroutine(ShootAfterDelay(target));
            attackCooldown = heroData.attackSpeed;
        }
    }

    public void ApplyAttackBuff(int multiplier, float duration, GameObject effectPrefab, Vector3 effectPos)
    {
        // 이미 버프 중이면 타이머만 리셋하고 이펙트 교체
        if (isAttackBuff)
        {
            buffTimer = duration;
            if (buffEffectInstance != null)
                Destroy(buffEffectInstance);
        }
        else
        {
            isAttackBuff = true;
            heroData.attack *= multiplier;
            buffTimer = duration;
        }

        // 버프 이펙트
        if (effectPrefab != null)
            buffEffectInstance = Instantiate(effectPrefab, effectPos, Quaternion.identity);
    }


    protected abstract IEnumerator ShootAfterDelay(GameObject target);

    protected GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDist = Mathf.Infinity;
        GameObject nearest = null;

        foreach (var e in enemies)
        {
            if (!e.activeInHierarchy)
                continue;

            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < minDist && d <= heroData.range)
            {
                minDist = d;
                nearest = e;
            }
        }
        return nearest;
    }
}
