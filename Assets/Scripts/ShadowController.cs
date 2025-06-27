using UnityEngine;

public class ShadowController : MonoBehaviour
{
    public SpriteRenderer shadowRenderer;
    public void SetColor(Color c)
    {
        if (shadowRenderer != null)
            shadowRenderer.color = c;
    }
}
