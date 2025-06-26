using TMPro;
using UnityEngine;

public class RulletLogUI : MonoBehaviour
{
    public static RulletLogUI instance { get; private set; }

    public Transform container;            
    public GameObject entryPrefab;        
    [Header("최대 표시 줄 수")]
    public int maxEntries = 4;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddMessage(string message, Color? color = null)
    {
        if (container.childCount >= maxEntries)
        {
            Destroy(container.GetChild(0).gameObject);
        }

         var go = Instantiate(entryPrefab, container, false);
        var text = go.GetComponentInChildren<TextMeshProUGUI>();
        text.text = message;
        if (color.HasValue) text.color = color.Value;
    }

}
