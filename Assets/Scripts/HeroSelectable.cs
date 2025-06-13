using UnityEngine;

public class HeroSelectable : MonoBehaviour
{
    public HeroData heroData;

    private void OnMouseDown()
    {
        HeroSelectionManager.instance.ToggleSelection(transform, heroData.range);
    }
}
