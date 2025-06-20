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

        // �� ���� ����: �� ��° ���ڷ� gridPos�� �Ѱ��ݴϴ�.
        HeroSelectionManager.instance
            .ToggleSelection(groupCenterPosition, heroData.range, gridPos);
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
                Debug.Log("�̹� �� �׷��� �巡�� ó�� ���Դϴ�.");
                return;
            }
            if (isAnyGroupBeingDragged && currentDraggedGroup != gridPos)
            {
                Debug.Log("�ٸ� �׷��� �巡�� ���Դϴ�.");
                return;
            }

            isAnyGroupBeingDragged = true;
            currentDraggedGroup = gridPos;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(
                new Vector3(dragEndPos.x, dragEndPos.y, Camera.main.nearClipPlane));
            Vector2Int targetCell =
                SummonManager.instance.GetCellIndexFromWorld(worldPos);

            if (targetCell != gridPos)
                SummonManager.instance.TrySwapGroup(gridPos, targetCell);

            Invoke(nameof(ResetDragFlag), 0.1f);
        }
    }

    private void ResetDragFlag()
    {
        isAnyGroupBeingDragged = false;
        currentDraggedGroup = new Vector2Int(-1, -1);
        HeroSelectionManager.instance.Deselect();
    }

    private void OnMouseExit()
    {
        if (isDragging && Input.GetMouseButton(0) == false)
            isDragging = false;
    }
}
