using UnityEngine;
using Unity.AI;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UIElements;
//using static UnityEngine.RuleTile.TilingRuleOutput;

public enum BossState
{
    Idle,
    Chasing,
    Attacking // 선택사항
}

public class BossFSMController : MonoBehaviour, IEnemy
{
    public float moveSpeed = 1f;
    public Vector2 rayBoxSize = new Vector2(2.5f, 2.5f);
    public float castPadding = 0.5f;
    public float hitPadding = 2f;
    public float detectRange = 4f;
    public float attackRange = 3f;
    public float MaxHealth = 250f;
    public float Health;

    // IEnemy 프로퍼티 구현
    float IEnemy.Health => Health;
    float IEnemy.MaxHealth => MaxHealth;
    public GameObject attackRangeIndicator;
    public GameObject hitEffectPrefab;

    private bool isattacking = false;
    public LayerMask playerLayer;

    private BossState currentState = BossState.Idle;
    private Transform Player;
    private Rigidbody2D rb;
    private float stateTimer = 0f;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Vector2 patrolAreaSize = new Vector2(10f,10f);
    private Vector2 PatrolPosition;
    private Vector2 randomDestination;
    Collider2D Hit;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        Setup(Player);
        PatrolPosition = rb.transform.position;
        animator = GetComponent<Animator>();
        Health = MaxHealth;
    }

    public void Setup(Transform Player)
    {
        this.Player = Player;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    private void Update()
    {
        Hit = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);
        if (Player == null)
        {
            Player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        }
        if (navMeshAgent == null)
        {
            Debug.Log("나브메쉬가없다");
            return;
        }
        
        if (!isattacking)
        {
            animator.SetFloat("Horizontal", navMeshAgent.velocity.x);
            animator.SetFloat("Vertical", navMeshAgent.velocity.y);
        }
        else
        {
            animator.SetFloat("Horizontal", rb.linearVelocity.x);
            animator.SetFloat("Vertical", rb.linearVelocity.y);
        }

        

        switch (currentState)
        {
            case BossState.Idle:
                HandleIdleState();
                break;
            case BossState.Chasing:
                HandleChasingState();
                break;
            case BossState.Attacking:
                HandleAttackingState();
                break;
        }
    }

    void MoveTo(Vector2 target)
    {
        if (isattacking) return;

        navMeshAgent.SetDestination(target);
        if (navMeshAgent.velocity.magnitude != 0)
        {
            animator.SetBool("isMove", true);
        }
        else
        {
            animator.SetBool("isMove", false);
        }
    }

    void HandleIdleState()
    {
        
        if (Hit)
        {
            currentState = BossState.Chasing;
            stateTimer = 0f;
        }
        else
        {
            stateTimer += Time.deltaTime;

            if (stateTimer >= 3f)
            {
                StartCoroutine(Patrol());
                stateTimer = 0f; // 다시 초기화해서 3초 간격 유지
                PatrolPosition = transform.position;
            }
        }
    }
    IEnumerator Patrol()
    {

        randomDestination = PatrolPosition + new Vector2(Random.Range(-patrolAreaSize.x / 2f, patrolAreaSize.x / 2f), Random.Range(-patrolAreaSize.y / 2f, patrolAreaSize.y / 2f));
        Debug.DrawRay(randomDestination - Vector2.one * 0.2f, randomDestination + Vector2.one * 0.2f, Color.red, 5f);
        yield return new WaitForSeconds(Random.Range(4f, 8f));

        MoveTo(randomDestination);


    }

    void HandleChasingState()
    {
        Collider2D attackRangeCol = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);

        if (attackRangeCol && !isattacking)
        {
            stateTimer += Time.deltaTime;

            if(stateTimer > Random.Range(3f, 10f))
            {
                currentState = BossState.Attacking;
                stateTimer = 0f;
            }
            
        }
        else
        {
            stateTimer = 0f;
        }


        MoveTo(Player.transform.position);
        if (!Hit)
        {
            stateTimer += Time.deltaTime;

            if (stateTimer >= 3f)
            {
                currentState = BossState.Idle;
                stateTimer = 0f;
            }
        }
    }


    void HandleAttackingState()
    {
        int rand = Random.Range(0, 100);
        if (!isattacking)
        {
            isattacking = true;

            switch (rand)
            {
                case < 50:
                    StartCoroutine(Attack2());
                    break;
                case >= 50:
                    StartCoroutine(Attack1());
                    break;
            }
        }


    }

    IEnumerator Attack1()
    {
        rb.linearVelocity = Vector2.zero;
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.enabled = false;
        Vector2 dashDir = (Player.transform.position - transform.position).normalized;
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("isDash");

        rb.AddForce(dashDir * 20f, ForceMode2D.Impulse);
        Debug.Log(rb.linearVelocity);
        stateTimer = 0;
        yield return new WaitUntil(() => { stateTimer += Time.deltaTime; return stateTimer >= 1f;});

        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(2f);
        rb.linearDamping = 0f;
        stateTimer = 0f;
        isattacking = false;
        navMeshAgent.enabled = true;
        currentState = BossState.Chasing;
    }

    IEnumerator Attack2()
    {
        
        rb.linearVelocity = Vector2.zero;
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.enabled = false;
        attackRangeIndicator.GetComponent<Collider2D>().enabled = false;
        attackRangeIndicator.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("isAttack");
        attackRangeIndicator.GetComponent<Collider2D>().enabled = true;
        yield return new WaitForSeconds(1f);
        attackRangeIndicator.SetActive(false);
        attackRangeIndicator.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(1f);
        navMeshAgent.enabled = true;
        isattacking = false;
        currentState = BossState.Chasing;
    }
    public void hit(float damage,Vector2 attakPosition)
    {
        Health -= damage;
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, attakPosition, Quaternion.identity);
        }
        if (Health <= 0)
        {
            Health = 0;
            navMeshAgent.enabled = false;
            attackRangeIndicator.GetComponent<Collider2D>().enabled = false;
            animator.SetTrigger("isDead");
            Destroy(gameObject, 2f);
        }

    }



    private void OnDrawGizmos()
    {
        if (Player == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(PatrolPosition, patrolAreaSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
