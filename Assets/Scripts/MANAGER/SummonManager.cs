using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
    [Header("Combine Button UI")]
    public GameObject combineButtonPrefab;
    private CombineButton[,] combineButtons;


    [Header("소환 궤적")]
    public GameObject linePrefab;           
    public RectTransform summonButtonRT;     
    public RectTransform canvasRect;         
    public float lineThickness = 10f;         

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitGrid();
        }
        else Destroy(gameObject);
    }

    private void InitGrid()
    {
        cellWidth = Mathf.Abs(endX - startX) / cols;
        cellHeight = Mathf.Abs(endY - startY) / rows;
        summonPos = new Vector3[rows, cols];
        cellData = new CellData[rows, cols];
        sellButtons = new SellButton[rows, cols];
        combineButtons = new CombineButton[rows, cols];

        for (int r = 0; r < rows; r++)
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

    public int GetGroupCount(int row, int col)
    => cellData[row, col].instances.Count;

    public HeroGrade GetGroupGrade(int row, int col)
    => cellData[row, col].heroData.grade;

    /// <summary>기존 일반 소환 버튼에서 호출</summary>
    public void Summon()
    {
        if (!GameManager.instance.DoSummon()) return;
        if (!SetUiSummon()) return;
        HeroData selectHero = SelectRandomHero();
        SummonHero(selectHero);
    }

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

            StartCoroutine(ShowSummonLine(selectHero.grade, groupPos));
        }
        else
        {
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

            xindex++;
            StartCoroutine(ShowSummonLine(selectHero.grade, spawnPos));
        }


        

        var quick = FindObjectOfType<QuickMythUI>();
        if (quick != null) quick.Refresh();
    }

    public void SummonResult(HeroGrade grade)
    {
        HeroData baseHero = heroDatas[UnityEngine.Random.Range(0, heroDatas.Count)];
        HeroData runtimeHero = Instantiate(baseHero);
        runtimeHero.grade = grade;
        SummonHero(runtimeHero);
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

    public void ShowCombineButton(int row, int col)
    {
        if (combineButtons[row, col] == null)
        {
            GameObject go = Instantiate(combineButtonPrefab, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(GameObject.Find("Canvas_MainUI").transform, false);
            var cb = go.GetComponent<CombineButton>();
            cb.row = row;
            cb.col = col;
            combineButtons[row, col] = cb;
        }
        UpdateCombineButtonPosition(row, col);
    }

    public void HideCombineButton(int row, int col)
    {
        if (combineButtons[row, col] != null)
        {
            combineButtons[row, col].Hide();
            combineButtons[row, col] = null;
        }
    }

    private void UpdateSellButtonPosition(int row, int col)
    {
        Vector3 worldCenter = summonPos[row, col];
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldCenter + Vector3.up * 1.5f);

        RectTransform canvasRect = GameObject
            .Find("Canvas_MainUI")
            .GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, null, out Vector2 localPos);

        sellButtons[row, col]
            .GetComponent<RectTransform>().anchoredPosition = localPos;
    }

    private void UpdateCombineButtonPosition(int row, int col)
    {
        Vector3 worldCenter = summonPos[row, col];
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldCenter + Vector3.up * -2.0f);

        RectTransform canvasRect = GameObject
            .Find("Canvas_MainUI")
            .GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, null, out Vector2 localPos);

        combineButtons[row, col]
            .GetComponent<RectTransform>().anchoredPosition = localPos;
    }

    public void Combine(int row, int col)
    {
        var cell = cellData[row, col];
        int curCount = cell.instances.Count;

        HeroGrade curGrade = cell.heroData.grade;
        HeroGrade nextGrade = (HeroGrade)Mathf.Min((int)curGrade + 1, (int)HeroGrade.Mythic);
        if (nextGrade == HeroGrade.Mythic) return;

        string heroName = cell.heroData.heroName;

        // 그룹 내의 모든 영웅 제거 
        foreach (var go in cell.instances)
            Destroy(go);
        cell.instances.Clear();
        cell.heroData = null;

        // 랜덤 생성
        HeroData baseHero = heroDatas[UnityEngine.Random.Range(0, heroDatas.Count)];
        HeroData newHeroData = Instantiate(baseHero);
        newHeroData.grade = nextGrade;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var other = cellData[r, c];
                if (!other.IsEmpty
                    && other.heroData.heroName == newHeroData.heroName
                    && other.heroData.grade == nextGrade
                    && other.instances.Count < 3)
                {
                    // 편입할 그룹의 중심 및 오프셋 계산
                    Vector3 groupPos = summonPos[r, c];
                    Vector3 offset = GetOffsetForGroup(other.instances.Count);

                    // 인스턴스 생성 및 세팅
                    GameObject go = Instantiate(newHeroData.prefab, groupPos + offset, Quaternion.identity);
                    SetShadowColor(go, nextGrade);
                    var sel = go.GetComponent<HeroSelectable>();
                    sel.heroData = newHeroData;
                    sel.gridPos = new Vector2Int(r, c);
                    sel.groupCenterPosition = groupPos;

                    other.instances.Add(go);

                    StartCoroutine(ShowSummonLine(nextGrade, groupPos + offset));

                    // UI 업데이트 
                    GameManager gm = GameManager.instance;
                    gm.setCurHeroCnt(gm.getHeroCnt() - 2);
                    UIManager.instance.UpdateHeroCountText(
                        gm.getHeroCnt(), gm.getMaxHeroCnt());

                    HeroSelectionManager.instance.Deselect();
                    return;
                }
            }
        }

        // 편입되지 않은 경우 
        Vector3 center = summonPos[row, col];
        GameObject newGO = Instantiate(newHeroData.prefab, center, Quaternion.identity);
        SetShadowColor(newGO, nextGrade);

        // 영웅 정보 세팅
        var sel2 = newGO.GetComponent<HeroSelectable>();
        sel2.heroData = newHeroData;
        sel2.gridPos = new Vector2Int(row, col);
        sel2.groupCenterPosition = center;

        // 셀 데이터에 등록
        cell.heroData = newHeroData;
        cell.instances.Add(newGO);

        // UI 업데이트
        GameManager gm2 = GameManager.instance;
        gm2.setCurHeroCnt(gm2.getHeroCnt() - 2);
        UIManager.instance.UpdateHeroCountText(
            gm2.getHeroCnt(), gm2.getMaxHeroCnt());

        HeroSelectionManager.instance.Deselect();

        var quick = FindObjectOfType<QuickMythUI>();
        if (quick != null) quick.Refresh();
    }

    public void SellOne(int row, int col)
    {
        var cell = cellData[row, col];
        if (cell.IsEmpty) return;

        // 하나 제거
        var go = cell.instances[cell.instances.Count - 1];
        Destroy(go);
        cell.instances.RemoveAt(cell.instances.Count - 1);

        int refund = (int)cell.heroData.grade * 10;
        GameManager.instance.AddCoins(refund);

        // 남은 수 처리
        if (cell.instances.Count == 0)
        {
            cell.heroData = null;
            HideSellButton(row, col);
            HeroSelectionManager.instance.Deselect();
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

        HideCombineButton(row, col);
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
        {
            for (int col = 0; col < cols; col++)
            {
                var cell = cellData[row, col];
                if (!cell.IsEmpty && cell.CanAddHero(hero))
                {
                    position = summonPos[row, col];
                    cellIndex = new Vector2Int(row, col);
                    return cell;
                }
            }
        }

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                var cell = cellData[row, col];
                if (cell.IsEmpty)
                {
                    position = summonPos[row, col];
                    cellIndex = new Vector2Int(row, col);
                    return cell;
                }
            }
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
        var sc = heroObj.GetComponentInChildren<ShadowController>(true);
        if (sc == null) return;

        Color c;
        switch (grade)
        {
            case HeroGrade.Normal: c = Color.gray; break;
            case HeroGrade.Rare: c = new Color(0f, 0.5f, 1f); break;
            case HeroGrade.Epic: c = new Color(0.6f, 0f, 0.9f); break;
            case HeroGrade.Legendary: c = Color.yellow; break;
            case HeroGrade.Mythic: c = new Color(1f, 0.3f, 0f); break;
            default: c = Color.white; break;
        }

        sc.SetColor(c);
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

    public bool GetisHaveHero(HeroData hero)
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var cell = cellData[r, c];
                if (cell.heroData != null
                    && cell.heroData.heroName == hero.heroName
                    && cell.heroData.grade == hero.grade)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // 신화 소환 시 재료 영웅 지우기
    public bool RemoveOneHero(HeroData hero)
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var cell = cellData[r, c];
                if (!cell.IsEmpty &&
                    cell.heroData.heroName == hero.heroName &&
                    cell.heroData.grade == hero.grade)
                {
                    var go = cell.instances[cell.instances.Count - 1];
                    Destroy(go);
                    cell.instances.RemoveAt(cell.instances.Count - 1);

                    if (cell.instances.Count == 0)
                    {
                        cell.heroData = null;
                        HideSellButton(r, c);
                        HideCombineButton(r, c);
                    }
                    else
                    {
                        for (int i = 0; i < cell.instances.Count; i++)
                        {
                            cell.instances[i].transform.position =
                                summonPos[r, c] + GetOffsetForGroup(i);
                        }
                    }

                    GameManager.instance.setCurHeroCnt(
                        GameManager.instance.getHeroCnt() - 1);
                    UIManager.instance.UpdateHeroCountText(
                        GameManager.instance.getHeroCnt(),
                        GameManager.instance.getMaxHeroCnt());
                    return true;
                }
            }
        }
        return false;
    }

    public void SummonMythic(MythicRecipe recipe)
    {
        foreach (var req in recipe.requiredHeroes)
        {
            for (int i = 0; i < recipe.requiredCount; i++)
            {
                RemoveOneHero(req);
            }
        }

        HeroData runtime = Instantiate(recipe.resultHero);
        runtime.grade = HeroGrade.Mythic;
        SummonHero(runtime);

        GameManager.instance.setCurHeroCnt(
            GameManager.instance.getHeroCnt() + 1);
        UIManager.instance.UpdateHeroCountText(
            GameManager.instance.getHeroCnt(),
            GameManager.instance.getMaxHeroCnt());
    }

    public bool IsRecipeReady(MythicRecipe recipe)
    {
        foreach (var hero in recipe.requiredHeroes)
        {
            int have = 0;
            // 필드 전부 순회하며 heroData 일치 수 세기
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    var cell = cellData[r, c];
                    if (cell.heroData != null
                     && cell.heroData.heroName == hero.heroName
                     && cell.heroData.grade == hero.grade)
                    {
                        have += cell.instances.Count;
                    }
                }
            if (have < recipe.requiredCount)
                return false;
        }
        return true;
    }

    private IEnumerator ShowSummonLine(HeroGrade grade, Vector3 worldSpawnPos)
    {
        Vector2 btnScreen = RectTransformUtility
            .WorldToScreenPoint(null, summonButtonRT.position);
        Vector2 btnLocal = ToCanvasLocal(btnScreen);

        Vector2 spawnScreen = Camera.main.WorldToScreenPoint(worldSpawnPos);
        Vector2 spawnLocal = ToCanvasLocal(spawnScreen);

        Vector2 diff = spawnLocal - btnLocal;
        float distance = diff.magnitude;
        float angleDeg = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        var lineGO = Instantiate(linePrefab, canvasRect, false);
        var rt = lineGO.GetComponent<RectTransform>();
        rt.anchoredPosition = btnLocal + diff * 0.5f;
        rt.sizeDelta = new Vector2(distance, lineThickness);
        rt.localEulerAngles = new Vector3(0, 0, angleDeg);

        var img = lineGO.GetComponent<Image>();
        switch (grade)
        {
            case HeroGrade.Normal: img.color = Color.gray; break;
            case HeroGrade.Rare: img.color = Color.blue; break;
            case HeroGrade.Epic: img.color = new Color(0.6f, 0, 0.9f); break; 
            case HeroGrade.Legendary: img.color = Color.yellow; break;
            case HeroGrade.Mythic: img.color = new Color(1f, 0.3f, 0f); break;
        }

        yield return new WaitForSeconds(0.1f);
        Destroy(lineGO);
    }

    private Vector2 ToCanvasLocal(Vector2 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, null, out Vector2 localPos);
        return localPos;
    }
}
