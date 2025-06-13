using UnityEngine;

public class HeroSelectionManager : MonoBehaviour
{
    public static HeroSelectionManager instance;

    public GameObject rangeIndicatorPrefab;
    private GameObject currentIndicator;
    private Transform selectedHero;

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

    public void ToggleSelection(Transform heroTransform, float range)
    {
        if (selectedHero == heroTransform)
        {
            // 같은 유닛 다시 클릭 → 선택 해제
            Deselect();
        }
        else
        {
            // 다른 유닛 선택
            Select(heroTransform, range);
        }
    }

    private void Select(Transform heroTransform, float range)
    {
        if (currentIndicator != null)
            Destroy(currentIndicator);

        selectedHero = heroTransform;

        currentIndicator = Instantiate(rangeIndicatorPrefab);
        currentIndicator.transform.position = heroTransform.position;
        currentIndicator.transform.localScale = Vector3.one * range;
    }

    private void Deselect()
    {
        selectedHero = null;

        if (currentIndicator != null)
            Destroy(currentIndicator);
    }
}
