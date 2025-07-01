using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class StartGameButton : MonoBehaviour
{
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();

        _button.onClick.AddListener(OnClickStartGame);
    }

    private void OnClickStartGame()
    {
        SceneManager.LoadScene("InGame");
    }
}
