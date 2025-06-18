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

        if (Random.value < coinChance)
        {
            GameManager.instance.AddCoins(coinAmount);
            // ������ ���� �ö󰡴� �ִϸ��̼� ���� �߰� 
            SpawnCoinPopup(coinAmount);
        }
    }

    void SpawnCoinPopup(int amount)
    {
        Vector3 worldPos = transform.position + Vector3.up * 1.2f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        GameObject pop = PoolManager.instance.GetCoinPopup(Vector3.zero);

        RectTransform canvasRect = GameObject
            .Find("Canvas_MainUI")
            .GetComponent<RectTransform>();

        pop.transform.SetParent(canvasRect, false);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,       
            screenPos,        
            null,             
            out Vector2 localPos
        );

        RectTransform popRect = pop.GetComponent<RectTransform>();
        float xOffset = 80f;    // ��ġ ����
        popRect.anchoredPosition = localPos + new Vector2(xOffset, 0);

        pop.GetComponent<CoinPopup>().Show(amount);
    }
}
