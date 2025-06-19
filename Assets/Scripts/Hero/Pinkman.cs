using System.Collections;
using UnityEngine;

public class Pinkman : BaseHero
{
    // 다이아 획득 확률
    public float diamondChance = 0.1f;

    // 성공시 얻는 다이아 양
    public int diamondAmount = 1;

    protected override IEnumerator ShootAfterDelay(GameObject target)
    {
        yield return new WaitForSeconds(attackCooldown);
        UIManager.instance.ShowDamageTMP(heroData.attack, target);

        if (Random.value < diamondChance)
        {
            GameManager.instance.AddDiamonds(diamondAmount);

            SpawnDiamondPopup(diamondAmount);
        }
    }

    private void SpawnDiamondPopup(int amount)
    {
        Vector3 worldPos = transform.position + Vector3.up * 1.2f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        GameObject pop = PoolManager.instance.GetDiamondPopup(Vector3.zero);

        var canvasRect = GameObject
            .Find("Canvas_MainUI")
            .GetComponent<RectTransform>();

        pop.transform.SetParent(canvasRect, false);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, null, out Vector2 localPos
        );

        float xOffset = 80f;
        pop.GetComponent<RectTransform>().anchoredPosition
            = localPos + new Vector2(xOffset, 0);

        pop.GetComponent<DiamondPopup>().Show(amount);
    }
}
