using UnityEngine;

public class PlantBullet : BaseBullet
{
    public float slowFactor = 0.5f;
    public float slowDuration = 2f;

    protected override void OnHit(Enemy enemy)
    {
        enemy.ApplySlow(slowFactor, slowDuration);
    }
}
