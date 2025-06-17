using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI text;

    private float floatSpeed = 30f;       // ���� �̵� �ӵ�
    private float fadeDuration = 1f;      // ������� �� �ɸ��� �ð�
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
        text.color = originalColor;  // �ʱ�ȭ

        // ���� ��ġ�� �Ź� ������ �� �ְ� �ϸ� ����
        //rectTransform.anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        // 1. ���� ��������
        rectTransform.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;

        // 2. ���İ� ���̱�
        float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeDuration);
        Color c = text.color;
        c.a = alpha;
        text.color = c;

        // 3. ��������� Ǯ�� ��ȯ
        if (currentTime >= fadeDuration)
        {
            PoolManager.instance.ReleaseDamageText(gameObject);
        }
    }
}
