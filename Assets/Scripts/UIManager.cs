using UnityEngine;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI roundText;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI curCoin;
    public TextMeshProUGUI curDiamond;
    public TextMeshProUGUI curHero;

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

    public void UpdateCoinText(int coin)
    {
        curCoin.text = coin.ToString();
    }
    public void UpdateDiamondText(int diamond)
    {
        curDiamond.text = diamond.ToString();
    }

    public void UpdateHeroCountText(int current, int max)
    {
        curHero.text = $"{current}/{max}";
    }
}
