using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI text;

    private float floatSpeed = 30f;       // 위로 이동 속도
    private float fadeDuration = 1f;      // 사라지는 데 걸리는 시간
    private float currentTime = 0f;
    private RectTransform rectTransform;
    private Color originalColor;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalColor = text.color;
    }

    public void Show(float damage)
    {
        text.text = Mathf.RoundToInt(damage).ToString();
        currentTime = 0f;
        text.color = originalColor;  // 초기화

        // 시작 위치를 매번 리셋할 수 있게 하면 좋음
        //rectTransform.anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        // 1. 위로 떠오르기
        rectTransform.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;

        // 2. 알파값 줄이기
        float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeDuration);
        Color c = text.color;
        c.a = alpha;
        text.color = c;

        // 3. 사라졌으면 풀로 반환
        if (currentTime >= fadeDuration)
        {
            PoolManager.instance.ReleaseDamageText(gameObject);
        }
    }
}
