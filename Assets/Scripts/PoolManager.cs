using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance { get; set; }

    [Header("Monster Pool")]
    public GameObject monsterPrefab;
    public int monsterInitSize = 100;
    private Queue<GameObject> monsterPool = new Queue<GameObject>();

    [Header("Bullet Pool")]
    public GameObject bulletPrefab;
    public int bulletInitSize = 100;
    private Queue<GameObject> bulletPool = new();

    [Header("Damage Text Pool")]
    public GameObject damageTextPrefab;
    public int damageTextInitSize = 50;
    private Queue<GameObject> damageTextPool = new();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitPool(monsterPool, monsterPrefab, monsterInitSize);
            InitPool(bulletPool, bulletPrefab, bulletInitSize);
            InitPool(damageTextPool, damageTextPrefab, damageTextInitSize);
        }
        else Destroy(gameObject);
    }

    private void InitPool(Queue<GameObject> pool, GameObject prefab, int count)
    {
        for (int i = 0; i < count; ++i)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetMonster(Vector3 position)
    {
        return GetFromPool(monsterPool, monsterPrefab, position);
    }

    public void ReleaseMonster(GameObject monster)
    {
        monster.SetActive(false);
        monsterPool.Enqueue(monster);
    }

    public GameObject GetBullet(Vector3 position)
    {
        return GetFromPool(bulletPool, bulletPrefab, position);
    }

    public void ReleaseBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }

    public GameObject GetDamageText(Vector3 position)
    {
        //return GetFromPool(damageTextPool, damageTextPrefab, position);

        GameObject obj = GetFromPool(damageTextPool, damageTextPrefab, position);

        // 부모를 Canvas_WorldUI로 설정
        Transform worldCanvas = GameObject.Find("Canvas_WorldUI")?.transform;
        if (worldCanvas != null)
            obj.transform.SetParent(worldCanvas, worldPositionStays: true);

        return obj;
    }

    public void ReleaseDamageText(GameObject damageText)
    {
        damageText.SetActive(false);
        damageTextPool.Enqueue(damageText);
    }

    private GameObject GetFromPool(Queue<GameObject> pool, GameObject prefab, Vector3 position)
    {
        GameObject obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = Instantiate(prefab);
        }
        obj.transform.position = position;
        obj.SetActive(true);
        return obj;
    }
}
