using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Player : MonoBehaviour
{
    
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    public int MaxHealth;
    public int Health;
    public float Speed = 2.0f;
    public float Damage = 1.0f;
    Vector2 moveInput;
    bool isDash = false;
    bool dashing = false;
    float dashCoolTime = 1.5f;
    float dashTime = 0.2f;
    public float dashSpeed = 8f;
    bool isAttack = false;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();


    }
    void Start()
    {
        MaxHealth = 3;
        Health = 3;
    }

    private void FixedUpdate()
    {
        

        PlayerMove();


        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && isDash == false)
        {
            StartCoroutine(Dash());
        }
        if (isAttack == false)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                StartCoroutine(Attack(new Vector2(-1, 0)));
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                StartCoroutine(Attack(new Vector2(1,0)));
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                StartCoroutine(Attack(new Vector2(0,-1)));
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                StartCoroutine(Attack(new Vector2(0, 1)));
            }
        }

        UpdateAnimator();
    }

    public void AttackHZ()
    {
        float horizontal = 0;
        float vertical = 0;

        if (Input.GetKey(KeyCode.UpArrow)) vertical += 1;
        else if (Input.GetKey(KeyCode.DownArrow)) vertical -= 1;

        if (Input.GetKey(KeyCode.LeftArrow)) horizontal -= 1;
        else if (Input.GetKey(KeyCode.RightArrow)) horizontal += 1;

        Vector2 AttackInput = (horizontal * Vector2.right + vertical * Vector2.up).normalized;

        myAnimator.SetFloat("AHZ", AttackInput.x);
        myAnimator.SetFloat("AVZ", AttackInput.y);


    }

    public void PlayerMove()
    {

        if (isAttack == true)
        {
            myRigidbody.linearVelocity = Vector2.zero;
            return;
        }

        if (dashing == true) return;

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
        myAnimator.SetBool("Run", true);
        myRigidbody.linearVelocity = new Vector2(moveInput.x * dashSpeed, moveInput.y * dashSpeed);

        yield return new WaitForSeconds(dashTime);

        dashing = false;
        myAnimator.SetBool("Run", false);
        myRigidbody.linearVelocity = moveInput * Speed;


        yield return new WaitForSeconds(dashCoolTime);

        isDash = false;
    }
    public IEnumerator Attack(Vector2 HVZ)
    {
        myAnimator.SetTrigger("A");
        isAttack = true;
        myAnimator.SetFloat("AHZ", HVZ.x);
        myAnimator.SetFloat("AVZ", HVZ.y);

        yield return new WaitForSeconds(0.7f);

        myAnimator.SetFloat("AHZ", 0);
        myAnimator.SetFloat("AVZ", 0);

        isAttack = false;

    }

    public void Test()
    {
        Debug.Log("Animation Event"); 
    }

}
