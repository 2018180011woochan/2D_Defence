using System.Collections.Generic;
using UnityEngine;

public class SummonManager : MonoBehaviour
{
    public List<HeroData> NormalheroDatas;

    public void Summon()
    {
        // �ӽ÷� ���� ��ġ ��ȯ 
        float randX = Random.Range(-10f, 10f);
        float randY = Random.Range(-4f, 3f);
        Vector3 spawnPos = new Vector3(randX, randY, 0f);

        // �ӽ÷� �븻��
        HeroData selectHero = NormalheroDatas[Random.Range(0, NormalheroDatas.Count)];

        GameObject go = Instantiate(selectHero.prefab, spawnPos, Quaternion.identity);

        Debug.Log($"<color=yellow>[��ȯ]</color> {selectHero.heroName} ��ȯ�� (���: {selectHero.grade})");
    }
}
