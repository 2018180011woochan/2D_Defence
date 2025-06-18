using System.Collections;
using UnityEngine;

public class Trunk : BaseHero
{
    protected override IEnumerator ShootAfterDelay(GameObject target)
    {
        yield return new WaitForSeconds(attackCooldown);

        Vector3 dir = (target.transform.position - transform.position).normalized;
        GameObject bullet = PoolManager.instance.GetTrunkBullet(transform.position);
        bullet.GetComponent<TrunkBullet>().SetTarget(target.transform, heroData.attack);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * 5f;
    }
}
