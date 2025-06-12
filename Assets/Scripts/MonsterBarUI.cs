using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterBarUI : MonoBehaviour
{
    public static MonsterBarUI instance;

    public Image fillImage;
    public int maxMonsterCount = 100;
    public TextMeshProUGUI monsterCountText;

    private void Awake()
    {
        instance = this;
    }
    public void UpdateMonsterCount(int current)
    {
        float ratio = Mathf.Clamp01((float)current / maxMonsterCount);
        fillImage.fillAmount = ratio;

        monsterCountText.text = $"{current} / {maxMonsterCount}";
    }
}