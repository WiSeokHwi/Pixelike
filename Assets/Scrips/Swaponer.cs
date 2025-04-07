using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

[System.Serializable]
public class SpawnInfo
{
    public int areaIndex; // spawnArea의 인덱스
    public GameObject monsterPrefab;
    public int count;
}
public class Swaponer : MonoBehaviour
{
    public List<SpawnInfo> spawnInfos = new List<SpawnInfo>(); // 인스펙터에서 설정
    public GameObject Player;
    private GameObject playerInstance;
    private EnemyCTRL EnemyCTRL;
    public GameObject[] monsterPrefab;
    public int hellmaxMonsters = 20;
    public int maxMonsters = 5; // 최대 몬스터 수
    public Collider2D[] spawnArea; // BoxCollider2D 또는 PolygonCollider2D



    public GameObject monsterHell;
    private Collider2D monsterHellCollider;
    public float minDistanceBetweenMonsters = 1.0f;




    void Awake()
    {
        monsterHellCollider = monsterHell.GetComponent<Collider2D>();
        playerInstance = Instantiate(Player, new Vector3(0, 0, 0), Quaternion.identity);
        StartCoroutine(SpawnMonsters());

    }


    IEnumerator SpawnMonsters()
    {
        while (monsterHell.transform.childCount < hellmaxMonsters)
        {
            for (int i = 0; i < monsterPrefab.Count(); i++)
            {
                float x = Random.Range(monsterHellCollider.bounds.min.x, monsterHellCollider.bounds.max.x);
                float y = Random.Range(monsterHellCollider.bounds.min.y, monsterHellCollider.bounds.max.y);
                Vector2 spawnPos = new Vector2(x, y);


                GameObject newMonster = Instantiate(monsterPrefab[i], spawnPos, Quaternion.identity);
                newMonster.transform.parent = monsterHell.transform;

                EnemyCTRL enemyScript = newMonster.GetComponent<EnemyCTRL>();
                enemyScript.hellZone = monsterHell; // 헬존 콜라이더
                enemyScript.hellZoneCollider = monsterHellCollider;     // 몬스터 헬 부모 오브젝트
                newMonster.SetActive(false); // 처음에는 비활성화
            }
        }

        yield return StartCoroutine(ReSpawnMonsters());

    }

    public IEnumerator ReSpawnMonsters()
    {
        Debug.Log("리스폰 코루틴 시작");

        foreach (var info in spawnInfos)
        {
            Collider2D area = spawnArea[info.areaIndex];
            int spawned = 0;
            if (area.transform.childCount >= maxMonsters) continue;
            while (spawned < info.count)
            {
                Vector2 spawnPos = GetRandomPointInCollider(area);

                // 거리 체크
                bool canSpawn = true;
                foreach (Transform monster in monsterHell.transform)
                {
                    if (Vector2.Distance(monster.position, spawnPos) < minDistanceBetweenMonsters)
                    {
                        canSpawn = false;
                        break;
                    }
                }

                if (canSpawn)
                {
                    GameObject target = null;

                    // 비활성화 상태면서 prefab과 이름이 일치하는 몬스터 찾기
                    foreach (Transform monster in monsterHell.transform)
                    {
                        if (!monster.gameObject.activeInHierarchy &&
                            monster.name.Contains(info.monsterPrefab.name))
                        {
                            target = monster.gameObject;
                            break;
                        }
                    }

                    if (target != null)
                    {
                        target.transform.position = spawnPos;
                        target.SetActive(true);
                        target.transform.parent = area.transform;
                        spawned++;
                    }
                    else
                    {
                        Debug.LogWarning($"{info.monsterPrefab.name} 몬스터가 부족합니다.");
                        break;
                    }
                }

                yield return null;
            }
        }

        yield break;

        // 내부 함수: 콜라이더 안에서 랜덤 위치 구하기
        Vector2 GetRandomPointInCollider(Collider2D area)
        {
            Bounds bounds = area.bounds;
            Vector2 point;

            do
            {
                float x = Random.Range(bounds.min.x, bounds.max.x);
                float y = Random.Range(bounds.min.y, bounds.max.y);
                point = new Vector2(x, y);
            } while (!area.OverlapPoint(point));

            return point;
        }
    }
}
