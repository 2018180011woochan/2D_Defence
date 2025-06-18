using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Speed = 2f;
    public float Hp = 200f;

    private List<Transform> WayPoints;
    private int WayPointIndex = 0;
    private SpriteRenderer spriteRenderer;

    private Coroutine slowRoutine;

    public void Initialize(List<Transform> points)
    {
        
        WayPoints = points;
        transform.position = WayPoints[0].position;
    }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void ApplySlow(float factor, float duration)
    {
        if (slowRoutine != null) StopCoroutine(slowRoutine);
        slowRoutine = StartCoroutine(SlowCoroutine(factor, duration));
    }

    private IEnumerator SlowCoroutine(float factor, float duration)
    {
        float orig = Speed;
        Speed *= factor;
        yield return new WaitForSeconds(duration);
        Speed = orig;
        slowRoutine = null;
    }

    private void Update()
    {
        if (WayPoints == null) return;
        
        if (WayPointIndex >= WayPoints.Count)
        {
            WayPointIndex = 0;
        }

        // 방향 설정
        if (WayPointIndex == 3 || WayPointIndex == 0)
        {
            transform.localScale = new Vector3(-0.1f, 0.1f, 0.1f);
        }
        else
        {
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
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
        }
    }
}
