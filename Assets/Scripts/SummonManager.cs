using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CellData
{
    public HeroData heroData;           // �� ĭ�� �ִ� ���� ����
    public List<GameObject> instances;  // �� ĭ�� �ִ� ���� �ν��Ͻ���

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

    // ��ü �ʵ带 ��ȸ�ϸ鼭 ���� ������ �ִ� ĭ�� ã��
    // �� ĭ�� ������ 3���� �̸��̸� �ش� ĭ�� ��ġ�� ��ȯ
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

        // �ӽ÷� ��ȯ�� �Ϲ� ������ ��ȯ
        //HeroData selectHero = heroDatas[UnityEngine.Random.Range(0, heroDatas.Count)];
        HeroData selectHero = SelectRandomHero();

        // �ʵ����� ���� ������ �ִ� ĭ�� �ִ��� Ȯ��
        Vector3 groupPos;
        Vector2Int cellIndex;
        CellData existingCell = FindExistingCellInField(selectHero, out groupPos, out cellIndex);

        if (existingCell != null)
        {
            // ���� ������ �ִ� ĭ�� �ִٸ� �ش� ĭ�� �߰� ��ȯ
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
            // ù ��° �����̶�� heroData ����
            if (existingCell.heroData == null)
            {
                existingCell.heroData = selectHero;
            }

            return;
        }

        // ���ο� ��ġ ã��
        if (xindex >= cols)
        {
            xindex = 0;
            yindex++;
        }

        if (yindex >= rows)
        {
            Debug.Log("<color=red>�� �̻� ��ȯ�� ��ġ�� �����ϴ�.</color>");
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

        // ���ο� ĭ�� ���� ��ġ
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
            Debug.Log("���̻� ������ ��ȯ�� �� �����ϴ�.");
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
        // ���� ������Ʈ �� �̸��� Shadow�� �ڽ� ã��
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
                color = new Color(0f, 0.5f, 1f); // �Ķ�
                break;
            case HeroGrade.Epic:
                color = new Color(0.6f, 0f, 0.9f); // ����
                break;
            case HeroGrade.Legendary:
                color = Color.yellow;
                break;
            case HeroGrade.Mythic:
                color = new Color(1f, 0.3f, 0f); // ��Ȳ
                break;
        }

        sr.color = color;
    }

    /// ���� ��ǥ�� ���� �ε����� ��ȯ
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

        // �� ������ �׷� ���� ���� �������� ����
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

            // from �׷��� to ��ġ�� �ִϸ��̼�
            for (int i = 0; i < fromCell.instances.Count; i++)
            {
                Vector3 startPos = fromStartPositions[i];
                Vector3 targetPos = toPos + fromOffsets[i];
                fromCell.instances[i].transform.position = Vector3.Lerp(startPos, targetPos, t);
            }

            // to �׷��� from ��ġ�� �ִϸ��̼� (�ִ� ���)
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

        // �ִϸ��̼� �Ϸ� �� ��Ȯ�� ��ġ�� ����
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

            // ������ ��ȯ
            var tempData = fromCell.heroData;
            var tempInstances = fromCell.instances;

            fromCell.heroData = toCell.heroData;
            fromCell.instances = toCell.instances;

            toCell.heroData = tempData;
            toCell.instances = tempInstances;
        }
        else
        {
            // to ���� ����ִ� ��� - �ܼ� �̵�
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