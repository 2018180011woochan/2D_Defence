using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MythDetailUI : MonoBehaviour
{
    [Header("°á°ú ¿µ¿õ")]
    public Image heroIcon;
    public TextMeshProUGUI heroName;

    public void Show(HeroData hd)
    {
        heroIcon.sprite = hd.iconThumbnail;
        heroName.text = hd.heroName;


    }
}
