using UnityEngine;
using System.Collections;


public class EnemyCTRL : MonoBehaviour
{
    public float moveSpeed = 2f;            // 이동 속도
    public float detectionRange = 5f;       // 플레이어 감지 범위
    public float lostPlayerChaseTime = 3f;  // 마지막 감지 위치까지 쫓는 시간
    public LayerMask playerLayer;           // 플레이어 레이어
    public LayerMask obstacleLayer;         // 장애물(벽) 레이어
    public Vector2 patrolAreaSize = new Vector2(5f, 5f); // 순찰 범위

    private Rigidbody2D rb;
    private Vector2 randomDestination;
    private Vector2 lastKnownPlayerPosition;
    private bool isChasingPlayer = false;
    private bool lostPlayer = false;

    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(Patrol());
    }

    void Update()
    {
        FindPlayer();
    }

    // 📌 플레이어 감지 및 이동 결정
    void FindPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);

        if (playerCollider)
        {
            player = playerCollider.transform;
            Vector2 direction = (player.position - transform.position).normalized;

            // 🔎 Enemy와 Player 사이 장애물 체크
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange, obstacleLayer);

            if (hit.collider == null) // ✅ 장애물이 없으면 플레이어 쫓기
            {
                isChasingPlayer = true;
                lostPlayer = false;
                lastKnownPlayerPosition = player.position;
                MoveTo(player.position);
            }
            else // ❌ 장애물이 있으면 마지막 위치로 이동
            {
                if (isChasingPlayer) // 플레이어를 쫓던 상태라면
                {
                    lostPlayer = true;
                    isChasingPlayer = false;
                    StartCoroutine(ChaseLastKnownPosition());
                }
            }
        }
    }

    // 📌 마지막 플레이어 위치로 이동
    IEnumerator ChaseLastKnownPosition()
    {
        float timer = 0;
        while (timer < lostPlayerChaseTime)
        {
            MoveTo(lastKnownPlayerPosition);
            timer += Time.deltaTime;
            yield return null;
        }
        lostPlayer = false;
        StartCoroutine(Patrol()); // 다시 순찰 모드로 전환
    }

    // 📌 Enemy 순찰 (랜덤 이동)
    IEnumerator Patrol()
    {
        while (!isChasingPlayer && !lostPlayer)
        {
            randomDestination = (Vector2)transform.position + new Vector2(Random.Range(-patrolAreaSize.x, patrolAreaSize.x), Random.Range(-patrolAreaSize.y, patrolAreaSize.y));
            MoveTo(randomDestination);
            yield return new WaitForSeconds(Random.Range(2f, 4f));
        }
    }

    // 📌 이동 함수
    void MoveTo(Vector2 target)
    {
        Vector2 moveDirection = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    // 📌 Gizmos로 감지 범위 표시
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, patrolAreaSize * 2);
    }
}
