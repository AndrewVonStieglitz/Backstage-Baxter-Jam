using System.Collections;
using DefaultNamespace.Pluggables;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private Rigidbody2D baxterRigidBody;
    private SpriteRenderer baxterSpriteRenderer;
    private CircleCollider2D baxterCollider;
    // TODO: Remove this dependency.
    private Cables.CableHead cableHead;
    private ConnectionHead connectionHead;

    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpBuffer;
    private float jumpBufferCounter = -1f;
    [SerializeField] private float jumpHoldMaxTime;
    private bool isGrounded;
    [SerializeField] private float coyoteTime;
    private float coyoteTimeCounter = -1f;
    private float distToGround;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float movementAxisGravity;
    [SerializeField] private float movementAxisSensitivity;
    private float moveAxis;
    [SerializeField] private float quadraticThreshold = 0.2f;
    [SerializeField] private float linearDrag = 0.05f;
    [SerializeField] private float quadDrag = 0.05f;

    PlayerControls playerControls;

    [SerializeField] private int raycastResolution;
    private Vector2 rayIncrementor, raycastOrigin;
    private float halfHeight;
    private float colliderWidth;
    private int groundedLayerMask;
    private int platformLayer = 6;

    private Animator animator;

    private float pickupHeldFrom = 0f;
    private bool pickupBeingHeld = false;
    [SerializeField] private float maxHoldPickup;

    public bool debugIsGrouned = false; // shows the state of this variable in inspector. 

    //private PlayerInput baxterInput;
    private IEnumerator pauseControlsRoutine;

    ////the collider
    //CircleCollider2D circleCollider;
    //Vector2 defaultCollisionOffset;

    // Start is called before the first frame update
    private void Awake()
    {
        //circleCollider = GetComponent<CircleCollider2D>();
        //defaultCollisionOffset = circleCollider.offset;

        baxterRigidBody = GetComponent<Rigidbody2D>();
        baxterSpriteRenderer = GetComponent<SpriteRenderer>();
        //baxterCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        //baxterInput = GetComponent<PlayerInput>();
        // TODO: Use SerialisedFields.
        cableHead = transform.GetChild(0).GetComponent<Cables.CableHead>();// requires cable head be first child!!
        connectionHead = transform.GetChild(0).GetComponent<ConnectionHead>();// requires cable head be first child!!

        playerControls = new PlayerControls();

        //distToGround = baxterCollider.radius*1.05f;
        // print("Distance to ground: " + distToGround);

        // TODO: Unlisten
        playerControls.Baxter.Enable();
        playerControls.Baxter.Jump.performed += StartJump;
        playerControls.Baxter.Jump.canceled += EndJump;
        playerControls.Baxter.Move.performed += PlayerMove;
        playerControls.Baxter.PickupRelease.performed += PickupPressDown;
        playerControls.Baxter.PickupRelease.canceled += PickupPressUp;
        
        groundedLayerMask = (1 << platformLayer);
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        colliderWidth = box.size.x;
        distToGround = box.size.y * 1.05f;
        rayIncrementor = new Vector2(colliderWidth / raycastResolution, 0f);
        raycastOrigin = new Vector2(rayIncrementor.x * -(raycastResolution)/2, 0f);
    }

    private void OnEnable()
    {
        GameEvents.onPlayerCableCollision += OnPlayerCableCollision;
    }

    private void OnDisable()
    {
        GameEvents.onPlayerCableCollision -= OnPlayerCableCollision;
    }

    private void OnPlayerCableCollision(Vector2 position, Vector2 normal)
    {
       SetFryState(); 
    }

    void Update()
    {
        if (playerControls.Baxter.enabled)
            setMoveAxis();

        if (coyoteTimeCounter > 0) coyoteTimeCounter -= Time.deltaTime;
        animator.SetFloat("yVelo", baxterRigidBody.velocity.y);
    }

    private void FixedUpdate()
    {
        float xVelo = baxterRigidBody.velocity.x;
        float drag = (Mathf.Abs(xVelo) < quadraticThreshold) ? linearDrag * xVelo : (xVelo * xVelo * Mathf.Sign(xVelo)) * quadDrag;
        //print("Drag: " + -drag);
        baxterRigidBody.AddForce(Vector2.right * -drag);
        jumpBufferCounter -= Time.fixedDeltaTime;
        if (playerControls.Baxter.enabled)
        {
            GetIsGroundedRayCast();
            if (isGrounded)
                coyoteTimeCounter = coyoteTime;
            else
                coyoteTimeCounter -= Time.fixedDeltaTime;
            // Jump has been input and is legal
            if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
            {
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
                baxterRigidBody.velocity = new Vector2(baxterRigidBody.velocity.x, 0f);
                baxterRigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                animator.ResetTrigger("Landed");
                animator.SetTrigger("JumpCommand");
            }

            //baxterRigidBody.AddForce(Vector2.right * moveSpeed * moveAxis, ForceMode2D.Impulse);
            baxterRigidBody.velocity = new Vector2(moveSpeed * moveAxis, baxterRigidBody.velocity.y);
            debugIsGrouned = isGrounded;
            animator.SetFloat("xVelo", Mathf.Abs(baxterRigidBody.velocity.x));
            //animator.ResetTrigger("Landed");
        }
    }

    public void StartJump(InputAction.CallbackContext context)
    {
        jumpBufferCounter = jumpBuffer;
        //if (isGrounded || (coyoteTimeCounter > 0f && jumpBuffer > 0f))
        //{
        //    // print("Jump executing");
        //    jumpBuffer = jumpHoldMaxTime;
        //    baxterRigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        //    coyoteTimeCounter = -1;
        //    jumpBuffer = 0f;
        //    animator.ResetTrigger("Landed");
        //    animator.SetTrigger("JumpCommand");
        //}
        
    }

    public void EndJump(InputAction.CallbackContext context)
    {
        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;
    }

    public void PickupPressDown(InputAction.CallbackContext context)
    {
        pickupHeldFrom = Time.time;
        pickupBeingHeld = true;
        StopCoroutine(countdownPickupHold());
        StartCoroutine(countdownPickupHold());
    }

    public void PickupPressUp(InputAction.CallbackContext context)
    {
        float pickupHeldTime= Time.time - pickupHeldFrom;
        pickupBeingHeld = false;
        StopCoroutine(countdownPickupHold());
        if (pickupHeldTime < maxHoldPickup)
        {
            // pick up the cable
            connectionHead.TryInteract();
        }
        else
        {
            // release the cable
            cableHead.DropCable();
        }
    }

    private IEnumerator countdownPickupHold()
    {
        float startTime = Time.time;
        yield return new WaitForSeconds(maxHoldPickup);
        if (pickupBeingHeld && Time.time - pickupHeldFrom >= maxHoldPickup)
        {
            // release cable
            //print("Coroutine force releasing cable\tat: " + Time.time + ",\t started at: " + startTime);
            cableHead.DropCable();
            pickupBeingHeld = false;
        }
    }

    public void PlayerMove(InputAction.CallbackContext context)
    {
        if (playerControls.Baxter.Move.ReadValue<float>() > 0)
        {
            baxterSpriteRenderer.flipX = false;
            //circleCollider.offset = defaultCollisionOffset;
        }
        else if (playerControls.Baxter.Move.ReadValue<float>() < 0)
        {
            baxterSpriteRenderer.flipX = true;
            //circleCollider.offset = new Vector2(defaultCollisionOffset.x * -1, defaultCollisionOffset.y);
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

    private bool GetIsGroundedRayCast()
    {
        bool oldIsGrounded = isGrounded;
        isGrounded = false;
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        for (int ray = 0; ray <= raycastResolution; ray++)
        {
            Vector2 rayStart = position + raycastOrigin + (rayIncrementor * ray);
            RaycastHit2D hit = Physics2D.Raycast(rayStart, -Vector2.up, 50f, groundedLayerMask);
            Debug.DrawRay(rayStart, -Vector2.up);
            if (hit.collider != null)
            {
                if (hit.distance < distToGround)
                {
                    isGrounded = true;
                    break;
                }
                //print("raycast distance: " + hit.distance + "\t, dist to ground: " + distToGround + "\t, bool: " + (hit.distance < distToGround));
            }
        }
        animator.SetBool("isGrounded", isGrounded);
        if (isGrounded && !oldIsGrounded)
        {
            animator.SetTrigger("Landed");
        }
        return isGrounded;
    }

    /// <summary>
    /// Call whever the player enters the frying state
    /// </summary>
    public void SetFryState()
    {
        animator.SetTrigger("Frying");
        if (pauseControlsRoutine != null)//if the coroutine is currently running stop it to avoid multiple running at once
            StopCoroutine(pauseControlsRoutine);
        pauseControlsRoutine = TempDisablePlayerControls(0.71f);
        StartCoroutine(pauseControlsRoutine);
    }

    /// <summary>
    /// temporarily disable the player controls
    /// </summary>
    /// <param name="waitTime">time to wait before the player controls are enabled</param>
    IEnumerator TempDisablePlayerControls(float waitTime)
    {
        baxterRigidBody.velocity = Vector2.zero;
        baxterRigidBody.angularVelocity = 0;
        baxterRigidBody.isKinematic = true;
        moveAxis = 0;
        animator.SetFloat("xVelo", 0);
        playerControls.Baxter.Disable();
        //Debug.Log("Stop input from running" + playerControls.Baxter.enabled);
        yield return new WaitForSeconds(waitTime);
        baxterRigidBody.isKinematic = false;
        playerControls.Baxter.Enable();
        //Debug.Log("Re-enable input from running" + playerControls.Baxter.enabled);

    }
}
