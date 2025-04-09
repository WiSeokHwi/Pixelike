using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Linq;
using TMPro;
using static UnityEngine.UI.Image;


public class EnemyCTRL : MonoBehaviour, IEnemy
{
    public float MaxHealth = 10f;               // 체력
    public float Health;                       // 현재 체력
    public float moveSpeed = 1f;            // 이동 속도
    public float detectionRange = 5f;       // 플레이어 감지 범위
    public float lostPlayerChaseTime = 5f;  // 마지막 감지 위치까지 쫓는 시간
    private Vector2 rayBoxSize = new Vector2(1.5f, 1.5f); // 레이캐스트 박스 크기

    float IEnemy.Health => Health;
    float IEnemy.MaxHealth => MaxHealth;

    public LayerMask playerLayer;           // 플레이어 레이어
    public LayerMask obstacleLayer;         // 장애물(벽) 레이어
    public Vector2 patrolAreaSize = new Vector2(5f, 5f); // 순찰 범위
    public float attackRange = 2f; // 공격범위
    public Player playerObject;
    Animator animator;
    public GameObject[] item; 
    

    private Rigidbody2D rb;
    private Vector2 randomDestination;
    private Vector2 lastKnownPlayerPosition;
    private bool isChasingPlayer = false;
    private Vector2 PatolPosition;
    private bool lostPlayer = false;
    private bool isAttacking = false;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private bool isDead = false;
    private bool isHit = false;
    public GameObject hellZone;
    public Collider2D hellZoneCollider;
    public Swaponer swaponer;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        swaponer = GameObject.Find("Swaponer").GetComponent<Swaponer>();
    }

    protected virtual void OnEnable()
    {
        PatolPosition = rb.position;
        StartCoroutine(Patrol());
        Health = MaxHealth;
        isDead = false;
        rb.GetComponent<Collider2D>().enabled = true;

    }

    void Update()
    {
        if (isDead) return;
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

            float distance = Vector2.Distance(transform.position, player.position);
            Vector2 fullBoxSize = new Vector2(distance, rayBoxSize.y); // 방향 따라 바꿔도 됨
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, fullBoxSize, angle, direction, distance, LayerMask.GetMask("Wall"));


            if (hit.collider == null) // 장애물이 없으면 플레이어 쫓기
            {
                isChasingPlayer = true;
                lostPlayer = false;

                //Debug.Log(lastKnownPlayerPosition);
                MoveTo(player.position);
            }
            else // 장애물이 있으면 마지막 위치로 이동
            {
                if (isChasingPlayer)
                {
                    lastKnownPlayerPosition = player.position;
                    Debug.DrawLine(lastKnownPlayerPosition - Vector2.one * 1f, lastKnownPlayerPosition + Vector2.one * 1f, Color.red, 5f);
                    Debug.Log("장애물 발견");
                }
                
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
            lastKnownPlayerPosition = player.position;
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
            yield return new WaitForSeconds(Random.Range(4f, 8f));
        }
    }

    //  이동 함수
    void MoveTo(Vector2 target)
    {
        if (isAttacking || isHit) return;
        //Debug.Log("MoveTo!!!");
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
            StartCoroutine(Attack(target));
        }
        else
        {


            rb.linearVelocity = moveDirection * moveSpeed;
        }          
    }
    
    IEnumerator Attack(Vector2 target)
    {
        if (isHit) yield break;
        isAttacking = true;
        animator.SetTrigger("isAttack");
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.7f);
        if (isHit) yield break;
        Vector2 attackDirection = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = attackDirection * moveSpeed * 5;
        yield return new WaitForSeconds(0.6f);
        if (isHit) yield break;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1.2f);
        isAttacking = false;

    }
    public void hit(float DMG, Vector2 hitDirection)
    {
        if (isDead) return;
        StartCoroutine(hit_(DMG, hitDirection));
    }
    IEnumerator hit_(float DMG, Vector2 hitDirection)
    {
        Health -= DMG;
        if(Health <= 0)
        {
            StartCoroutine(Dead());
            yield break;
        }
        
        isHit = true;
        isAttacking = false;
        animator.SetTrigger("Hit");
        rb.AddForce(((Vector2)transform.position - hitDirection).normalized * 2, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.8f);
        isHit = false;

    }
    protected virtual int dropChans()
    {
        return Random.Range(0, 100);
    }
    protected virtual IEnumerator Dead()
    {
        animator.SetTrigger("Dead");
        rb.GetComponent<Collider2D>().enabled = false;
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        
        yield return new WaitForSeconds(1f);
        
        if (dropChans() >= 50)
        {
            Instantiate(item[Random.Range(0, item.Count())], transform.position, Quaternion.identity);
        }
        

        transform.position = new Vector2(
            Random.Range(hellZoneCollider.bounds.min.x, hellZoneCollider.bounds.max.x), 
            Random.Range(hellZoneCollider.bounds.min.y, hellZoneCollider.bounds.max.y));

        // 부모를 monsterHell로 설정
        transform.parent = hellZone.transform;

        yield return new WaitForSeconds(20f);

        StartCoroutine(swaponer.ReSpawnMonsters());
        // 다시 초기화
        gameObject.SetActive(false); // 비활성화 후 나중에 스폰 시 재활성화

        

        
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

        if (player == null) return;
        Vector2 origin = transform.position;
        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);
        // 중심점 계산 (전체 Box 영역의 중심)
        Vector2 center = origin + direction * distance * 0.5f;

        // 회전 각도 계산 (방향 → 각도로 변환)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 전체 길이만큼 박스 크기 확장 (가로 또는 세로로)
        Vector2 fullBoxSize = new Vector2(distance, rayBoxSize.y); // 방향 따라 바꿔도 됨

        // 회전과 위치 반영
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, fullBoxSize); // 회전은 이렇게 적용됨!
        //Gizmos.matrix = Matrix4x4.identity; // 원래대로 되돌리기
    }
}
