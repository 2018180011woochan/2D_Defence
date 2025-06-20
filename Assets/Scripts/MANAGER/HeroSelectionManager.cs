using UnityEngine;
using UnityEngine.EventSystems;

public class HeroSelectionManager : MonoBehaviour
{
    public static HeroSelectionManager instance;

    [Header("Range Indicator")]
    public GameObject rangeIndicatorPrefab;
    private GameObject currentIndicator;

    // 지금 선택된 셀 (row, col)
    private Vector2Int? selectedCell = null;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        // 빈 공간 클릭 시 해제
        if (Input.GetMouseButtonDown(0))
        {
            // UI 클릭은 무시
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            // 아무거나 찍히지 않았거나, HeroSelectable 없으면 해제
            if (hit.collider == null ||
                hit.collider.GetComponent<HeroSelectable>() == null)
            {
                Deselect();
            }
        }
    }

    /// <summary>
    /// 외부(HeroSelectable)에서 영웅 클릭 시 호출하는 인터페이스
    /// </summary>
    /// <param name="position">그룹 중앙 월드 좌표</param>
    /// <param name="range">사거리(원 스케일)</param>
    public void ToggleSelection(Vector3 position, float range, Vector2Int cell)
    {
        // 같은 셀을 다시 클릭하면 해제
        if (selectedCell.HasValue && selectedCell.Value == cell)
        {
            Deselect();
        }
        else
        {
            Select(position, range, cell);
        }
    }

    /// <summary>
    /// 선택 상태로 전환. 사거리 표시 + 판매 버튼 표시
    /// </summary>
    private void Select(Vector3 position, float range, Vector2Int cell)
    {
        // 기존 해제
        if (currentIndicator != null) Destroy(currentIndicator);
        // (이전 선택된 칸의 판매 버튼도 숨기기)
        DeselectSellButton();

        // 새로 선택
        selectedCell = cell;

        // 사거리 인디케이터 생성
        currentIndicator = Instantiate(rangeIndicatorPrefab);
        currentIndicator.transform.position = position;
        currentIndicator.transform.localScale = Vector3.one * range * 1.5f;

        // 판매 버튼 표시
        SummonManager.instance.ShowSellButton(cell.x, cell.y);
    }

    /// <summary>
    /// 해제: 사거리 제거 + 판매 버튼 숨기기
    /// </summary>
    public void Deselect()
    {
        selectedCell = null;
        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
            currentIndicator = null;
        }
        DeselectSellButton();
    }

    // 현재 선택된 셀의 판매 버튼만 숨깁니다
    private void DeselectSellButton()
    {
        if (selectedCell.HasValue)
        {
            var c = selectedCell.Value;
            SummonManager.instance.HideSellButton(c.x, c.y);
        }
    }
}
