using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Swaponer : MonoBehaviour
{
    public GameObject Player;
    private GameObject playerInstance;

    public GameObject monsterPrefab;
    public int maxMonsters = 3;
    public Collider2D spawnArea; // BoxCollider2D 또는 PolygonCollider2D
    public float minDistanceBetweenMonsters = 1.0f;

    private List<GameObject> spawnedMonsters = new List<GameObject>();

    void Awake()
    {
        playerInstance = Instantiate(Player, new Vector3(0, 0, 0), Quaternion.identity);
    }
    void Start()
    {
        StartCoroutine(SpawnMonsters());
    }

    IEnumerator SpawnMonsters()
    {
        while (true)
        {
            if (spawnedMonsters.Count < maxMonsters)
            {
                Vector2 spawnPos = GetRandomPointInCollider();

                // 겹치지 않는지 확인
                bool canSpawn = true;
                foreach (var monster in spawnedMonsters)
                {
                    if (Vector2.Distance(monster.transform.position, spawnPos) < minDistanceBetweenMonsters)
                    {
                        canSpawn = false;
                        break;
                    }
                }

                if (canSpawn)
                {
                    GameObject newMonster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
                    spawnedMonsters.Add(newMonster);
                }
            }

            yield return new WaitForSeconds(1.0f); // 1초마다 시도
        }
        // Update is called once per frame
        Vector2 GetRandomPointInCollider()
        {
            Bounds bounds = spawnArea.bounds;
            Vector2 point;

            int maxTries = 10;
            int tries = 0;

            do
            {
                float x = Random.Range(bounds.min.x, bounds.max.x);
                float y = Random.Range(bounds.min.y, bounds.max.y);
                point = new Vector2(x, y);
                tries++;
            } while (!spawnArea.OverlapPoint(point) && tries < maxTries);

            return point;
        }
    }
}
