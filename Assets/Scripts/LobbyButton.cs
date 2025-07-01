using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyButton : MonoBehaviour
{
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClickLobby);
    }

    private void OnClickLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
