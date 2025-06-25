using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MythListUI : MonoBehaviour
{
    [Header("신화 목록")]
    public List<HeroData> Myths;

    public GameObject buttonPrefab;
    public System.Action<HeroData> onClickHero; // 클릭된 영웅 콜백
    public MythDetailUI detailUI;
    void Start()
    {
        foreach (var hd in Myths)
        {
            var go = Instantiate(buttonPrefab, transform);
            // 아이콘 이미지 설정
            go.transform.Find("Icon")
              .GetComponent<Image>().sprite = hd.iconThumbnail;

            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                detailUI.Show(hd);
            });
        }
    }
}