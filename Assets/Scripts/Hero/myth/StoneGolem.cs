using System.Collections;
using UnityEngine;

public class StoneGolem : BaseHero
{
    [Range(0, 1)] public float SpcialAttackChance = 0.2f;         
    public float SpcialAttackDamageMultiplier = 50f;             
    public GameObject dustEffectPrefab;                         
    public float specialADelay = 2.0f;

    public float hitDelay = 1.0f;
    private bool isSpecialPlaying = false;

    protected override void Update()
    {
        if (isSpecialPlaying) return;
        base.Update();
    }

    protected override IEnumerator ShootAfterDelay(GameObject target)
    {
        if (Random.value < SpcialAttackChance)
        {
            isSpecialPlaying = true;
            Debug.Log("스페셜공격 발동!");
            animator.SetTrigger("SpecialATrigger");

            yield return new WaitForSeconds(specialADelay);

            // 이펙트 생성
            //Instantiate(dustEffectPrefab, transform.position, Quaternion.identity);

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position, heroData.range, LayerMask.GetMask("Enemy"));

            foreach (var col in hits)
            {
                var enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    float dmg = heroData.attack * SpcialAttackDamageMultiplier;
                    enemy.GetDamage(dmg);
                    UIManager.instance.ShowDamageTMP((int)dmg, enemy.gameObject);
                }
            }

            isSpecialPlaying = false;
            yield break;  
        }


        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(hitDelay);

        if (target != null)
        {
            var enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                float dmg = heroData.attack;
                enemy.GetDamage(dmg);
                UIManager.instance.ShowDamageTMP(heroData.attack, target);
            }
        }
    }

    public void OnSpecialEffect()
    {
        Instantiate(dustEffectPrefab, transform.position, Quaternion.identity);
    }
}
