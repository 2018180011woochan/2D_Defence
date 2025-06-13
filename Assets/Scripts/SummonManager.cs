using System;
using System.Collections.Generic;
using UnityEngine;

class HeroGroup
{
    public HeroData heroData;
    public List<GameObject> instances = new();
}

class CellData
{
    public List<HeroGroup> heroGroups = new();
}

public class SummonManager : MonoBehaviour
{
    public List<HeroData> NormalheroDatas;

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

    // ��ü �ʵ带 ��ȸ�ϸ鼭 ���� ���� �׷��� �ִ��� Ȯ���ϰ�
    // �� �׷쿡 ������ 3���� �̸��̸� �ش� �׷�� ��ġ�� ��ȯ
    private HeroGroup FindExistingGroupInField(HeroData hero, out Vector3 position)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                CellData cell = cellData[row, col];

                foreach (HeroGroup group in cell.heroGroups)
                {
                    bool sameName = group.heroData.heroName == hero.heroName;
                    bool sameGrade = group.heroData.grade == hero.grade;
                    bool notFull = group.instances.Count < 3;

                    if (sameName && sameGrade && notFull)
                    {
                        position = summonPos[row, col];
                        return group;
                    }
                }
            }
        }

        position = Vector3.zero;
        return null;
    }

    public void Summon()
    {
        // �ӽ÷� ��ȯ�� �Ϲ� ������ ��ȯ
        HeroData selectHero = NormalheroDatas[UnityEngine.Random.Range(0, NormalheroDatas.Count)];

        // �ʵ����� ���� ���� �׷��� �ִ��� Ȯ��
        Vector3 groupPos;
        HeroGroup existingGroup = FindExistingGroupInField(selectHero, out groupPos);

        if (existingGroup != null)
        {
            // ���� �׷��� �ִٸ� ���� �׷쿡 �߰� ��ȯ
            Vector3 offset = GetOffsetForGroup(existingGroup.instances.Count);
            GameObject go = Instantiate(selectHero.prefab, groupPos + offset, Quaternion.identity);
            SetShadowColor(go, selectHero.grade);
            existingGroup.instances.Add(go);

            Debug.Log($"<color=yellow>[������]</color> {selectHero.heroName} �׷쿡 �߰��� ({existingGroup.instances.Count}����)");
            return;
        }

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

        HeroGroup newGroup = new HeroGroup();
        newGroup.heroData = selectHero;
        newGroup.instances.Add(newObj);

        cellData[yindex, xindex].heroGroups.Add(newGroup);

        Debug.Log($"<color=green>[�� �׷�]</color> {selectHero.heroName} ���� ��ȯ��");

        xindex++;
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
}
