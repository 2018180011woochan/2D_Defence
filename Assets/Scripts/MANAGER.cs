using UnityEngine;

public class MANAGER : MonoBehaviour
{
    public static MANAGER instance = null;
    public static GameManager GameManager;
    public static PoolManager PoolManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        GameManager = GetComponent<GameManager>();
        PoolManager = GetComponent<PoolManager>();
    }
}
