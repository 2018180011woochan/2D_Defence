using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public GameObject bossPrefab;

    [Header("Round UI")]
    public GameObject roundUIPrefab;       
    private Transform _uiCanvas;

    public GameObject BossAppearPrefab;
    public GameObject GameOverNotionPrefab;
    public GameObject ResultUIPrefab;

    public bool isGameOver = false;
    private void Awake()
    {
        instance = this;
        foreach (Transform t in WayPointParent)
        {
            WayPoints.Add(t);
        }

        _uiCanvas = GameObject.Find("Canvas_MainUI").transform;
    }

    private void Start()
    {
        Coin = 30000;
        Diamond = 500;
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

    public bool SpendDiamonds(int diamond)
    {
        if (Diamond < diamond) return false;
        else
        {
            Diamond -= diamond;
            return true;
        }
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
        UIManager.instance.UpdateSummonPrice(StartSummonCoin + summonCnt * 2);
        return true;
    }

    private IEnumerator SpawnWaves()
    {
        for (int curRound = 1; curRound <= Round; curRound++)
        {
            ShowRoundUI(curRound);
            //bool isBossRound = (curRound % 5 == 0);
            bool isBossRound = (curRound == 2); // 테스트용


            UIManager.instance.UpdateRoundText(curRound);

            // 라운드 시작 시 현재 코인의 10%만큼 더해주기
            AddCoins(Coin / 10);

            Debug.Log($"[Round {curRound}] 시작");
            float curTime = 0f;

            if (isBossRound)
            {
                Instantiate(BossAppearPrefab, _uiCanvas, false);
                Vector3 bossSpawnPos = WayPoints[0].position;
                GameObject boss = Instantiate(bossPrefab, bossSpawnPos, Quaternion.identity);

                boss.GetComponent<Enemy>().Initialize(WayPoints);

                int bossTime = 5;
                while (bossTime > 0)
                {
                    UIManager.instance.UpdateTimerText(bossTime);
                    yield return new WaitForSeconds(1f);
                    bossTime--;
                }

                // 보스를 잡지 못했다면 게임 오버 로직 실행
                if (boss != null)
                {
                    isGameOver = true;
                    Instantiate(GameOverNotionPrefab, _uiCanvas, false);

                    yield return new WaitForSeconds(2f);

                    Instantiate(ResultUIPrefab, _uiCanvas, false);
                    yield break;
                }
            }
            else
            {
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

    private void ShowRoundUI(int curRound)
    {
        GameObject uiGo = Instantiate(roundUIPrefab, _uiCanvas, false);

        var tmp = uiGo.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = $"Round {curRound}";

    }
}
