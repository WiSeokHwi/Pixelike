using Mono.Cecil;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    public float moveSpeed = 1f; // 이동 속도

    private int phase = 0; // 보스의 현재 단계

    private Vector2 targetPosition; // 목표 위치

    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D mycollider;
    private GameObject Player;
    private Vector2 rayBoxSize = new Vector2(2f, 2f); // 레이캐스트 박스 크기
    private float castPadding = 0.5f; // 레이캐스트 패딩
    private float hitPadding = 5f; // 히트 패딩


    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        mycollider = GetComponent<Collider2D>();
        Player = GameObject.FindWithTag ("Player");
    }
    void Start()
    {
        targetPosition = Player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player == null)
        {
            Player = GameObject.FindWithTag("Player");
        }
        FindTarget(targetPosition);
    }

    private void MoveTo(Vector2 target)
    {
        rb.linearVelocity = (target - (Vector2)transform.position).normalized * moveSpeed;
    }

    private void FindTarget(Vector2 target)
    {
        
        
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetPosition) - castPadding;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, rayBoxSize, angle, direction, distance, LayerMask.GetMask("Wall"));
        if (!hit.collider)
        {
            // 장애물이 없으면 목표 위치로 이동
            targetPosition = Player.transform.position;
        }
        else
        {
            // 장애물이 있으면 목표 위치를 변경
            targetPosition = (Vector2)Player.transform.position + (Vector2)hit.normal * hitPadding;
            Debug.DrawLine(targetPosition - Vector2.one * 1f, targetPosition + Vector2.one * 1f, Color.red, 5f);
        }
        MoveTo(targetPosition);

    }

    void OnDrawGizmos()
    {
        if (Player == null) return;

        Vector2 origin = transform.position;
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetPosition) - castPadding;
        Vector2 castBoxSize = rayBoxSize; // 실제 사용되는 크기 그대로
        // 중심 좌표 계산
        Vector2 center = origin + direction * distance * 0.5f;
        // 회전 각도 계산 (방향 → 각도로 변환)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // 전체 길이만큼 박스 크기 확장 (가로 또는 세로로)
        Vector2 fullBoxSize = new Vector2(distance, castBoxSize.y); // 방향 따라 바꿔도 됨
        // 기즈모 색
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, fullBoxSize);
        //Gizmos.matrix = Matrix4x4.identity;
    }
}

