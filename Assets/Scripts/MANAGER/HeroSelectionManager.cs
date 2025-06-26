using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroSelectionManager : MonoBehaviour
{
    public static HeroSelectionManager instance;

    [Header("영웅 전용 레이어 마스크")]
    public LayerMask heroLayerMask;

    [Header("사거리 표시")]
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
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // Hero 레이어만 찍도록 Raycast
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(
                ray.origin,
                ray.direction, 
                Mathf.Infinity,
                heroLayerMask);


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
        if (SummonManager.instance.GetGroupCount(cell.x, cell.y) == 3 &&
            SummonManager.instance.GetGroupGrade(cell.x, cell.y) != HeroGrade.Legendary &&
            SummonManager.instance.GetGroupGrade(cell.x, cell.y) != HeroGrade.Mythic)
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
