using System.Collections;
using UnityEngine;

public class DragonWarior : BaseHero
{
    public float strikeChance = 0.9f;   
    public float strikeAnimDelay = 1.0f;                 
    public float blastDamageMultiplier = 10f;

    public float explosionChance = 0.1f;
    public float explosionAnimDelay = 0.5f;
    public float explosionDamageMultiplier = 30f;   
    public float explosionRadius = 2f;

    private float explosionSpawnYOffset = -1.3f;

    private Vector3 spawnPos;


    protected override void Start()
    {
        base.Start();

        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected override IEnumerator ShootAfterDelay(GameObject target)
    {
        spawnPos = transform.position + Vector3.up * explosionSpawnYOffset;
        float r = Random.value;

        Vector3 dir = (target.transform.position - transform.position).normalized;
        spriteRenderer.flipX = (dir.x < 0);

        if (r < 0.2f)
        {
            animator.SetTrigger("Strike");
            yield return new WaitForSeconds(strikeAnimDelay);

            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f;
                Vector3 dir8 = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad),
                    0f);

                GameObject blast = PoolManager.instance.GetBlastBullet(spawnPos);
                var bb = blast.GetComponent<BlastBullet>();
                bb.Init(dir8, heroData.attack * blastDamageMultiplier);

                var rb8 = blast.GetComponent<Rigidbody2D>();
                rb8.linearVelocity = dir8 * bb.speed;
            }

            yield break;
        }
        else if (r < 0.4f)
        {
            animator.SetTrigger("Explosion");
            yield return new WaitForSeconds(explosionAnimDelay);

            Vector3 targetPos = target.transform.position;
            GameObject expl = PoolManager.instance.GetExplosionBullet(spawnPos);
            expl.GetComponent<ExplosionBullet>()
                .Init(targetPos,
                      heroData.attack * explosionDamageMultiplier,
                      explosionRadius);

            yield break;
        }
        else
        {
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.5f);
            //Vector3 dir = (target.transform.position - transform.position).normalized;
            GameObject bullet = PoolManager.instance.GetDWBullet(spawnPos);

            var bw = bullet.GetComponent<DWBullet>();
            bw.SetTarget(target.transform, heroData.attack);

            var rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = dir * bullet.GetComponent<BaseBullet>().speed;
        }
    }
}
