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

    // 전체 필드를 순회하면서 같은 영웅 그룹이 있는지 확인하고
    // 그 그룹에 영웅이 3마리 미만이면 해당 그룹과 위치를 반환
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
        // 임시로 소환시 일반 영웅만 소환
        HeroData selectHero = NormalheroDatas[UnityEngine.Random.Range(0, NormalheroDatas.Count)];

        // 필드위에 같은 영웅 그룹이 있는지 확인
        Vector3 groupPos;
        HeroGroup existingGroup = FindExistingGroupInField(selectHero, out groupPos);

        if (existingGroup != null)
        {
            // 같은 그룹이 있다면 기존 그룹에 추가 소환
            Vector3 offset = GetOffsetForGroup(existingGroup.instances.Count);
            GameObject go = Instantiate(selectHero.prefab, groupPos + offset, Quaternion.identity);
            SetShadowColor(go, selectHero.grade);
            existingGroup.instances.Add(go);

            Debug.Log($"<color=yellow>[합쳐짐]</color> {selectHero.heroName} 그룹에 추가됨 ({existingGroup.instances.Count}마리)");
            return;
        }

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

        HeroGroup newGroup = new HeroGroup();
        newGroup.heroData = selectHero;
        newGroup.instances.Add(newObj);

        cellData[yindex, xindex].heroGroups.Add(newGroup);

        Debug.Log($"<color=green>[새 그룹]</color> {selectHero.heroName} 새로 소환됨");

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
}
