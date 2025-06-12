using UnityEngine;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI roundText;
    public TextMeshProUGUI TimerText;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateRoundText(int round)
    {
        roundText.text = $"Round {round}";
    }
    public void UpdateTimerText(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        TimerText.text = $"{minutes:D2}:{seconds:D2}";
    }
}
