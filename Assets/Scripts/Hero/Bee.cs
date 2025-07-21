using System.Collections;
using UnityEngine;

public class Bee : BaseHero
{
    protected override IEnumerator ShootAfterDelay(GameObject target)
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 dir = (target.transform.position - transform.position).normalized;
        GameObject bullet = PoolManager.instance.GetBeeBullet(transform.position);
        Debug.Log("°ø°Ý·Â" + heroData.attack);
        bullet.GetComponent<BeeBullet>().SetTarget(target.transform, heroData.attack);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * 5f;
    }

}
