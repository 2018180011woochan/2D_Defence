using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class RulletManager : MonoBehaviour
{
    public static RulletManager instance { get; private set; }

    public Animator rareWheelAnimator;
    public Animator epicWheelAnimator;
    public Animator legendaryWheelAnimator;

    public Button rareSpinButton;          
    public Button epicSpinButton;          
    public Button legendarySpinButton;

    public float rareRate = 0.60f;
    public float epicRate = 0.25f;
    public float legendaryRate = 0.10f;

    private bool isSpinning = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        rareSpinButton.onClick.AddListener(() => TrySpin(rareRate, 1,  HeroGrade.Rare, rareWheelAnimator));
        epicSpinButton.onClick.AddListener(() => TrySpin(epicRate, 1, HeroGrade.Epic, epicWheelAnimator));
        legendarySpinButton.onClick.AddListener(() => TrySpin(legendaryRate, 2, HeroGrade.Legendary, legendaryWheelAnimator));
    }

    void TrySpin(float rate, int cost, HeroGrade grade, Animator wheelAnimator)
    {
        if (isSpinning) return;
        if (!GameManager.instance.SpendDiamonds(cost)) return;  
        StartCoroutine(SpinRoutine(rate, grade, wheelAnimator));
    }

    IEnumerator SpinRoutine(float rate, HeroGrade grade, Animator wheelAnimator)
    {
        isSpinning = true;

        wheelAnimator.SetTrigger("Run");

        var state = wheelAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length);

        if (Random.value < rate)
        {
            SummonManager.instance.SummonResult(grade);
        }
        else
        {
            Debug.Log($"{grade} ·ê·¿ ½ÇÆÐ!");
        }

        isSpinning = false;
    }
}
