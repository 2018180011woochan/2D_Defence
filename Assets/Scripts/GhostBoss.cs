using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

public class GhostBoss : Enemy
{
    [Header("Health Bar")]
    public GameObject healthBarPrefab;    // Canvas_MainUI 아래에 띄울 빨간 바 프리팹
    private RectTransform _canvasRect;    // Canvas_MainUI 의 RectTransform
    private RectTransform _barRect;       // 생성된 HealthBar UI 의 RectTransform
    private Camera _mainCam;
    private float _maxHp;

    private GameObject _healthBarGO;
    void Start()
    {
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
    }

    protected override void Update()
    {
        // 1) 기본 Enemy 이동/스턴 로직
        base.Update();

        // 2) HP 바 업데이트
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        Vector3 headWorld = transform.position + Vector3.up * 4.0f;
        Vector3 screenPos = _mainCam.WorldToScreenPoint(headWorld);
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect, screenPos, null, out localPos
        );
        _barRect.anchoredPosition = localPos;

        float pct = Mathf.Clamp01(Hp / _maxHp);
        _barRect.localScale = new Vector3(pct, 1f, 1f);
    }
    private void OnDisable()
    {
        if (_healthBarGO != null)
            Destroy(_healthBarGO);
    }
}