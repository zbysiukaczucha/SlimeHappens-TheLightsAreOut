using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementLO : MonoBehaviour
{
    [Header("##  GENERAL  ##")]
    private Rigidbody2D playerRigid;
    private BoxCollider2D boxCollider;
    private LayerMask groundLayer;
    private LayerMask edgeLayer;
    private GameManager gameManager;
    private BackgroundMusic bgMusic;
    
    private bool lookingRight = true;
    private bool isIdle;

    [Header("##  DASH  ##")]
    private bool canDash = true;
    private float dashTime = 10;

    [Header("## HEIGHT LIMIT ##")]
    public float minY = 0f;
    public float maxY = 2.5f;


    [Header("##  PUBLIC VARIABLES  ##")]
    [ShowOnly] public Animator anim;
    [ShowOnly] public float baseSpeed = 9;
    [ShowOnly] public float speed;
    [ShowOnly] public bool isDashing = false;
    [ShowOnly] public float dashCooldown = 1;

    [Header("## MOVEMENT ##")]
    public InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction dashAction;
    public float velocityX;
    public float velocityY;
    private float friction = 40f;
    private float acceleration = 35f;
    [ShowOnly] public bool canMove;

    void Awake()
    {
        canMove = false;
        anim = GetComponent<Animator>();
        playerRigid = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
        edgeLayer = 1 << LayerMask.NameToLayer("Edge");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        bgMusic = GameObject.Find("Audio Source").GetComponent<BackgroundMusic>();

        speed = baseSpeed;

        var playerActionMap = inputActions.FindActionMap("Player");
        moveAction = playerActionMap.FindAction("Move");
        dashAction = playerActionMap.FindAction("Dash");

        moveAction.Enable();
        dashAction.Enable();
    }

    void Update()
    {
        if (isDashing || gameManager.gameOver)
            return;
        
        if(!anim.GetBool("Finished_Falling_Touchdown"))
            finishedFalling();

        if (isTouchingWall() && playerRigid.linearVelocity.y == 0)
        {
            anim.ResetTrigger("AD_Pressed");
            anim.SetTrigger("AD_Dropped");
            isIdle = true;
        }
        else
            isIdle = false;

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        float moveX = moveInput.x;
        float moveY = moveInput.y;
        float targetSpeedX = moveInput.x * speed;
        float targetSpeedY = moveInput.y * speed * 0.75f;

        
        // X MOVEMENT
        if (Mathf.Abs(moveX) > 0.1f && canMove)
            velocityX = Mathf.MoveTowards(velocityX, targetSpeedX, acceleration * Time.deltaTime);
        else
            velocityX = Mathf.MoveTowards(velocityX, 0, friction * Time.deltaTime);

        // Y MOVEMENT
        if (Mathf.Abs(moveY) > 0.1f && canMove)
            velocityY = Mathf.MoveTowards(velocityY, targetSpeedY, acceleration * Time.deltaTime);
        else
            velocityY = Mathf.MoveTowards(velocityY, 0, friction * Time.deltaTime);

        
        // MOVE
        if (anim.GetBool("Finished_Falling_Touchdown"))
        {
            playerRigid.linearVelocity = new Vector2(velocityX, velocityY);
            
            // Clamping to height limits
            var pos = transform.position;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            transform.position = pos;

            playerRigid.gravityScale = 0f;

            if (lookingRight)
                transform.localScale = new Vector3(-0.2f + pos.y * 0.01f, 0.2f - pos.y * 0.01f, 0.2f - pos.y * 0.01f);
            else
                transform.localScale = new Vector3(0.2f - pos.y * 0.01f, 0.2f - pos.y * 0.01f, 0.2f - pos.y * 0.01f);
        }
        
        if ((playerRigid.linearVelocity.x == 0 && playerRigid.linearVelocity.y == 0) ||
        (transform.position.y == minY && playerRigid.linearVelocity.y != 0 && playerRigid.linearVelocity.x == 0) || 
        (transform.position.y == maxY && playerRigid.linearVelocity.y != 0 && playerRigid.linearVelocity.x == 0))
        {
            anim.ResetTrigger("AD_Pressed");
            anim.SetTrigger("AD_Dropped");
        }
        else
        {
            if (!isIdle)
            {
                anim.ResetTrigger("AD_Dropped");
                anim.SetTrigger("AD_Pressed");
            }
        }

        if (anim.GetBool("Finished_Falling_Touchdown") && (moveX > 0 && !lookingRight || moveX < 0 && lookingRight))
            Flip();

        if (dashAction.WasPerformedThisFrame() && canDash)
            StartCoroutine(Dash());
    }

    public bool isGrounded()
    {
        RaycastHit2D raycast = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.5f, groundLayer);
        return raycast.collider != null;
    }

    private bool isTouchingWall()
    {
        RaycastHit2D raycast = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.right, 0.1f, edgeLayer);
        RaycastHit2D raycast2 = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.left, 0.1f, edgeLayer);

        return raycast.collider != null || raycast2.collider != null;
    }

    public void finishedFalling()
    {
        if (isGrounded()){
            anim.SetBool("Finished_Falling", true);
            playerRigid.linearVelocity = new Vector2(0, 0);
            playerRigid.gravityScale = 0f;
            StartCoroutine(FinishedFallingTouchdown());
        }
        if (transform.position.y < 0)
        {
            Vector3 pos = transform.position;
            pos.y = 0f;
            transform.position = pos;
        }

    }

    void Flip()
    {
        lookingRight = !lookingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    

    public void DisableMovement()
    {
        StartCoroutine(DisableMovementCor());
    }
    
    private IEnumerator FinishedFallingTouchdown()
    {
        yield return new WaitForSeconds(0.6f);
        anim.SetBool("Finished_Falling_Touchdown", true);
        canMove = true;
    }
    
    private IEnumerator Dash()
    {
        if (anim.GetBool("Finished_Falling_Touchdown"))
        {
            anim.SetTrigger("Dash");
            bgMusic.dashSound.Play();

            canDash = false;
            isDashing = true;
            float baseGravityScale = playerRigid.gravityScale;
            playerRigid.gravityScale = 0f;
            playerRigid.linearVelocity = 2.5f * speed * new Vector2(1 * transform.localScale.x * -5, 0);

            yield return new WaitForSeconds(dashTime / 100);
            playerRigid.gravityScale = baseGravityScale;
            isDashing = false;

            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }
    }
    

    public IEnumerator DisableMovementCor()
    {
        canMove = false;
        yield return new WaitForSeconds(0.3f);
        canMove = true;
        yield return new WaitForSeconds(0.05f);
        inputActions.Enable();
    }
}
