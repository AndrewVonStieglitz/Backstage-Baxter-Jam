using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private Rigidbody2D baxterRigidBody;
    private SpriteRenderer baxterSpriteRenderer;
    private CircleCollider2D baxterCollider;


    [SerializeField] private float jumpForce;
    private float jumpBuffer = -1;
    [SerializeField] private float jumpHoldMaxTime;
    private bool isGrounded;
    [SerializeField] private float coyoteTime;
    private float coyoteTimer = -1;
    private float distToGround;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float movementAxisGravity;
    [SerializeField] private float movementAxisSensitivity;
    private float moveAxis;
    [SerializeField] private float quadraticThreshold = 0.2f;
    [SerializeField] private float linearDrag = 0.05f;
    [SerializeField] private float quadDrag = 0.05f;

    PlayerControls playerControls;

    private float halfHeight;
    private int groundedLayerMask;
    private int platformLayer = 6;

    private Animator animator;

    public bool debugIsGrouned = false; // shows the state of this variable in inspector. 

    //private PlayerInput baxterInput;

    // Start is called before the first frame update
    private void Awake()
    {
        baxterRigidBody = GetComponent<Rigidbody2D>();
        baxterSpriteRenderer = GetComponent<SpriteRenderer>();
        baxterCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        //baxterInput = GetComponent<PlayerInput>();

        playerControls = new PlayerControls();

        distToGround = baxterCollider.radius*1.05f;
        print("Distance to ground: " + distToGround);

        playerControls.Baxter.Enable();
        playerControls.Baxter.Jump.performed += StartJump;
        playerControls.Baxter.Jump.canceled += EndJump;
        playerControls.Baxter.Move.performed += PlayerMove;
        
        groundedLayerMask = (1 << platformLayer);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        setMoveAxis();

        if (coyoteTimer > 0) coyoteTimer -= Time.deltaTime;
        animator.SetFloat("yVelo", baxterRigidBody.velocity.y);
    }

    private void FixedUpdate()
    {
        float xVelo = baxterRigidBody.velocity.x;
        float drag = (xVelo < quadraticThreshold) ? linearDrag * xVelo : (xVelo * xVelo * Mathf.Sign(xVelo)) * quadDrag;
        baxterRigidBody.AddForce(Vector2.right * drag);
        GetIsGroundedRayCast();
        if (jumpBuffer > 0)
        {
            jumpBuffer -= Time.fixedDeltaTime;
            baxterRigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            coyoteTimer = -1;
        }

        //baxterRigidBody.AddForce(Vector2.right * moveSpeed * moveAxis, ForceMode2D.Impulse);
        baxterRigidBody.velocity = new Vector2( moveSpeed * moveAxis, baxterRigidBody.velocity.y);
        debugIsGrouned = isGrounded;
        animator.SetFloat("xVelo", Mathf.Abs(baxterRigidBody.velocity.x));
        //animator.ResetTrigger("Landed");
    }

    public void StartJump(InputAction.CallbackContext context)
    {
        if (isGrounded || (coyoteTimer > 0f && jumpBuffer > 0f))
        {
            print("Jump executing");
            jumpBuffer = jumpHoldMaxTime;
            baxterRigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            coyoteTimer = -1;
            jumpBuffer = 0f;
            animator.ResetTrigger("Landed");
            animator.SetTrigger("JumpCommand");
        }
        
    }

    public void EndJump(InputAction.CallbackContext context)
    {
        jumpBuffer = -1;
    }

    public void PlayerMove(InputAction.CallbackContext context)
    {
        if (playerControls.Baxter.Move.ReadValue<float>() > 0)
        {
            baxterSpriteRenderer.flipX = false;
        }
        else if (playerControls.Baxter.Move.ReadValue<float>() < 0)
        {
            baxterSpriteRenderer.flipX = true;
        }
    }


    private void setMoveAxis()
    {
        float inputMoveAxis = playerControls.Baxter.Move.ReadValue<float>();

        if (inputMoveAxis != 0)
        {
            moveAxis = Mathf.MoveTowards(moveAxis, inputMoveAxis, movementAxisSensitivity / 10 * Time.deltaTime);
        }

        if ((Mathf.Sign(inputMoveAxis) != Mathf.Sign(moveAxis) && inputMoveAxis != 0) || inputMoveAxis == 0)
        {
            moveAxis = Mathf.MoveTowards(moveAxis, 0, movementAxisGravity / 10 * Time.deltaTime);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //CheckIsGrounded(collision);    
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //CheckIsGrounded(collision);
    }

    private void CheckIsGrounded(Collision2D collision) // doesn't work 
    {
        ContactPoint2D[] contactPoints = new ContactPoint2D[10];
        collision.GetContacts(contactPoints);
        
        isGrounded = false;
        coyoteTimer = coyoteTime;

        foreach (ContactPoint2D contact in contactPoints)
        {
            if (contact.point.y < transform.position.y - distToGround)
                isGrounded = true;
        }
        animator.SetBool("isGrounded", isGrounded); 
    }

    private bool GetIsGroundedRayCast()
    {
        bool oldIsGrounded = isGrounded;
        isGrounded = false;
        Vector2 rayOrigin = transform.position; // upgrade to array of raycasts soon
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 50f, groundedLayerMask);
        if (hit.collider != null)
        {
            if (hit.distance < distToGround)
                isGrounded = true;
            //print("raycast distance: " + hit.distance + "\t, dist to ground: " + distToGround + "\t, bool: " + (hit.distance < distToGround));
        }
        animator.SetBool("isGrounded", isGrounded);
        if (isGrounded && !oldIsGrounded) { animator.SetTrigger("Landed"); print("Landed at: " + Time.time); }
        return isGrounded;
    }
}
