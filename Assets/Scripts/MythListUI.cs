using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MythListUI : MonoBehaviour
{
    [Header("신화 목록")]
    public List<MythicRecipe> recipes;

    public GameObject buttonPrefab;
    public MythDetailUI detailUI;
    void Start()
    {
        foreach (var recipe in recipes)
        {
            var go = Instantiate(buttonPrefab, transform);

            go.transform.Find("Icon")
              .GetComponent<Image>().sprite = recipe.resultHero.iconThumbnail;

            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                detailUI.Show(recipe);
            });
        }
    }
}