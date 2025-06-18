using UnityEngine;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI roundText;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI curCoin;
    public TextMeshProUGUI curDiamond;
    public TextMeshProUGUI curHero;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateRoundText(int round)
    {
        roundText.text = $"Round {round}";
    }
    public void UpdateTimerText(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        TimerText.text = $"{minutes:D2}:{seconds:D2}";
    }

    public void UpdateCoinText(int coin)
    {
        curCoin.text = coin.ToString();
    }
    public void UpdateDiamondText(int diamond)
    {
        curDiamond.text = diamond.ToString();
    }

    public void UpdateHeroCountText(int current, int max)
    {
        curHero.text = $"{current}/{max}";
    }

    public void ShowDamageTMP(int Damage, GameObject target)
    {
        float minDmg = Damage - 10f;
        float maxDmg = Damage + 10f;

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

        // 위치 보정
        float xOffset = 150f;
        damageRect.anchoredPosition = localPos + new Vector2(-xOffset, 0);

        damageTextObj.GetComponent<DamageText>().Show(randDamage);
    }
}
