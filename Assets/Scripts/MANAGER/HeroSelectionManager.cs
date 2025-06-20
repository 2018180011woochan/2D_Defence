using UnityEngine;
using UnityEngine.EventSystems;

public class HeroSelectionManager : MonoBehaviour
{
    public static HeroSelectionManager instance;

    [Header("Range Indicator")]
    public GameObject rangeIndicatorPrefab;
    private GameObject currentIndicator;

    // ���� ���õ� �� (row, col)
    private Vector2Int? selectedCell = null;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        // �� ���� Ŭ�� �� ����
        if (Input.GetMouseButtonDown(0))
        {
            // UI Ŭ���� ����
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            // �ƹ��ų� ������ �ʾҰų�, HeroSelectable ������ ����
            if (hit.collider == null ||
                hit.collider.GetComponent<HeroSelectable>() == null)
            {
                Deselect();
            }
        }
    }

    /// <summary>
    /// �ܺ�(HeroSelectable)���� ���� Ŭ�� �� ȣ���ϴ� �������̽�
    /// </summary>
    /// <param name="position">�׷� �߾� ���� ��ǥ</param>
    /// <param name="range">��Ÿ�(�� ������)</param>
    public void ToggleSelection(Vector3 position, float range, Vector2Int cell)
    {
        // ���� ���� �ٽ� Ŭ���ϸ� ����
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
    /// ���� ���·� ��ȯ. ��Ÿ� ǥ�� + �Ǹ� ��ư ǥ��
    /// </summary>
    private void Select(Vector3 position, float range, Vector2Int cell)
    {
        // ���� ����
        if (currentIndicator != null) Destroy(currentIndicator);
        // (���� ���õ� ĭ�� �Ǹ� ��ư�� �����)
        DeselectSellButton();

        // ���� ����
        selectedCell = cell;

        // ��Ÿ� �ε������� ����
        currentIndicator = Instantiate(rangeIndicatorPrefab);
        currentIndicator.transform.position = position;
        currentIndicator.transform.localScale = Vector3.one * range * 1.5f;

        // �Ǹ� ��ư ǥ��
        SummonManager.instance.ShowSellButton(cell.x, cell.y);
    }

    /// <summary>
    /// ����: ��Ÿ� ���� + �Ǹ� ��ư �����
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

    // ���� ���õ� ���� �Ǹ� ��ư�� ����ϴ�
    private void DeselectSellButton()
    {
        if (selectedCell.HasValue)
        {
            var c = selectedCell.Value;
            SummonManager.instance.HideSellButton(c.x, c.y);
        }
    }
}
