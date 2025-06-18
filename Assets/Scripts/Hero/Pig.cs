using System.Collections;
using UnityEngine;

public class Pig : BaseHero
{
    // ���� ȹ�� Ȯ�� (0~1)
    public float coinChance = 0.1f;

    // ���� �� ��� ���� ��
    public int coinAmount = 10;
    protected override IEnumerator ShootAfterDelay(GameObject target)
    {
        yield return new WaitForSeconds(attackCooldown);
        UIManager.instance.ShowDamageTMP(heroData.attack, target);
        /*float minDmg = heroData.attack - 10f;
        float maxDmg = heroData.attack + 10f;

        float randDamage = Random.Range(minDmg, maxDmg);

        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.GetDamage(randDamage);
        }

        Vector3 worldPos = target.transform.position + Vector3.up * 0.5f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        GameObject damageTextObj = PoolManager.instance.GetDamageText(Vector3.zero);

        Transform canvasTransform = GameObject.Find("Canvas_MainUI").transform;
        damageTextObj.transform.SetParent(canvasTransform, false);

        RectTransform canvasRect = canvasTransform.GetComponent<RectTransform>();
        RectTransform damageRect = damageTextObj.GetComponent<RectTransform>();

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, null, out localPos
        );

        // ��ġ ����
        float xOffset = 150f;
        damageRect.anchoredPosition = localPos + new Vector2(-xOffset, 0);

        damageTextObj.GetComponent<DamageText>().Show(randDamage);*/

        if (Random.value < coinChance)
        {
            GameManager.instance.AddCoins(coinAmount);
            // ������ ���� �ö󰡴� �ִϸ��̼� ���� �߰� 
        }
    }
}
