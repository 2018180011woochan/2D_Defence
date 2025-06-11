using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Speed = 2f;

    private List<Transform> WayPoints;
    private int WayPointIndex = 0;

    public void Initialize(List<Transform> points)
    {
        WayPoints = points;
        transform.position = WayPoints[0].position;
    }


    private void Update()
    {
        if (WayPoints == null)
        {
            Debug.Log("웨이포인트가 없음");
            return;
        }
        if (WayPointIndex >= WayPoints.Count)
        {
            WayPointIndex = 0;
        }

        Transform target = WayPoints[WayPointIndex];
        Vector3 dir = (target.position - transform.position).normalized;
        transform.Translate(dir * Speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
            WayPointIndex++;
    }
}
