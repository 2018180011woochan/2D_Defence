using System.Collections;
using UnityEngine;

public class SkeletonMage : BaseHero
{
    public GameObject hitEffectPrefab;

    public float hitDelay = 1.0f;

    protected override IEnumerator ShootAfterDelay(GameObject target)
    {
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
