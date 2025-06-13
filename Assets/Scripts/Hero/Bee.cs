using System.Collections;
using UnityEngine;

public class Bee : MonoBehaviour
{
    public HeroData heroData;                
    public GameObject bulletPrefab;           

    private Animator animator;
    private float attackCooldown;

    private void Start()
    {
        animator = GetComponent<Animator>();
        attackCooldown = heroData.attackSpeed;
    }

    private void Update()
    {
        attackCooldown -= Time.deltaTime;

        GameObject target = FindNearestEnemy();

        if (target != null)
        {
            if (attackCooldown <= 0f)
            {
                animator.SetTrigger("Attack");
                StartCoroutine(ShootAfterDelay(target)); 
                attackCooldown = heroData.attackSpeed;
            }
        }
    }

    private IEnumerator ShootAfterDelay(GameObject target)
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 dir = (target.transform.position - transform.position).normalized;
        GameObject bullet = PoolManager.instance.GetBullet(transform.position);
        bullet.GetComponent<Bullet>().SetTarget(target.transform);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * 5f;
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDist = Mathf.Infinity;
        GameObject nearest = null;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist && dist <= heroData.range)
            {
                minDist = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }
}
