using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class CellData
{
    public HeroData heroData;
    public List<GameObject> instances;

    public CellData()
    {
        instances = new List<GameObject>();
    }

    public bool IsEmpty => instances.Count == 0;
    public bool IsFull => instances.Count >= 3;

    public bool CanAddHero(HeroData newHero)
    {
        if (IsEmpty) return true;
        return heroData.heroName == newHero.heroName
            && heroData.grade == newHero.grade
            && !IsFull;
    }
}

public class SummonManager : MonoBehaviour
{
    public static SummonManager instance { get; private set; }

    [Header("Hero Data List")]
    public List<HeroData> heroDatas;

    [Header("Grid Settings")]
    public int rows = 3;
    public int cols = 7;
    public static float startX = -11f;
    public static float startY = 2.9f;
    public static float endX = 9.9f;
    public static float endY = -3.7f;

    private Vector3[,] summonPos;
    private CellData[,] cellData;
    private float cellWidth;
    private float cellHeight;
    private static int xindex = 0;
    private static int yindex = 0;

    [Header("Animation Settings")]
    public float moveDuration = 0.5f;

    [Header("Sell Button UI")]
    public GameObject sellButtonPrefab;
    private SellButton[,] sellButtons;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 그리드 초기화
        cellWidth = Mathf.Abs(endX - startX) / cols;
        cellHeight = Mathf.Abs(endY - startY) / rows;
        summonPos = new Vector3[rows, cols];
        cellData = new CellData[rows, cols];
        sellButtons = new SellButton[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                summonPos[r, c] = new Vector3(
                    startX + c * cellWidth + cellWidth / 2f,
                    startY - r * cellHeight - cellHeight / 2f,
                    0f
                );
                cellData[r, c] = new CellData();
            }
        }
    }

    /// <summary>기존 일반 소환 버튼에서 호출</summary>
    public void Summon()
    {
        if (!GameManager.instance.DoSummon()) return;
        if (!SetUiSummon()) return;
        HeroData selectHero = SelectRandomHero();
        SummonHero(selectHero);
    }

    /// <summary>실제 필드에 영웅을 배치</summary>
    public void SummonHero(HeroData selectHero)
    {
        // 같은 종류·등급 칸 검색
        Vector3 groupPos;
        Vector2Int cellIdx;
        CellData existing = FindExistingCellInField(selectHero, out groupPos, out cellIdx);

        if (existing != null)
        {
            // 그룹에 추가 배치
            Vector3 offset = GetOffsetForGroup(existing.instances.Count);
            GameObject go = Instantiate(selectHero.prefab, groupPos + offset, Quaternion.identity);
            SetShadowColor(go, selectHero.grade);

            var sel = go.GetComponent<HeroSelectable>();
            sel.groupCenterPosition = groupPos;
            sel.heroData = selectHero;
            sel.gridPos = cellIdx;

            existing.instances.Add(go);
            if (existing.heroData == null)
                existing.heroData = selectHero;

            // 버튼 위치 갱신
            //ShowSellButton(cellIdx.x, cellIdx.y);
            return;
        }

        // 빈 칸에 배치
        if (xindex >= cols)
        {
            xindex = 0;
            yindex++;
        }
        if (yindex >= rows)
        {
            Debug.LogWarning("필드에 더 이상 빈 칸이 없습니다!");
            return;
        }

        Vector3 spawnPos = summonPos[yindex, xindex];
        GameObject newObj = Instantiate(selectHero.prefab, spawnPos, Quaternion.identity);
        SetShadowColor(newObj, selectHero.grade);

        var sel2 = newObj.GetComponent<HeroSelectable>();
        sel2.groupCenterPosition = spawnPos;
        sel2.heroData = selectHero;
        sel2.gridPos = new Vector2Int(yindex, xindex);

        CellData newCell = cellData[yindex, xindex];
        newCell.heroData = selectHero;
        newCell.instances.Add(newObj);

        // 버튼 생성
        //ShowSellButton(yindex, xindex);

        xindex++;
    }

    /// <summary>룰렛 결과 호출용: 등급만 넘기면 랜덤 영웅 소환</summary>
    public void SummonResult(HeroGrade grade)
    {
        // 1) 랜덤 에셋 pick
        HeroData baseHero = heroDatas[UnityEngine.Random.Range(0, heroDatas.Count)];
        // 2) 복제본 만들어서 grade 덮어쓰기
        HeroData runtimeHero = Instantiate(baseHero);
        runtimeHero.grade = grade;
        // 3) 실제 소환
        SummonHero(runtimeHero);
        // 4) UI 갱신
        GameManager.instance.setCurHeroCnt(GameManager.instance.getHeroCnt() + 1);
        UIManager.instance.UpdateHeroCountText(
            GameManager.instance.getHeroCnt(),
            GameManager.instance.getMaxHeroCnt());
    }

    public void ShowSellButton(int row, int col)
    {
        if (sellButtons[row, col] == null)
        {
            GameObject go = Instantiate(sellButtonPrefab, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(GameObject.Find("Canvas_MainUI").transform, false);
            var sb = go.GetComponent<SellButton>();
            sb.row = row;
            sb.col = col;
            sellButtons[row, col] = sb;
        }
        UpdateSellButtonPosition(row, col);
    }

    public void HideSellButton(int row, int col)
    {
        if (sellButtons[row, col] != null)
        {
            sellButtons[row, col].Hide();
            sellButtons[row, col] = null;
        }
    }

    private void UpdateSellButtonPosition(int row, int col)
    {
        Vector3 worldCenter = summonPos[row, col];
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldCenter + Vector3.up * 0.5f);

        RectTransform canvasRect = GameObject
            .Find("Canvas_MainUI")
            .GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, null, out Vector2 localPos);

        sellButtons[row, col]
            .GetComponent<RectTransform>().anchoredPosition = localPos;
    }

    public void SellOne(int row, int col)
    {
        var cell = cellData[row, col];
        if (cell.IsEmpty) return;

        // 하나 제거
        var go = cell.instances[cell.instances.Count - 1];
        Destroy(go);
        cell.instances.RemoveAt(cell.instances.Count - 1);

        // 보상 예: grade * 10 코인
        int refund = (int)cell.heroData.grade * 10;
        GameManager.instance.AddCoins(refund);

        // 남은 수 처리
        if (cell.instances.Count == 0)
        {
            cell.heroData = null;
            HideSellButton(row, col);
        }
        else
        {
            for (int i = 0; i < cell.instances.Count; i++)
                cell.instances[i].transform.position =
                    summonPos[row, col] + GetOffsetForGroup(i);
            UpdateSellButtonPosition(row, col);
        }

        GameManager.instance.setCurHeroCnt(
            GameManager.instance.getHeroCnt() - 1);
        UIManager.instance.UpdateHeroCountText(
            GameManager.instance.getHeroCnt(),
            GameManager.instance.getMaxHeroCnt());
    }

    private HeroData SelectRandomHero()
    {
        HeroData baseHero = heroDatas[UnityEngine.Random.Range(0, heroDatas.Count)];
        HeroData runtimeHero = Instantiate(baseHero);

        int randVal = UnityEngine.Random.Range(0, 10);
        if (randVal < 5) runtimeHero.grade = HeroGrade.Normal;
        else if (randVal < 7) runtimeHero.grade = HeroGrade.Rare;
        else if (randVal == 8) runtimeHero.grade = HeroGrade.Epic;
        else runtimeHero.grade = HeroGrade.Legendary;

        return runtimeHero;
    }

    private CellData FindExistingCellInField(HeroData hero, out Vector3 position, out Vector2Int cellIndex)
    {
        for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
                if (cellData[row, col].CanAddHero(hero))
                {
                    position = summonPos[row, col];
                    cellIndex = new Vector2Int(row, col);
                    return cellData[row, col];
                }

        position = Vector3.zero;
        cellIndex = Vector2Int.zero;
        return null;
    }

    private bool SetUiSummon()
    {
        if (GameManager.instance.getHeroCnt() >= GameManager.instance.getMaxHeroCnt())
        {
            Debug.Log("더 이상 영웅을 소환할 수 없습니다.");
            return false;
        }
        GameManager.instance.setCurHeroCnt(GameManager.instance.getHeroCnt() + 1);
        return true;
    }

    private Vector3 GetOffsetForGroup(int index)
    {
        float offset = 0.4f;
        switch (index)
        {
            case 0: return Vector3.zero;
            case 1: return new Vector3(-offset, -offset, 0f);
            case 2: return new Vector3(offset, -offset, 0f);
            default: return Vector3.zero;
        }
    }

    private void SetShadowColor(GameObject heroObj, HeroGrade grade)
    {
        Transform shadow = heroObj.transform.Find("Shadow");
        if (shadow == null) return;
        SpriteRenderer sr = shadow.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        switch (grade)
        {
            case HeroGrade.Normal: sr.color = Color.gray; break;
            case HeroGrade.Rare: sr.color = new Color(0f, 0.5f, 1f); break;
            case HeroGrade.Epic: sr.color = new Color(0.6f, 0f, 0.9f); break;
            case HeroGrade.Legendary: sr.color = Color.yellow; break;
            case HeroGrade.Mythic: sr.color = new Color(1f, 0.3f, 0f); break;
        }
    }

    public Vector2Int GetCellIndexFromWorld(Vector3 worldPos)
    {
        int col = Mathf.FloorToInt((worldPos.x - startX) / cellWidth);
        int row = Mathf.FloorToInt((startY - worldPos.y) / cellHeight);
        return new Vector2Int(row, col);
    }

    public void TrySwapGroup(Vector2Int from, Vector2Int to)
    {
        if (!IsValidCell(from) || !IsValidCell(to)) return;
        var fromCell = cellData[from.x, from.y];
        var toCell = cellData[to.x, to.y];
        if (fromCell.IsEmpty) return;
        StartCoroutine(Swap(from, to, fromCell, toCell));
    }

    private IEnumerator Swap(Vector2Int from, Vector2Int to, CellData fromCell, CellData toCell)
    {
        Vector3 fromPos = summonPos[from.x, from.y];
        Vector3 toPos = summonPos[to.x, to.y];

        var fromOffsets = new List<Vector3>();
        var fromStartPositions = new List<Vector3>();
        foreach (var hero in fromCell.instances)
        {
            fromOffsets.Add(hero.transform.position - fromPos);
            fromStartPositions.Add(hero.transform.position);
        }

        var toOffsets = new List<Vector3>();
        var toStartPositions = new List<Vector3>();
        if (!toCell.IsEmpty)
        {
            foreach (var hero in toCell.instances)
            {
                toOffsets.Add(hero.transform.position - toPos);
                toStartPositions.Add(hero.transform.position);
            }
        }

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;

            for (int i = 0; i < fromCell.instances.Count; i++)
                fromCell.instances[i].transform.position =
                    Vector3.Lerp(fromStartPositions[i], toPos + fromOffsets[i], t);

            if (!toCell.IsEmpty)
                for (int i = 0; i < toCell.instances.Count; i++)
                    toCell.instances[i].transform.position =
                        Vector3.Lerp(toStartPositions[i], fromPos + toOffsets[i], t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 위치 최종 보정 및 데이터 교환
        for (int i = 0; i < fromCell.instances.Count; i++)
        {
            fromCell.instances[i].transform.position = toPos + fromOffsets[i];
            var sel = fromCell.instances[i].GetComponent<HeroSelectable>();
            if (sel != null)
            {
                sel.gridPos = to;
                sel.groupCenterPosition = toPos;
            }
        }

        if (!toCell.IsEmpty)
        {
            for (int i = 0; i < toCell.instances.Count; i++)
            {
                toCell.instances[i].transform.position = fromPos + toOffsets[i];
                var sel = toCell.instances[i].GetComponent<HeroSelectable>();
                if (sel != null)
                {
                    sel.gridPos = from;
                    sel.groupCenterPosition = fromPos;
                }
            }

            var tempData = fromCell.heroData;
            var tempInstances = fromCell.instances;

            fromCell.heroData = toCell.heroData;
            fromCell.instances = toCell.instances;

            toCell.heroData = tempData;
            toCell.instances = tempInstances;
        }
        else
        {
            toCell.heroData = fromCell.heroData;
            toCell.instances = fromCell.instances;
            fromCell.heroData = null;
            fromCell.instances = new List<GameObject>();
        }
    }

    private bool IsValidCell(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < rows
            && cell.y >= 0 && cell.y < cols;
    }
}
