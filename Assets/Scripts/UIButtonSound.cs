using UnityEngine;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour
{
    public AudioClip clickSound;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(() =>
        {
            SFXManager.instance.PlaySFX(clickSound);
        });
    }
}
