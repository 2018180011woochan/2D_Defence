using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MythicRecipe", menuName = "TowerDefense/MythicRecipe")]
public class MythicRecipe : ScriptableObject
{
    public HeroData resultHero;                
    public List<HeroData> requiredHeroes;      
    public int requiredCount = 1;
}
