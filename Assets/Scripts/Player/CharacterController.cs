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
    private float jumpHoldTimer = -1;
    [SerializeField] private float jumpHoldMaxTime;
    private bool isGrounded;
    [SerializeField] private float coyoteTime;
    private float coyoteTimer = -1;
    private float distToGround;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float movementAxisGravity;
    [SerializeField] private float movementAxisSensitivity;
    private float moveAxis;

    PlayerControls playerControls;

    //private PlayerInput baxterInput;

    // Start is called before the first frame update
    private void Awake()
    {
        baxterRigidBody = GetComponent<Rigidbody2D>();
        baxterSpriteRenderer = GetComponent<SpriteRenderer>();
        baxterCollider = GetComponent<CircleCollider2D>();
        //baxterInput = GetComponent<PlayerInput>();

        playerControls = new PlayerControls();

        distToGround = baxterCollider.radius - baxterCollider.radius*0.05f;

        playerControls.Baxter.Enable();
        playerControls.Baxter.Jump.performed += StartJump;
        playerControls.Baxter.Jump.canceled += EndJump;
        playerControls.Baxter.Move.performed += PlayerMove;

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        setMoveAxis();

        if (coyoteTimer > 0) coyoteTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (jumpHoldTimer > 0)
        {
            jumpHoldTimer -= Time.fixedDeltaTime;
            baxterRigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            coyoteTimer = -1;
        }

        //baxterRigidBody.AddForce(Vector2.right * moveSpeed * moveAxis, ForceMode2D.Impulse);
        baxterRigidBody.velocity = new Vector2( moveSpeed * moveAxis, baxterRigidBody.velocity.y);
    }

    public void StartJump(InputAction.CallbackContext context)
    {
        if (isGrounded || coyoteTimer > 0)
        {
            jumpHoldTimer = jumpHoldMaxTime;
            baxterRigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            coyoteTimer = -1;
        }
    }

    public void EndJump(InputAction.CallbackContext context)
    {
        jumpHoldTimer = -1;
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
        CheckIsGrounded(collision);    
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        CheckIsGrounded(collision);
    }

    private void CheckIsGrounded(Collision2D collision)
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
    }
}
