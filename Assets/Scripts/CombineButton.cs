using UnityEngine;
using UnityEngine.UI;

public class CombineButton : MonoBehaviour
{
    public int row, col;
    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickCombine);
    }

    private void OnClickCombine()
    {
        SummonManager.instance.Combine(row, col);
    }

    public void Hide()
    {
        Destroy(gameObject);
    }
}
