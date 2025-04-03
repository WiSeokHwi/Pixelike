using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Player : MonoBehaviour
{
    public event Action OnHealthChanged;

    Rigidbody2D myRigidbody;
    Animator myAnimator;
    SpriteRenderer mySpriteRenderer;
    public LayerMask enemyLayer;
    public int MaxHealth { get; private set; }
    private int health;
    public int Health
    {
        get { return health; }
        set
        {
            health = Mathf.Clamp(value, 0, MaxHealth); // 체력 범위 제한
            OnHealthChanged?.Invoke(); // 체력이 변경될 때마다 이벤트 호출
        }
    }
    public float attackRange = 1.5f;
    public float Speed = 2.0f;
    public float Damage = 1.0f;
    Vector2 moveInput;
    bool isDash = false;
    bool dashing = false;
    public float dashingTime;
    public float dashCoolTime = 1.5f;
    float dashTime = 0.2f;
    public float dashSpeed = 8f;
    bool isAttack = false;
    bool isHit = false;
    public bool isDead = false;
    Vector2 HVZ;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        SpriteRenderer mySpriteRenderer = GetComponent<SpriteRenderer>();


    }
    void Start()
    {
        MaxHealth = 3;
        Health = 3;
        isDead = false;
        dashingTime = dashCoolTime;
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        PlayerMove();


        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        if (dashingTime < dashCoolTime)
        {
            dashingTime += Time.deltaTime; // 대시 타이머 증가
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && isDash == false && moveInput != Vector2.zero)
        {
            StartCoroutine(Dash());
        }
        if (isAttack == false)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                HVZ = new Vector2(-1, 0);
                StartCoroutine(Attack());
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                HVZ = new Vector2(1, 0);
                StartCoroutine(Attack());
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                HVZ = new Vector2(0, -1);
                StartCoroutine(Attack());
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                HVZ = new Vector2(0, 1);
                StartCoroutine(Attack());
            }
        }

        UpdateAnimator();
    }

    //public void AttackHZ()
    //{
    //    float horizontal = 0;
    //    float vertical = 0;

    //    if (Input.GetKey(KeyCode.UpArrow)) vertical += 1;
    //    else if (Input.GetKey(KeyCode.DownArrow)) vertical -= 1;

    //    if (Input.GetKey(KeyCode.LeftArrow)) horizontal -= 1;
    //    else if (Input.GetKey(KeyCode.RightArrow)) horizontal += 1;

    //    Vector2 AttackInput = (horizontal * Vector2.right + vertical * Vector2.up).normalized;

    //    myAnimator.SetFloat("AHZ", AttackInput.x);
    //    myAnimator.SetFloat("AVZ", AttackInput.y);


    //}

    public void PlayerMove()
    {

        if (isAttack == true || dashing == true || isHit == true) return;

        float horizontal = 0;
        float vertical = 0;

        if (Input.GetKey(KeyCode.W)) vertical += 1;
        else if (Input.GetKey(KeyCode.S)) vertical -= 1;

        if (Input.GetKey(KeyCode.A)) horizontal -= 1;
        else if (Input.GetKey(KeyCode.D)) horizontal += 1;

        moveInput = (horizontal * Vector2.right + vertical * Vector2.up).normalized;


        myRigidbody.linearVelocity = moveInput * Speed;
        //myAnimator.SetFloat("Speed", myRigidbody.linearVelocity.magnitude);
    }
    public void UpdateAnimator()
    {
        myAnimator.SetFloat("Horizontal", moveInput.x);
        myAnimator.SetFloat("Vertical", moveInput.y);
        
        if (moveInput != Vector2.zero)
        {
            myAnimator.SetBool("Walk", true);
        }
        else
        {
            myAnimator.SetBool("Walk", false);
        }


    }
    public IEnumerator Dash()
    {
        isDash = true;
        dashing = true;
        dashingTime = 0;

        myAnimator.SetBool("Run", true);
        myRigidbody.linearVelocity = new Vector2(moveInput.x * dashSpeed, moveInput.y * dashSpeed);

        yield return new WaitForSeconds(dashTime);

        dashing = false;
        myAnimator.SetBool("Run", false);
        myRigidbody.linearVelocity = moveInput * Speed;


        yield return new WaitForSeconds(dashCoolTime);

        isDash = false;
    }
    public IEnumerator Attack()
    {
        myAnimator.SetTrigger("A");
        isAttack = true;
        myAnimator.SetFloat("AHZ", HVZ.x);
        myAnimator.SetFloat("AVZ", HVZ.y);
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(attackRange, attackRange), 0f, HVZ, attackRange, enemyLayer); ;
        myRigidbody.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.3f);
        if (hit)
        {
            EnemyCTRL enemy = hit.collider.GetComponent<EnemyCTRL>();
            enemy.hit(Damage, transform.position);
        }
        yield return new WaitForSeconds(0.4f);

        myAnimator.SetFloat("AHZ", 0);
        myAnimator.SetFloat("AVZ", 0);

        isAttack = false;

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(isDead) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(Hit(collision.transform));
        }
    }


    IEnumerator Hit(Transform enemyTransForm)
    {
        
        Health -= 1;
        if (Health <= 0)
        {
            StartCoroutine(Death());
            yield break;
        }
        myAnimator.SetTrigger("Hit");
        isHit = true;
        Vector2 knockBackDirection = (transform.position - enemyTransForm.position).normalized;
        myRigidbody.linearVelocity = Vector2.zero;
        myRigidbody.AddForce(knockBackDirection * 2f, ForceMode2D.Impulse);
        StartCoroutine(BlinkEffect());
        yield return new WaitForSeconds(0.5f);
        isHit = false;
    }

    IEnumerator BlinkEffect()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        for (int i = 0; i < 5; i++)  // 5번 깜빡이기
        {
            spriteRenderer.color = new Color(1, 1, 1, 0);  // 투명
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = new Color(1, 1, 1, 1);  // 다시 원래 색
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator Death()
    {
        myAnimator.SetTrigger("Death");
        isDead = true;

        myRigidbody.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(1f);
        // 게임 오버 처리
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDrawGizmos()
    {
        if (isAttack)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position, new Vector2(attackRange,attackRange));
            Vector2 endPoint = (Vector2)transform.position + HVZ.normalized * attackRange; // 목표 위치

            // 시작 지점 박스 (이동 전)
            Gizmos.DrawWireCube(transform.position, new Vector2(attackRange, attackRange));

            // 끝 지점 박스 (이동 후)
            Gizmos.DrawWireCube(endPoint, new Vector2(attackRange, attackRange));

            // 이동 경로 표시 (선으로 연결)
            Gizmos.DrawLine(transform.position, endPoint);
        }

    }
}
