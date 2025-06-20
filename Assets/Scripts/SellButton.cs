using UnityEngine;
using UnityEngine.UI;

public class SellButton : MonoBehaviour
{
    public int row, col;
    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickSell);
    }

    private void OnClickSell()
    {
        SummonManager.instance.SellOne(row, col);
    }

    public void Hide()
    {
        Destroy(gameObject);
    }
}
