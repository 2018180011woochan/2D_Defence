using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance { get; set; }

    [Header("Monster Pool")]
    public GameObject monsterPrefab;
    public int monsterInitSize = 100;
    private Queue<GameObject> monsterPool = new Queue<GameObject>();

    [Header("Bee Bullet Pool")]
    public GameObject beeBulletPrefab;
    public int beeBulletPoolSize = 100;
    private Queue<GameObject> beeBulletPool = new Queue<GameObject>();

    [Header("Plant Bullet Pool")]
    public GameObject plantBulletPrefab;              
    public int plantBulletPoolSize = 100;
    private Queue<GameObject> plantBulletPool = new Queue<GameObject>();

    [Header("Trunk Bullet Pool")]
    public GameObject trunkBulletPrefab;
    public int trunkBulletPoolSize = 100;
    private Queue<GameObject> trunkBulletPool = new Queue<GameObject>();

    [Header("Damage Text Pool")]
    public GameObject damageTextPrefab;
    public int damageTextInitSize = 50;
    private Queue<GameObject> damageTextPool = new();

    [Header("Coin Popup Pool")]
    public GameObject coinPopPrefab;     
    public int coinPopPoolSize = 20;
    private Queue<GameObject> coinPopPool = new Queue<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitPool(monsterPool, monsterPrefab, monsterInitSize);
            InitPool(beeBulletPool, beeBulletPrefab, beeBulletPoolSize);
            InitPool(plantBulletPool, plantBulletPrefab, plantBulletPoolSize);
            InitPool(trunkBulletPool, trunkBulletPrefab, trunkBulletPoolSize);
            InitPool(damageTextPool, damageTextPrefab, damageTextInitSize);
            InitPool(coinPopPool, coinPopPrefab, coinPopPoolSize);
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

    public GameObject GetBeeBullet(Vector3 position)
    {
        return GetFromPool(beeBulletPool, beeBulletPrefab, position);
    }

    public GameObject GetPlantBullet(Vector3 position)
    {
        return GetFromPool(plantBulletPool, plantBulletPrefab, position);
    }

    public GameObject GetTrunkBullet(Vector3 position)
    {
        return GetFromPool(trunkBulletPool, trunkBulletPrefab, position);
    }

    public GameObject GetCoinPopup(Vector3 worldPos)
    {
        GameObject obj = coinPopPool.Count > 0 ? coinPopPool.Dequeue() : Instantiate(coinPopPrefab);
        obj.transform.position = worldPos;
        obj.SetActive(true);
        return obj;
    }

    public void ReleaseCoinPopup(GameObject go)
    {
        go.SetActive(false);
        coinPopPool.Enqueue(go);
    }

    public void ReleaseBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        if (bullet.GetComponent<BeeBullet>() != null)
            beeBulletPool.Enqueue(bullet);
        else if (bullet.GetComponent<PlantBullet>() != null)
            plantBulletPool.Enqueue(bullet);
        else if (bullet.GetComponent<TrunkBullet>() != null)
            plantBulletPool.Enqueue(bullet);
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
