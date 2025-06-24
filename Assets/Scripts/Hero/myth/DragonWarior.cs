using System.Collections;
using UnityEngine;

public class DragonWarior : BaseHero
{
    public float strikeChance = 0.9f;   
    public float strikeAnimDelay = 0.3f;                 
    public float blastDamageMultiplier = 10f;            

    protected override IEnumerator ShootAfterDelay(GameObject target)
    {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);

        if (Random.value <= strikeChance)
        {
            animator.SetTrigger("Strike");
            yield return new WaitForSeconds(1.0f);
            yield return new WaitForSeconds(strikeAnimDelay);

            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f;
                Vector3 dir8 = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad), 0f);

                GameObject blast = PoolManager.instance.GetBlastBullet(transform.position);
                blast.GetComponent<BlastBullet>().Init(dir8, heroData.attack * blastDamageMultiplier);

                Rigidbody2D rb8 = blast.GetComponent<Rigidbody2D>();
                rb8.linearVelocity = dir8 * blast.GetComponent<BlastBullet>().speed;
            }

            yield break;   
        }

        Vector3 dir = (target.transform.position - transform.position).normalized;
        GameObject bullet = PoolManager.instance.GetDWBullet(transform.position);


        var bw = bullet.GetComponent<DWBullet>();
        bw.SetTarget(target.transform, heroData.attack);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * 5f;
    }
}
