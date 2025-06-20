using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CellData
{
    public HeroData heroData;           // 이 칸에 있는 영웅 종류
    public List<GameObject> instances;  // 이 칸에 있는 영웅 인스턴스들

    public CellData()
    {
        instances = new List<GameObject>();
    }

    public bool IsEmpty => instances.Count == 0;
    public bool IsFull => instances.Count >= 3;
    public bool CanAddHero(HeroData newHero)
    {
        if (IsEmpty) return true;
        return heroData.heroName == newHero.heroName &&
               heroData.grade == newHero.grade &&
               !IsFull;
    }
}

public class SummonManager : MonoBehaviour
{
    public static SummonManager instance { get; set; }

    public List<HeroData> heroDatas;

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


    public float moveDuration = 0.5f;

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
        cellWidth = Math.Abs(endX - startX) / cols;
        cellHeight = Math.Abs(endY - startY) / rows;

        summonPos = new Vector3[rows, cols];
        cellData = new CellData[rows, cols];

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

    // 전체 필드를 순회하면서 같은 영웅이 있는 칸을 찾고
    // 그 칸에 영웅이 3마리 미만이면 해당 칸과 위치를 반환
    private CellData FindExistingCellInField(HeroData hero, out Vector3 position, out Vector2Int cellIndex)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                CellData cell = cellData[row, col];

                if (cell.CanAddHero(hero))
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

    public void Summon()
    {
        if (!GameManager.instance.DoSummon()) return;
        if (!SetUiSummon()) return;

        // 임시로 소환시 일반 영웅만 소환
        //HeroData selectHero = heroDatas[UnityEngine.Random.Range(0, heroDatas.Count)];
        HeroData selectHero = SelectRandomHero();

        // 필드위에 같은 영웅이 있는 칸이 있는지 확인
        Vector3 groupPos;
        Vector2Int cellIndex;
        CellData existingCell = FindExistingCellInField(selectHero, out groupPos, out cellIndex);

        if (existingCell != null)
        {
            // 같은 영웅이 있는 칸이 있다면 해당 칸에 추가 소환
            Vector3 offset = GetOffsetForGroup(existingCell.instances.Count);
            GameObject go = Instantiate(selectHero.prefab, groupPos + offset, Quaternion.identity);
            SetShadowColor(go, selectHero.grade);

            HeroSelectable selectable = go.GetComponent<HeroSelectable>();
            if (selectable != null)
            {
                selectable.groupCenterPosition = groupPos;
                selectable.heroData = selectHero;
                selectable.gridPos = cellIndex;
            }

            existingCell.instances.Add(go);
            // 첫 번째 영웅이라면 heroData 설정
            if (existingCell.heroData == null)
            {
                existingCell.heroData = selectHero;
            }

            return;
        }

        // 새로운 위치 찾기
        if (xindex >= cols)
        {
            xindex = 0;
            yindex++;
        }

        if (yindex >= rows)
        {
            Debug.Log("<color=red>더 이상 소환할 위치가 없습니다.</color>");
            return;
        }

        Vector3 spawnPos = summonPos[yindex, xindex];
        GameObject newObj = Instantiate(selectHero.prefab, spawnPos, Quaternion.identity);
        SetShadowColor(newObj, selectHero.grade);

        HeroSelectable selectable2 = newObj.GetComponent<HeroSelectable>();
        if (selectable2 != null)
        {
            selectable2.groupCenterPosition = spawnPos;
            selectable2.heroData = selectHero;
            selectable2.gridPos = new Vector2Int(yindex, xindex);
        }

        // 새로운 칸에 영웅 배치
        CellData newCell = cellData[yindex, xindex];
        newCell.heroData = selectHero;
        newCell.instances.Add(newObj);


        xindex++;
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

    private bool SetUiSummon()
    {
        if (GameManager.instance.getHeroCnt() >= GameManager.instance.getMaxHeroCnt())
        {
            Debug.Log("더이상 영웅을 소환할 수 없습니다.");
            return false;
        }
        GameManager.instance.setCurHeroCnt(GameManager.instance.getHeroCnt() + 1);
        return true;
    }

    private Vector3 GetOffsetForGroup(int index)
    {
        float offset = 0.4f;

        Vector3 result;

        switch (index)
        {
            case 0:
                result = Vector3.zero;
                break;
            case 1:
                result = new Vector3(-offset, -offset, 0f);
                break;
            case 2:
                result = new Vector3(offset, -offset, 0f);
                break;
            default:
                result = Vector3.zero;
                break;
        }

        return result;
    }

    private void SetShadowColor(GameObject heroObj, HeroGrade grade)
    {
        // 하위 오브젝트 중 이름이 Shadow인 자식 찾기
        Transform shadowTransform = heroObj.transform.Find("Shadow");
        if (shadowTransform == null) return;

        SpriteRenderer sr = shadowTransform.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        Color color = Color.gray;

        switch (grade)
        {
            case HeroGrade.Normal:
                color = Color.gray;
                break;
            case HeroGrade.Rare:
                color = new Color(0f, 0.5f, 1f); // 파랑
                break;
            case HeroGrade.Epic:
                color = new Color(0.6f, 0f, 0.9f); // 보라
                break;
            case HeroGrade.Legendary:
                color = Color.yellow;
                break;
            case HeroGrade.Mythic:
                color = new Color(1f, 0.3f, 0f); // 주황
                break;
        }

        sr.color = color;
    }

    /// 월드 좌표를 격자 인덱스로 변환
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

        // 각 영웅의 그룹 센터 기준 오프셋을 저장
        List<Vector3> fromOffsets = new List<Vector3>();
        List<Vector3> fromStartPositions = new List<Vector3>();
        foreach (GameObject hero in fromCell.instances)
        {
            fromOffsets.Add(hero.transform.position - fromPos);
            fromStartPositions.Add(hero.transform.position);
        }

        List<Vector3> toOffsets = new List<Vector3>();
        List<Vector3> toStartPositions = new List<Vector3>();
        if (!toCell.IsEmpty)
        {
            foreach (GameObject hero in toCell.instances)
            {
                toOffsets.Add(hero.transform.position - toPos);
                toStartPositions.Add(hero.transform.position);
            }
        }

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;

            // from 그룹을 to 위치로 애니메이션
            for (int i = 0; i < fromCell.instances.Count; i++)
            {
                Vector3 startPos = fromStartPositions[i];
                Vector3 targetPos = toPos + fromOffsets[i];
                fromCell.instances[i].transform.position = Vector3.Lerp(startPos, targetPos, t);
            }

            // to 그룹을 from 위치로 애니메이션 (있는 경우)
            if (!toCell.IsEmpty)
            {
                for (int i = 0; i < toCell.instances.Count; i++)
                {
                    Vector3 startPos = toStartPositions[i];
                    Vector3 targetPos = fromPos + toOffsets[i];
                    toCell.instances[i].transform.position = Vector3.Lerp(startPos, targetPos, t);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 애니메이션 완료 후 정확한 위치로 설정
        for (int i = 0; i < fromCell.instances.Count; i++)
        {
            fromCell.instances[i].transform.position = toPos + fromOffsets[i];

            HeroSelectable sel = fromCell.instances[i].GetComponent<HeroSelectable>();
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

                HeroSelectable sel = toCell.instances[i].GetComponent<HeroSelectable>();
                if (sel != null)
                {
                    sel.gridPos = from;
                    sel.groupCenterPosition = fromPos;
                }
            }

            // 데이터 교환
            var tempData = fromCell.heroData;
            var tempInstances = fromCell.instances;

            fromCell.heroData = toCell.heroData;
            fromCell.instances = toCell.instances;

            toCell.heroData = tempData;
            toCell.instances = tempInstances;
        }
        else
        {
            // to 셀이 비어있는 경우 - 단순 이동
            toCell.heroData = fromCell.heroData;
            toCell.instances = fromCell.instances;

            fromCell.heroData = null;
            fromCell.instances = new List<GameObject>();
        }
    }

    private bool IsValidCell(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < rows && cell.y >= 0 && cell.y < cols;
    }
}