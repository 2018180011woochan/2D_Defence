using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Transform WayPointParent;

    public List<Transform> WayPoints = new List<Transform>();

    public int Round = 80;
    public float roundTime = 15f;

    private int monsterCount = 0;

    private int StartSummonCoin = 30;
    private int Coin = 0;
    private int Diamond = 0;
    private int curHeroCnt = 0;
    private int maxHeroCnt = 28;
    private int summonCnt = 0;

    private void Awake()
    {
        instance = this;
        foreach (Transform t in WayPointParent)
        {
            WayPoints.Add(t);
        }
    }

    private void Start()
    {
        Coin = 300;
        Diamond = 5;
        UIManager.instance.UpdateCoinText(Coin);
        UIManager.instance.UpdateDiamondText(Diamond);
        UIManager.instance.UpdateHeroCountText(curHeroCnt, maxHeroCnt);

        StartCoroutine(SpawnWaves());
    }

    public int getMaxHeroCnt()
    {
        return maxHeroCnt;
    }

    public int getHeroCnt()
    {
        return curHeroCnt;
    }
    public void setCurHeroCnt(int heroCnt)
    {
        curHeroCnt = heroCnt;

        UIManager.instance.UpdateHeroCountText(curHeroCnt, maxHeroCnt);
    }

    public void AddCoins(int coin)
    {
        Coin += coin;
        UIManager.instance.UpdateCoinText(Coin);
    }

    public void AddDiamonds(int diamond)
    {
        Diamond += diamond;
        UIManager.instance.UpdateDiamondText(diamond);
    }

    public int GetCurMonsterCnt()
    {
        return monsterCount;
    }

    public void SetMonsterCnt(int monsterCnt)
    {
        monsterCount = monsterCnt;
    }

    public bool DoSummon()
    {
        if (Coin - (StartSummonCoin + summonCnt * 2) < 0) return false;
        Coin -= StartSummonCoin + summonCnt * 2;
        UIManager.instance.UpdateCoinText(Coin);
        summonCnt++;
        return true;
    }

    private IEnumerator SpawnWaves()
    {
        for (int curRound = 1; curRound <= Round; curRound++)
        {
            UIManager.instance.UpdateRoundText(curRound);

            // 라운드 시작 시 현재 코인의 10%만큼 더해주기
            AddCoins(Coin / 10);

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
