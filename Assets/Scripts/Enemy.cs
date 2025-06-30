using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Speed = 2f;
    private float _baseSpeed;
    public float Hp = 200f;
    private bool isStunned = false;
    private List<Transform> WayPoints;
    protected int WayPointIndex = 0;
    private SpriteRenderer spriteRenderer;
    private Vector3 _origScale;

    private Coroutine slowRoutine;
    private Coroutine stunRoutine;

    public void Initialize(List<Transform> points)
    {
        _baseSpeed = Speed;
        WayPoints = points;
        transform.position = WayPoints[0].position;
    }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _origScale = transform.localScale;
    }

    public void ApplySlow(float factor, float duration)
    {
        if (slowRoutine != null) StopCoroutine(slowRoutine);
        slowRoutine = StartCoroutine(SlowCoroutine(factor, duration));
    }

    private IEnumerator SlowCoroutine(float factor, float duration)
    {
        Speed = _baseSpeed * factor;     
        yield return new WaitForSeconds(duration);
        Speed = _baseSpeed;              
        slowRoutine = null;
    }

    protected virtual void Update()
    {
        if (isStunned) return;
        if (WayPoints == null) return;
        
        if (WayPointIndex >= WayPoints.Count)
        {
            WayPointIndex = 0;
        }

        // 방향 설정
        if (WayPointIndex == 3 || WayPointIndex == 0)
        {
            transform.localScale = new Vector3(_origScale.x * -1,
                                   _origScale.y,
                                   _origScale.z);
        }
        else
        {
            transform.localScale = new Vector3(_origScale.x,
                                   _origScale.y,
                                   _origScale.z);
        }

        Transform target = WayPoints[WayPointIndex];
        Vector3 dir = (target.position - transform.position).normalized;
        transform.Translate(dir * Speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
            WayPointIndex++;
    }

    public void GetDamage(float damage)
    {
        Hp -= damage;

        if (Hp <= 0f)
        {
            PoolManager.instance.ReleaseMonster(this.gameObject);
            GameManager.instance.AddCoins(2);

            int monsterCount = GameManager.instance.GetCurMonsterCnt();

            GameManager.instance.SetMonsterCnt(monsterCount - 1);
        }
    }

    public void Stun(float duration)
    {
        // 이미 기절 중이면 연장
        if (stunRoutine != null) StopCoroutine(stunRoutine);
        stunRoutine = StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
        stunRoutine = null;
    }
}
