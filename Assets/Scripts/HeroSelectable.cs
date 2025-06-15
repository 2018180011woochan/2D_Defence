using UnityEngine;

public class HeroSelectable : MonoBehaviour
{
    public HeroData heroData;
    public Vector3 groupCenterPosition;
    public Vector2Int gridPos;
    private Vector3 dragStartPos;
    private bool isDragging = false;

    // 정적 변수로 현재 드래그 중인 그룹을 추적
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

        // 드래그 거리가 충분한지 확인
        if (Vector3.Distance(dragStartPos, dragEndPos) > 30f)
        {
            // 현재 이 그룹이 이미 드래그 처리 중인지 확인
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

            Debug.Log($"드래그 시작 위치: {gridPos}, 목표 위치: {targetCell}");

            if (targetCell != gridPos)
            {
                SummonManager.instance.TrySwapGroup(gridPos, targetCell);
                Debug.Log($"그룹 드래그 완료: {gridPos} → {targetCell}");
            }

            // 드래그 처리 완료 후 0.1초 후에 플래그 리셋
            Invoke(nameof(ResetDragFlag), 0.1f);
        }
    }

    private void ResetDragFlag()
    {
        isAnyGroupBeingDragged = false;
        currentDraggedGroup = new Vector2Int(-1, -1);
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