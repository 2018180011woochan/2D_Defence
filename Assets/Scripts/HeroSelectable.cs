using UnityEngine;

public class HeroSelectable : MonoBehaviour
{
    public HeroData heroData;
    public Vector3 groupCenterPosition;

    private void OnMouseDown()
    {
        HeroSelectionManager.instance.ToggleSelection(groupCenterPosition, heroData.range);
    }
}
