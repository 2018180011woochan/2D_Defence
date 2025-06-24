using UnityEngine;
using UnityEngine.EventSystems;

public class HeroSelectionManager : MonoBehaviour
{
    public static HeroSelectionManager instance;

    [Header("Range Indicator")]
    public GameObject rangeIndicatorPrefab;
    private GameObject currentIndicator;

    private Vector2Int? selectedCell = null;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider == null ||
                hit.collider.GetComponent<HeroSelectable>() == null)
            {
                Deselect();
            }
        }
    }

    public void ToggleSelection(Vector3 position, float range, Vector2Int cell)
    {
        if (selectedCell.HasValue && selectedCell.Value == cell)
        {
            Deselect();
        }
        else
        {
            Select(position, range, cell);
        }
    }

    private void Select(Vector3 position, float range, Vector2Int cell)
    {
        if (currentIndicator != null) Destroy(currentIndicator);

        if (selectedCell.HasValue)
        {
            var prev = selectedCell.Value;
            SummonManager.instance.HideSellButton(prev.x, prev.y);
            SummonManager.instance.HideCombineButton(prev.x, prev.y);
        }

        selectedCell = cell;

        currentIndicator = Instantiate(rangeIndicatorPrefab);
        currentIndicator.transform.position = position;
        currentIndicator.transform.localScale = Vector3.one * range * 1.5f;

        SummonManager.instance.ShowSellButton(cell.x, cell.y);
        SummonManager.instance.ShowCombineButton(cell.x, cell.y);
    }

    public void Deselect()
    {
        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
            currentIndicator = null;
        }

        if (selectedCell.HasValue)
        {
            var c = selectedCell.Value;
            SummonManager.instance.HideSellButton(c.x, c.y);
            SummonManager.instance.HideCombineButton(c.x, c.y);
        }

        selectedCell = null;
    }
}
