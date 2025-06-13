using UnityEngine;

public class HeroSelectable : MonoBehaviour
{
    public HeroData heroData;
    public Vector3 groupCenterPosition;

    public Vector2Int gridPos;
    private Vector3 dragStartPos;

    private void OnMouseDown()
    {
        dragStartPos = Input.mousePosition;
        HeroSelectionManager.instance.ToggleSelection(groupCenterPosition, heroData.range);
    }

    /// <summary>
    /// ////////////////////////////////////////////////
    /// 마우스 끌기로 교체 구현중
    /// </summary>
    private void OnMouseUp()
    {
        Vector3 dragEndPos = Input.mousePosition;

        if (Vector3.Distance(dragStartPos, dragEndPos) > 30f)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(dragEndPos);
            Vector2Int targetCell = SummonManager.instance.GetCellIndexFromWorld(worldPos);

            if (targetCell != gridPos)
            {
                SummonManager.instance.TrySwapGroup(gridPos, targetCell);
            }
        }
    }
}
