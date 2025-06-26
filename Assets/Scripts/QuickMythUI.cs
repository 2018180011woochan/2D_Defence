using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickMythUI : MonoBehaviour
{
    [Header("모든 신화 레시피")]
    public List<MythicRecipe> allRecipes;

    [Header("즉시 소환 버튼 프리팹")]
    public GameObject quickButtonPrefab;

    [Header("버튼을 담을 컨테이너")]
    public Transform container; 

    [Header("최대 표시 개수")]
    public int maxVisible = 3;

    public GameObject mythAppearedPrefab;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (Transform t in container)
            Destroy(t.gameObject);

        int shown = 0;
        foreach (var recipe in allRecipes)
        {
            if (shown >= maxVisible)
                break;

            if (SummonManager.instance.IsRecipeReady(recipe))
            {
                var btnGO = Instantiate(quickButtonPrefab, container);

                var icon = btnGO.transform.Find("Icon").GetComponent<Image>();
                icon.sprite = recipe.resultHero.iconThumbnail;
                icon.preserveAspect = true;

                var label = btnGO.transform.Find("Label")
                              .GetComponent<TextMeshProUGUI>();
                label.text = "즉시 소환";

                var btn = btnGO.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    SummonManager.instance.SummonMythic(recipe);

                    // 소환 ui 등장
                    var canvas = GameObject.Find("Canvas_MainUI").transform;

                    mythAppearedPrefab.transform.Find("Icon").GetComponent<Image>().sprite = recipe.resultHero.iconThumbnail;
                    mythAppearedPrefab.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = recipe.resultHero.name;

                    var popup = Instantiate(mythAppearedPrefab, canvas, false);
                    Destroy(popup, 1f);

                    Refresh();
                });

                shown++;
            }
        }
    }
}
