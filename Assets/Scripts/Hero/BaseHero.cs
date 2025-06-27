using System.Collections;
using UnityEngine;

public abstract class BaseHero : MonoBehaviour
{
    public HeroData heroData;

    protected Animator animator;
    protected float attackCooldown;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackCooldown = heroData.attackSpeed;
    }

    protected virtual void Update()
    {
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
