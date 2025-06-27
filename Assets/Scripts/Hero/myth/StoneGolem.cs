using System.Collections;
using UnityEngine;

public class StoneGolem : BaseHero
{
    [Range(0, 1)] public float SpcialAttackChance = 0.2f;         
    public float SpcialAttackDamageMultiplier = 50f;             
    public GameObject dustEffectPrefab;                         
    public float specialADelay = 2.0f;

    [Range(0, 1)] public float rockChance = 0.2f;              
    public float rockSpawnHeight = 15.0f;                       
    public GameObject rockPrefab;                              
    //public GameObject rockImpactEffectPrefab;                  
    public float rockAreaDamageMultiplier = 20f;
    public float rockADelay = 2.0f;
    public int rockCount = 5;
    public float rockSpreadX = 1.0f;
    public float rockSpreadY = 0.5f;

    public float hitDelay = 1.0f;
    private bool isSkillPlaying = false;

    protected override void Update()
    {
        if (isSkillPlaying) return;
        base.Update();
    }

    protected override IEnumerator ShootAfterDelay(GameObject target)
    {
        float r = Random.value;
        if (r < SpcialAttackChance)
        {
            isSkillPlaying = true;
            animator.SetTrigger("SpecialATrigger");

            yield return new WaitForSeconds(specialADelay);

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position,
                heroData.range);

            foreach (var col in hits)
            {
                if (!col.CompareTag("Enemy"))
                    continue;
                var enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    float dmg = heroData.attack * SpcialAttackDamageMultiplier;
                    enemy.GetDamage(dmg);
                    UIManager.instance.ShowDamageTMP((int)dmg, enemy.gameObject);
                }
            }

            isSkillPlaying = false;
            yield break;  
        }
        else if (r < SpcialAttackChance + rockChance)
        {
            isSkillPlaying = true;
            animator.SetTrigger("ClimbTrigger");


            Vector3 basePos = target.transform.position + Vector3.up * rockSpawnHeight;
            for (int i = 0; i < rockCount; i++)
            {
                // 랜덤 오프셋
                float dx = Random.Range(-rockSpreadX, rockSpreadX);
                float dy = Random.Range(-rockSpreadY, rockSpreadY);
                Vector3 spawnPos = basePos + new Vector3(dx, dy, 0);

                // 돌 생성 & 초기화
                var rock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);
                var proj = rock.GetComponent<RockProjectile>();
                proj.Init(
                    heroData.attack,
                    rockAreaDamageMultiplier,
                    heroData.range);
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(rockADelay);
            isSkillPlaying = false;
            yield break;

        }
        else
        {
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

    }

    public void OnSpecialEffect()
    {
        Instantiate(dustEffectPrefab, transform.position, Quaternion.identity);
    }
}
