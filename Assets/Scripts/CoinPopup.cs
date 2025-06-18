using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CoinPopup : MonoBehaviour
{
    public Image icon;                   
    public TextMeshProUGUI text;         

    public float floatSpeed = 40f;       // ���� �������� �ӵ� 
    public float fadeDuration = 1f;     // ������¼ӵ�

    private RectTransform rt;
    private float timer;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public void Show(int amount)
    {
        text.text = $"+{amount}";
        timer = fadeDuration;

    }


    private void Update()
    {
        // 1) ���� ��������
        rt.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;

        // 2) ���� ���� ���̱�
        timer -= Time.deltaTime;
        float a = Mathf.Clamp01(timer / fadeDuration);
        Color c = text.color;
        c.a = a;
        text.color = c;
        icon.color = c;

        // 3) �ð� ������ Ǯ�� ��ȯ
        if (timer <= 0f)
            PoolManager.instance.ReleaseCoinPopup(gameObject);
    }
}
