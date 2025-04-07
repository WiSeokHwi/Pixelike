using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

public class Swaponer : MonoBehaviour
{
    public GameObject Player;
    private GameObject playerInstance;
    private EnemyCTRL EnemyCTRL;
    public GameObject monsterPrefab;
    public int hellmaxMonsters = 20;
    public int maxMonsters = 5; // �ִ� ���� ��
    public Collider2D[] spawnArea; // BoxCollider2D �Ǵ� PolygonCollider2D

    bool firstSpawn = true;
    public GameObject monsterHell;
    private Collider2D monsterHellCollider;
    public float minDistanceBetweenMonsters = 1.0f;
    public float swaponerdelay = 15;
    


    void Awake()
    {
        monsterHellCollider = monsterHell.GetComponent<Collider2D>();
        playerInstance = Instantiate(Player, new Vector3(0, 0, 0), Quaternion.identity);
        StartCoroutine(SpawnMonsters());
        
    }


    IEnumerator SpawnMonsters()
    {
        while (true)
        {
            if (monsterHell.transform.childCount < hellmaxMonsters)
            {
                float x = Random.Range(monsterHellCollider.bounds.min.x, monsterHellCollider.bounds.max.x);
                float y = Random.Range(monsterHellCollider.bounds.min.y, monsterHellCollider.bounds.max.y);
                Vector2 spawnPos = new Vector2(x, y);


                GameObject newMonster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
                newMonster.transform.parent = monsterHell.transform;

                EnemyCTRL enemyScript = newMonster.GetComponent<EnemyCTRL>();
                enemyScript.hellZone = monsterHell; // ���� �ݶ��̴�
                enemyScript.hellZoneCollider = monsterHellCollider;     // ���� �� �θ� ������Ʈ
                newMonster.SetActive(false); // ó������ ��Ȱ��ȭ
            }
            StartCoroutine(ReSpawnMonsters());
            yield return null;
        }

    }
    public IEnumerator ReSpawnMonsters()
    {
        
        
        while (monsterHell.transform.childCount < hellmaxMonsters)
        {
            yield return null;
        }

        if (!firstSpawn)
        {
            new WaitForSeconds(swaponerdelay);
        }
        else
        {
            firstSpawn = false;
        }

        while (true)
        {
            
            for (int i = 0; i < spawnArea.Length; i++)
            {
                
                if (spawnArea[i].transform.childCount < maxMonsters)
                {
                    Vector2 spawnPos = GetRandomPointInCollider(spawnArea[i]);

                    // ��ġ�� �ʴ��� Ȯ��
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
                        // ���� �̵����� ���� ���͸� �̵�
                        foreach (Transform monster in monsterHell.transform)
                        {
                            bool alreadyInArea = false;
                            foreach (Transform child in spawnArea[i].transform)
                            {
                                if (child == monster)
                                {
                                    alreadyInArea = true;
                                    break;
                                }
                            }

                            if (!alreadyInArea)
                            {
                                
                                monster.position = spawnPos;
                                monster.gameObject.SetActive(true);

                                Debug.Log("���� ����!!");
                                monster.parent = spawnArea[i].transform;
                                break;
                            }
                        }
                    }
                }
                
            }

        }

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
