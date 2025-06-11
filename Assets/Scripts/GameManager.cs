using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform WayPointParent;
    public GameObject EnemyPrefab;

    public List<Transform> WayPoints = new List<Transform>();

    public int Round = 80;
    public float roundTime = 15f;

    private void Awake()
    {
        foreach (Transform t in WayPointParent)
        {
            WayPoints.Add(t);
        }
    }

    private void Start()
    {

    }

    private IEnumerator SpawnWaves()
    {
        for (int curRound = 1; curRound <= Round; curRound++)
        {
            Debug.Log($"[Round {curRound}] 시작");
            float curTime = 0f;

            while (curTime < roundTime)
            {
                Vector3 spawnPos = WayPoints[0].position;

                GameObject enemy = MANAGER.PoolManager.Get(spawnPos);
                enemy.GetComponent<Enemy>().Initialize(WayPoints);

                yield return new WaitForSeconds(1f);
                curTime += 1f;
            }
            Debug.Log($"[Round {curRound}] 종료");
        }
    }
}
