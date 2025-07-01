using System.Collections;
using TMPro;
using UnityEngine;

public class CountDown : MonoBehaviour
{
    public int startCount = 5;

    public TextMeshProUGUI countdownText;

    public IEnumerator Play()
    {
        int counter = startCount;
        while (counter > 0)
        {
            countdownText.text = counter.ToString();
            yield return new WaitForSeconds(1f);
            counter--;
        }

        countdownText.text = "Go!";
        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }

    private void Awake()
    {
        if (countdownText == null)
            countdownText = GetComponentInChildren<TextMeshProUGUI>();
    }
}
