using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.EventSystems;


public class EnemyCTRL : MonoBehaviour
{
    public float maxHealth = 10f;               // 체력
    public float Health;                       // 현재 체력
    public float moveSpeed = 1f;            // 이동 속도
    public float detectionRange = 5f;       // 플레이어 감지 범위
    public float lostPlayerChaseTime = 5f;  // 마지막 감지 위치까지 쫓는 시간
    
    public LayerMask playerLayer;           // 플레이어 레이어
    public LayerMask obstacleLayer;         // 장애물(벽) 레이어
    public Vector2 patrolAreaSize = new Vector2(5f, 5f); // 순찰 범위
    public float attackRange = 2f; // 공격범위
    public Player playerObject;
    Animator animator;

    private Rigidbody2D rb;
    private Vector2 randomDestination;
    private Vector2 lastKnownPlayerPosition;
    private bool isChasingPlayer = false;
    private Vector2 PatolPosition;
    private bool lostPlayer = false;
    private bool isAttacking = false;
    private SpriteRenderer spriteRenderer;
    private Transform player;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        PatolPosition = rb.position;
        StartCoroutine(Patrol());
        Health = maxHealth;


    }

    void Update()
    {
        
        FindPlayer();
    }

    // 📌 플레이어 감지 및 이동 결정
    void FindPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);

        if (playerCollider && !playerObject.isDead)
        {
            player = playerCollider.transform;
            Vector2 direction = (player.position - transform.position).normalized;

            //  Enemy와 Player 사이 장애물 체크
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange, obstacleLayer);

            Debug.DrawRay(transform.position, direction * detectionRange, Color.cyan);

            if (hit.collider == null) // 장애물이 없으면 플레이어 쫓기
            {
                isChasingPlayer = true;
                lostPlayer = false;

                //Debug.Log(lastKnownPlayerPosition);
                MoveTo(player.position);
            }
            else // 장애물이 있으면 마지막 위치로 이동
            {
                lastKnownPlayerPosition = player.position;
                if (isChasingPlayer) // 플레이어를 쫓던 상태라면
                {
                    lostPlayer = true;
                    isChasingPlayer = false;
                    StartCoroutine(ChaseLastKnownPosition());
                }
            }
        }
        else if (!playerCollider && isChasingPlayer)
        {
            lostPlayer = true;
            isChasingPlayer = false;
            StartCoroutine(ChaseLastKnownPosition());
        }
        
    }

    //  마지막 플레이어 위치로 이동
    IEnumerator ChaseLastKnownPosition()
    {
        //Debug.Log("마지막 포지션으로 이동중");
        float timer = 0;
        while (timer < lostPlayerChaseTime)
        {
            MoveTo(lastKnownPlayerPosition);
            timer += Time.deltaTime;
            yield return null;
        }
        lostPlayer = false;
        PatolPosition = rb.position;
        StartCoroutine(Patrol()); // 다시 순찰 모드로 전환
    }

    //  Enemy 순찰 (랜덤 이동)
    IEnumerator Patrol()
    {
        while (!isChasingPlayer && !lostPlayer)
        {
            randomDestination = PatolPosition + new Vector2(Random.Range(-patrolAreaSize.x, patrolAreaSize.x), Random.Range(-patrolAreaSize.y, patrolAreaSize.y));
            MoveTo(randomDestination);
            yield return new WaitForSeconds(Random.Range(2f, 4f));
        }
    }

    //  이동 함수
    void MoveTo(Vector2 target)
    {
        if (isAttacking) return;
        Debug.Log("MoveTo!!!");
        Vector2 moveDirection = (target - (Vector2)transform.position).normalized;
        Collider2D attackRangeCollider = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);

        if (moveDirection.x < 0)
        {
            spriteRenderer.flipX = true; // 왼쪽 방향
        }
        else if (moveDirection.x > 0)
        {
            spriteRenderer.flipX = false; // 오른쪽 방향
        }

        if (attackRangeCollider)
        {
            Debug.Log("공격");
            StartCoroutine(Attack(target));
        }
        else
        {


            rb.linearVelocity = moveDirection * moveSpeed;
        }          
    }
    
    IEnumerator Attack(Vector2 target)
    {
        isAttacking = true;
        animator.SetTrigger("isAttack");
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.7f);
        Vector2 attackDirection = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = attackDirection * moveSpeed * 5;
        yield return new WaitForSeconds(0.6f);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1.2f);
        isAttacking = false;

    }
    IEnumerator hit()
    {

    }



    //  Gizmos로 감지 범위 표시
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(PatolPosition, patrolAreaSize * 2);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
