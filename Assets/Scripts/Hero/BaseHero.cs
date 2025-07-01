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
    public GameObject AttackbuffEffectPrefab;    // Inspector 에 연결
    private GameObject buffEffectInstance;                 // 버프 이펙트 인스턴스

    protected Vector3 _origScale;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackCooldown = heroData.attackSpeed;

        originalAttack = heroData.attack;
        _origScale = transform.localScale;
    }

    protected virtual void Update()
    {
        if (GameManager.instance.isGameOver) return;

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

    public void ApplyAttackBuff(float multiplier, float duration)
    {
        if (buffEffectInstance != null)
            Destroy(buffEffectInstance);

        buffEffectInstance = Instantiate(
            AttackbuffEffectPrefab,
            transform.position,
            Quaternion.identity
        );

        buffEffectInstance.transform.localScale = Vector3.one;

        buffEffectInstance.transform.SetParent(transform, true);

        isAttackBuff = true;
        buffTimer = duration;
        heroData.attack = Mathf.RoundToInt(originalAttack * multiplier);

        StartCoroutine(RemoveBuffEffectAfter(duration));
    }

    private IEnumerator RemoveBuffEffectAfter(float t)
    {
        yield return new WaitForSeconds(t);
        if (buffEffectInstance != null)
            Destroy(buffEffectInstance);
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
