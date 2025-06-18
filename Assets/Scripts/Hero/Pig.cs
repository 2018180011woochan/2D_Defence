using System.Collections;
using UnityEngine;

public class Pig : BaseHero
{
    // 코인 획득 확률 (0~1)
    public float coinChance = 0.1f;

    // 성공 시 얻는 코인 양
    public int coinAmount = 10;
    protected override IEnumerator ShootAfterDelay(GameObject target)
    {
        yield return new WaitForSeconds(attackCooldown);
        UIManager.instance.ShowDamageTMP(heroData.attack, target);

        if (Random.value < coinChance)
        {
            GameManager.instance.AddCoins(coinAmount);
            // 코인이 위로 올라가는 애니메이션 만들어서 추가 
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
        float xOffset = 80f;    // 위치 보정
        popRect.anchoredPosition = localPos + new Vector2(xOffset, 0);

        pop.GetComponent<CoinPopup>().Show(amount);
    }
}
