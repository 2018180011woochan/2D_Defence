using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform WayPointParent;

    public List<Transform> WayPoints = new List<Transform>();

    public int Round = 80;
    public float roundTime = 15f;

    private int monsterCount = 0;

    private void Awake()
    {
        foreach (Transform t in WayPointParent)
        {
            WayPoints.Add(t);
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        for (int curRound = 1; curRound <= Round; curRound++)
        {
            UIManager.instance.UpdateRoundText(curRound);

            Debug.Log($"[Round {curRound}] 시작");
            float curTime = 0f;

            while (curTime < roundTime)
            {
                Vector3 spawnPos = WayPoints[0].position;

                GameObject enemy = PoolManager.instance.GetMonster(spawnPos);
                enemy.GetComponent<Enemy>().Initialize(WayPoints);
                monsterCount++;

                MonsterBarUI.instance.UpdateMonsterCount(monsterCount);

                yield return new WaitForSeconds(1f);
                curTime += 1f;
                UIManager.instance.UpdateTimerText(roundTime - curTime);
            }
            Debug.Log($"[Round {curRound}] 종료");
        }
    }
}
