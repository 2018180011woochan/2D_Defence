using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CoinPopup : MonoBehaviour
{
    public Image icon;                   
    public TextMeshProUGUI text;         

    public float floatSpeed = 40f;       // 위로 떠오르는 속도 
    public float fadeDuration = 1f;     // 사라지는속도

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
        // 1) 위로 떠오르기
        rt.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;

        // 2) 투명도 점점 줄이기
        timer -= Time.deltaTime;
        float a = Mathf.Clamp01(timer / fadeDuration);
        Color c = text.color;
        c.a = a;
        text.color = c;
        icon.color = c;

        // 3) 시간 끝나면 풀로 반환
        if (timer <= 0f)
            PoolManager.instance.ReleaseCoinPopup(gameObject);
    }
}
