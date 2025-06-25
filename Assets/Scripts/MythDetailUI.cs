using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MythDetailUI : MonoBehaviour
{
    public Image heroIcon;
    public TextMeshProUGUI heroName;

    public Transform requirementsContainer;
    public GameObject reqSlotPrefab;

    public void Show(MythicRecipe recipe)
    {
        heroIcon.sprite = recipe.resultHero.iconThumbnail;
        heroName.text = recipe.resultHero.heroName;

        foreach (Transform t in requirementsContainer)
            Destroy(t.gameObject);

        foreach (var reqHero in recipe.requiredHeroes)
        {
            var slot = Instantiate(reqSlotPrefab, requirementsContainer);

            var iconImg = slot.transform.Find("Icon").GetComponent<Image>();
            iconImg.sprite = reqHero.iconThumbnail;
            iconImg.preserveAspect = true;

            var bgImg = slot.GetComponent<Image>();
            switch (reqHero.grade)
            {
                case HeroGrade.Normal:
                    bgImg.color = Color.gray; break;
                case HeroGrade.Rare:
                    bgImg.color = new Color(0f, 0.5f, 1f); break;
                case HeroGrade.Epic:
                    bgImg.color = new Color(0.6f, 0f, 0.9f); break;
                case HeroGrade.Legendary:
                    bgImg.color = Color.yellow; break;
            }

            // 아이콘 세팅
            slot.transform
                .Find("Icon").GetComponent<Image>()
                .sprite = reqHero.iconThumbnail;
        }
    }
}
