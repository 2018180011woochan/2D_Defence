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
        // �� ���� Ŭ�� �� ����
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
            // ���� ���� �ٽ� Ŭ�� �� ���� ����
            Deselect();
        }
        else
        {
            // �ٸ� ���� ����
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
