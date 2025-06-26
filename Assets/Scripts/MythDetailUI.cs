using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MythDetailUI : MonoBehaviour
{
    public Image heroIcon;
    public TextMeshProUGUI heroName;
    public GameObject mythAppearedPrefab;
    public Button summonButton;

    public Transform requirementsContainer;
    public GameObject reqSlotPrefab;
    MythicRecipe selectedRecipe;

    private void OnEnable()
    {
        if (selectedRecipe != null)
            Show(selectedRecipe);
    }

    public void Show(MythicRecipe recipe)
    {
        gameObject.SetActive(true);
        selectedRecipe = recipe;
        heroIcon.sprite = recipe.resultHero.iconThumbnail;
        heroName.text = recipe.resultHero.heroName;

        foreach (Transform t in requirementsContainer)
            Destroy(t.gameObject);

        bool canSummon = true;

        foreach (var reqHero in recipe.requiredHeroes)
        {
            var slot = Instantiate(reqSlotPrefab, requirementsContainer);

            var iconImg = slot.transform.Find("Icon").GetComponent<Image>();
            iconImg.sprite = reqHero.iconThumbnail;
            iconImg.preserveAspect = true;

            bool isHave = SummonManager.instance.GetisHaveHero(reqHero);
            var haveText = slot.transform.Find("isHave")
                                .GetComponent<TextMeshProUGUI>();
            if (isHave)
            {
                haveText.text = "보유";
            }
            else
            {
                haveText.text = "미보유";
                canSummon = false;
            }

            var checkGO = slot.transform.Find("Check").gameObject;
            checkGO.SetActive(isHave);

            checkGO.transform.SetAsLastSibling();

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

        summonButton.interactable = canSummon;
    }

    public void OnClickSummon()
    {
        if (selectedRecipe == null) return;

        // 실제 소환 실행
        SummonManager.instance.SummonMythic(selectedRecipe);

        // 소환 ui 등장
        var canvas = GameObject.Find("Canvas_MainUI").transform;

        mythAppearedPrefab.transform.Find("Icon").GetComponent<Image>().sprite = selectedRecipe.resultHero.iconThumbnail;
        mythAppearedPrefab.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = selectedRecipe.resultHero.name;

        var popup = Instantiate(mythAppearedPrefab, canvas, false);
        Destroy(popup, 1f);

        // UI 닫기
        gameObject.SetActive(false);
    }
}
