using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Player : MonoBehaviour
{
    
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    public int MaxHealth = 3;
    public int CurrentHealth = 3;
    public float Speed = 3.0f;
    public float Damage = 1.0f;
    Vector2 moveInput;
    bool isDash = false;
    bool dashing = false;
    float dashCoolTime = 1.5f;
    float dashTime = 0.2f;
    public float dashSpeed = 15f;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();


    }
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        

        PlayerMove();
        UpdateAnimator();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && isDash == false)
        {
            Debug.Log("Dash Start");
            StartCoroutine(Dash());
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartCoroutine(Attack());
        }
    }

    public void PlayerMove()
    {

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
        myAnimator.SetFloat("Hmove", moveInput.x);
        myAnimator.SetFloat("Vmove", moveInput.y);

        if (moveInput.x > 0)
        {
            myAnimator.Play("Sword_Walk_side_right_Clip");
        }
        else if (moveInput.x < 0)
        {
            myAnimator.Play("Sword_Walk_side_left_Clip");
        }
        else if (moveInput.y > 0)
        {
            myAnimator.Play("Sword_Walk_back_Clip");
        }
        else if (moveInput.y < 0)
        {
            myAnimator.Play("Sword_Walk_front_Clip");
        }


        if (moveInput.x != 0 || moveInput.y != 0) 
        {
            myAnimator.SetBool("moveState", true);
        }
        else
        {
            myAnimator.SetBool("moveState", false);
        }

    }
    public IEnumerator Dash()
    {
        Debug.Log("Dash!");
        isDash = true;
        dashing = true;
        myRigidbody.linearVelocity = new Vector2(moveInput.x * dashSpeed, moveInput.y * dashSpeed);

        yield return new WaitForSeconds(dashTime);

        dashing = false;

        myRigidbody.linearVelocity = moveInput * Speed;


        yield return new WaitForSeconds(dashCoolTime);

        isDash = false;
    }
    public IEnumerator Attack()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            myAnimator.Play("Sword_Attack_left_Clip");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            myAnimator.Play("Sword_Attack_right_Clip");
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            myAnimator.Play("Sword_Attack_back_Clip");
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            myAnimator.Play("Sword_Attack_front_Clip");

            yield return null;

        }
    }

}
