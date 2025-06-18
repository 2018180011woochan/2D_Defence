using System.Collections;
using UnityEngine;

public class Plant : BaseHero
{
    protected override IEnumerator ShootAfterDelay(GameObject target)
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 dir = (target.transform.position - transform.position).normalized;
        GameObject bullet = PoolManager.instance.GetPlantBullet(transform.position);
        bullet.GetComponent<PlantBullet>().SetTarget(target.transform, heroData.attack);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * 5f;
    }
}
