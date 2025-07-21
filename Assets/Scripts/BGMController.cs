using UnityEngine;

public class BGMController : MonoBehaviour
{
    public static BGMController Instance;

    public AudioClip normalBGM;
    public AudioClip bossBGM;
    private AudioSource audioSource;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayBossBGM()
    {
        if (audioSource.clip != bossBGM)  
        {
            audioSource.clip = bossBGM;
            audioSource.Play();
        }
    }

    public void PlayNormalBGM()
    {
        if (audioSource.clip != normalBGM) 
        {
            audioSource.clip = normalBGM;
            audioSource.Play();
        }
    }
}
