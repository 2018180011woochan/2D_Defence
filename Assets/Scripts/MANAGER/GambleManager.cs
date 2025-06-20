using UnityEngine;

public class GambleManager : MonoBehaviour
{
    public static GambleManager instance { get; private set; }

    public GameObject gambleBoard;   

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void ToggleGambleBoard()
    {
        bool isActive = gambleBoard.activeSelf;
        gambleBoard.SetActive(!isActive);
    }
}
