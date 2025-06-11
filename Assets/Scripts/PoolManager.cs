using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance { get; set; }

    public GameObject monsterPrefab;
    public int initSize = 30;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            for (int i = 0; i < initSize; ++i)
            {
                GameObject monster = Instantiate(monsterPrefab);
                monster.SetActive(false);
                pool.Enqueue(monster);
            }
        }
        else Destroy(gameObject);
    }

    public GameObject Get(Vector3 position)
    {
        GameObject getMonster;
        if (pool.Count > 0)
        {
            getMonster = pool.Dequeue();
            getMonster.SetActive(true);
        }
        else
        {
            getMonster = Instantiate(monsterPrefab);
        }
        getMonster.transform.position = position;
        return getMonster;
    }

    public void Release(GameObject monster)
    {
        monster.SetActive(false);
        pool.Enqueue(monster);
    }
}
