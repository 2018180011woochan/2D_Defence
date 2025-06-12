using UnityEngine;

public enum HeroGrade
{
    Normal,
    Rare,
    Epic,
    Legendary,
    Mythic
}
[CreateAssetMenu(fileName = "HeroData", menuName = "TowerDefense/HeroData")]
public class HeroData : ScriptableObject
{
    public string heroName;
    public GameObject prefab;
    public HeroGrade grade;
    public int attack;
    public float attackSpeed;
    public float range;
}
