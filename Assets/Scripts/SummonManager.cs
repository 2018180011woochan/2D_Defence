using System.Collections.Generic;
using UnityEngine;

public class SummonManager : MonoBehaviour
{
    public List<HeroData> NormalheroDatas;

    public void Summon()
    {
        // 임시로 랜덤 위치 소환 
        float randX = Random.Range(-10f, 10f);
        float randY = Random.Range(-4f, 3f);
        Vector3 spawnPos = new Vector3(randX, randY, 0f);

        // 임시로 노말만
        HeroData selectHero = NormalheroDatas[Random.Range(0, NormalheroDatas.Count)];

        GameObject go = Instantiate(selectHero.prefab, spawnPos, Quaternion.identity);

        Debug.Log($"<color=yellow>[소환]</color> {selectHero.heroName} 소환됨 (등급: {selectHero.grade})");
    }
}
