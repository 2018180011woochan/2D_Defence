using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadInGameScene()
    {
        SceneManager.LoadScene("InGame");
    }

    public void LoadLobbyScene()
    {
        SceneManager.LoadScene("Lobby");
    }
}
