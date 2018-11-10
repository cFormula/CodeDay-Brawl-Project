using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    
    public KeyCode Attack;
    public KeyCode Special;
    public KeyCode Jump;
    public KeyCode DashLeft;
    public KeyCode DashRight;

    public float walkSpeed;
    public float runSpeed;
    public float jumpHeight;
    public float DashDistance = 5f;
    public float startDashTime;
    private float dashTime;


    private bool isSprinting;

    Rigidbody mainRB;
    Animator mainAnim;

    bool facingRight;

    float ButtonCooldown = 0.2f;
    int ButtonCount = 0;

    bool grounded = false;
    Collider[] groundCollisions;
    float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    
    bool CanDoubleJump;

    // Use this for initialization
    void Start () {
        mainRB = GetComponent<Rigidbody>();
        mainAnim = GetComponent<Animator>();
        facingRight = true;
        CanDoubleJump = true;
        dashTime = startDashTime;
	}
	
	// Update is called once per frame
	void Update () {
      
    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Horizontal");
        mainAnim.SetFloat("Speed", Mathf.Abs(move));

        //Jump Input

        if (grounded && Input.GetKeyDown(Jump))
        {
            grounded = false;
            mainRB.AddForce(new Vector3(0, jumpHeight, 0));
            CanDoubleJump = true;
            mainAnim.SetTrigger("Jump");
        } else if (CanDoubleJump && Input.GetKeyDown(Jump))
        {
            mainRB.velocity = new Vector3(mainRB.velocity.x, 0, mainRB.velocity.z);
            mainRB.AddForce(new Vector3(0, jumpHeight, 0));
            mainAnim.SetTrigger("Double Jump");
            CanDoubleJump = false;

        }

        //Ground Check

        groundCollisions = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer);
        if (groundCollisions.Length > 0)
        {
            grounded = true;
            CanDoubleJump = true;
            //mainAnim.ResetTrigger("Double Jump");
        }
        else
        {
            grounded = false;
        }

        //Dash Input


        if (Input.GetKeyDown(DashLeft))
        {
            mainAnim.SetTrigger("DashL");
            //Vector3 dashVelocity = Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * mainRB.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * mainRB.drag + 1)) / -Time.deltaTime)));
            //mainRB.AddForce(dashVelocity, ForceMode.VelocityChange);
            mainRB.AddForce(Vector2.left * DashDistance);
        }
        else if (Input.GetKeyDown(DashRight))
        {
            mainAnim.SetTrigger("DashR");
            mainRB.AddForce(Vector2.right * DashDistance);
        }


        if (isSprinting == false) { 
            mainRB.velocity = new Vector3(0, mainRB.velocity.y, move * walkSpeed);
        } else if (isSprinting == true)
        {
            mainRB.velocity = new Vector3(0, mainRB.velocity.y, move * runSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) { 
            if (ButtonCooldown > 0 && ButtonCount == 1)
            {
                isSprinting = true;
                
            } else
            {
                ButtonCooldown = 0.5f;
                ButtonCount += 1;
            }

            if (ButtonCooldown > 0)
            {

                ButtonCooldown -= 1 * Time.deltaTime;

            }
            else
            {
                ButtonCount = 0;
                isSprinting = false;
            }
        } else 
        {
            isSprinting = false;
            mainAnim.SetBool("Sprinting", false);
        }

        if (move > 0)
        {
            Flip();
        } else if (move < 0)
        {
            Flip();
        }

        //Animator Parameters

        mainAnim.SetBool("Grounded", grounded);
        mainAnim.SetFloat("verticalSpeed", mainRB.velocity.y);
        mainAnim.SetBool("Sprinting", isSprinting);

    }

    void Flip()
    {
        transform.eulerAngles = new Vector3(0, -1 * transform.eulerAngles.y, 0);
    }
}
