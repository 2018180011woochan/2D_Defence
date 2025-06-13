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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitPool(monsterPool, monsterPrefab, monsterInitSize);
            InitPool(bulletPool, bulletPrefab, bulletInitSize);
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
