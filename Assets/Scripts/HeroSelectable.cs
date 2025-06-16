using UnityEngine;

public class HeroSelectable : MonoBehaviour
{
    public HeroData heroData;
    public Vector3 groupCenterPosition;
    public Vector2Int gridPos;
    private Vector3 dragStartPos;
    private bool isDragging = false;

    private static Vector2Int currentDraggedGroup = new Vector2Int(-1, -1);
    private static bool isAnyGroupBeingDragged = false;

    private void OnMouseDown()
    {
        dragStartPos = Input.mousePosition;
        isDragging = true;
        HeroSelectionManager.instance.ToggleSelection(groupCenterPosition, heroData.range);
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;

        Vector3 dragEndPos = Input.mousePosition;

        if (Vector3.Distance(dragStartPos, dragEndPos) > 30f)
        {
            if (isAnyGroupBeingDragged && currentDraggedGroup == gridPos)
            {
                Debug.Log("이미 이 그룹이 드래그 처리 중입니다.");
                return;
            }

            // 다른 그룹이 드래그 중이면 무시
            if (isAnyGroupBeingDragged && currentDraggedGroup != gridPos)
            {
                Debug.Log("다른 그룹이 드래그 중입니다.");
                return;
            }


            // 드래그 처리 시작
            isAnyGroupBeingDragged = true;
            currentDraggedGroup = gridPos;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(dragEndPos.x, dragEndPos.y, Camera.main.nearClipPlane));
            Vector2Int targetCell = SummonManager.instance.GetCellIndexFromWorld(worldPos);

            if (targetCell != gridPos)
            {
                SummonManager.instance.TrySwapGroup(gridPos, targetCell);
            }

            Invoke(nameof(ResetDragFlag), 0.1f);
        }
    }

    private void ResetDragFlag()
    {
        isAnyGroupBeingDragged = false;
        currentDraggedGroup = new Vector2Int(-1, -1);
        HeroSelectionManager.instance.Deselect();
    }

    // 마우스가 오브젝트를 벗어났을 때도 드래그 상태 리셋
    private void OnMouseExit()
    {
        if (isDragging && Input.GetMouseButton(0) == false)
        {
            isDragging = false;
        }
    }
}