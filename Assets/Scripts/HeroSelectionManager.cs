using UnityEngine;

public class HeroSelectionManager : MonoBehaviour
{
    public static HeroSelectionManager instance;

    public GameObject rangeIndicatorPrefab;
    private GameObject currentIndicator;
    //private Transform selectedHero;
    private Vector3? selectedPosition = null;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        // 빈 공간 클릭 시 해제
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider == null)
            {
                Deselect();
            }
            else
            {
                if (hit.collider.GetComponent<HeroSelectable>() == null)
                {
                    Deselect();
                }
            }
        }
    }

    public void ToggleSelection(Vector3 position, float range)
    {
        if (selectedPosition.HasValue && selectedPosition.Value == position)
        {
            Deselect();
        }
        else
        {
            Select(position, range);
        }
    }

    private void Select(Vector3 position, float range)
    {
        if (currentIndicator != null)
            Destroy(currentIndicator);

        selectedPosition = position;

        currentIndicator = Instantiate(rangeIndicatorPrefab);
        currentIndicator.transform.position = position;
        currentIndicator.transform.localScale = Vector3.one * range * 2f;
    }

    private void Deselect()
    {
        selectedPosition = null;

        if (currentIndicator != null)
            Destroy(currentIndicator);
    }
}
