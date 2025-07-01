using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using System.Collections.Generic;

public class GhostBoss : Enemy
{
    [Header("Health Bar")]
    public GameObject healthBarPrefab;    // Canvas_MainUI 아래에 띄울 빨간 바 프리팹
    private RectTransform _canvasRect;    // Canvas_MainUI 의 RectTransform
    private RectTransform _barRect;       // 생성된 HealthBar UI 의 RectTransform
    private Camera _mainCam;
    private float _maxHp;

    private GameObject _healthBarGO;

    [Header("보스 공격")]
    public GameObject warningEffectPrefab;    // 공격 예고 이펙트
    public GameObject explosionEffectPrefab;  // 실제 공격 이펙트
    public int attackWarningCount = 5;        // 매 공격당 경고 셀 개수
    public float attackInterval = 5f;         // 공격 주기
    public float warningDuration = 2f;        // 경고 -> 폭발까지 딜레이

    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _maxHp = Hp;

        _mainCam = Camera.main;

        GameObject canvasGO = GameObject.Find("Canvas_MainUI");
        _canvasRect = canvasGO.GetComponent<RectTransform>();

        _healthBarGO = Instantiate(
            healthBarPrefab,
            canvasGO.transform,   
            false                 
        );
        _barRect = _healthBarGO.GetComponent<RectTransform>();
        StartCoroutine(BossAttackRoutine());
    }

    private void OnEnable()
    {
        
    }
    protected override void Update()
    {
        base.Update();

        if (WayPointIndex == 3 || WayPointIndex == 0)
        {
            transform.localScale = new Vector3(0.4f,
                                   0.4f,
                                   0.4f);
        }
        else
        {
            transform.localScale = new Vector3(0.4f * -1,
                                   0.4f,
                                   0.4f);
        }

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        Vector3 headWorld = transform.position + Vector3.up * 3.0f;
        Vector3 screenPos = _mainCam.WorldToScreenPoint(headWorld);
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect, screenPos, null, out localPos
        );

        if (transform.localScale.x < 0)
            localPos.x -= 80f;

        _barRect.anchoredPosition = localPos;

        float pct = Mathf.Clamp01(Hp / _maxHp);
        _barRect.localScale = new Vector3(pct, 1f, 1f);
    }

    private IEnumerator BossAttackRoutine()
    {
        var mgr = SummonManager.instance;

        while (true)
        {
            yield return new WaitForSeconds(attackInterval);

            

            var chosen = new HashSet<Vector2Int>();
            while (chosen.Count < attackWarningCount)
            {
                int r = Random.Range(0, mgr.rows);
                int c = Random.Range(0, mgr.cols);
                chosen.Add(new Vector2Int(r, c));
            }

            foreach (var idx in chosen)
            {
                Vector3 pos = mgr.GetCellWorldPosition(idx.x, idx.y);
                Instantiate(warningEffectPrefab, pos, Quaternion.identity);
            }
            _animator.SetTrigger("Attack");

            yield return new WaitForSeconds(warningDuration);

            foreach (var idx in chosen)
            {
                Vector3 pos = mgr.GetCellWorldPosition(idx.x, idx.y);
                Instantiate(explosionEffectPrefab, pos, Quaternion.identity);

                var cell = mgr.cellData[idx.x, idx.y];
                foreach (var hero in cell.instances)
                {
                    Destroy(hero);

                    GameManager.instance.setCurHeroCnt(
                        GameManager.instance.getHeroCnt() - 1);
                }
                cell.instances.Clear();
                cell.heroData = null;

                mgr.HideSellButton(idx.x, idx.y);
                mgr.HideCombineButton(idx.x, idx.y);

            }
        }
    }

    private void OnDisable()
    {
        if (_healthBarGO != null)
            Destroy(_healthBarGO);
        StopAllCoroutines();
    }
}