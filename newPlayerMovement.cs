using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.InputSystem;

public class newPlayerMovement : MonoBehaviour
{

    public PlayerInput playerInput;
    public Vector2 moveInput;
    public Collider2D assignedGoal;

    [Header("References")]
    public PlayerMovementStats MoveStats;
    [SerializeField] private Collider2D _feetCoil;
    [SerializeField] private Collider2D _bodyCoil;
    [SerializeField] public InputManager InputManager;
    private Camera _mainCamera;
    bool isOwner;

    private Rigidbody2D _rb;

    //movement vars
    private Vector2 _moveVelocity;
    private bool _isFacingRight;

    private RaycastHit2D _groundHit;
    public bool _isGrounded;

    public float VerticalVelocity { get; private set; }

    private float _jumpBufferTimer;

    // animation
    private Animator anim;


    private void Awake()
    {
        _isFacingRight = true; 
        _rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void FixedUpdate()
    {
        playerInput = GetComponent<PlayerInput>();
        Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>(); // Read input from the assigned device.
        AnimationChecks(moveInput);

        if (_isGrounded)
        {
            Move(MoveStats.GroundAcceleration, MoveStats.GroundDeceleration, moveInput);
        }
        else
        {
            Move(MoveStats.AirAcceleration, MoveStats.AirDeceleration, moveInput);
        }

    }



    // Update is called once per frame
    void Update()
    {
        CollisionChecks();
        Jump(moveInput);

    }



    #region Movement
    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {

            TurnCheck(moveInput);

            Vector2 targetVelocity = new Vector2(moveInput.x, 0f) * MoveStats.MaxWalkSpeed;
            _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocity.y);
        }

        else if (moveInput == Vector2.zero)
        {
            _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocity.y);
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {

        if (_isFacingRight && moveInput.x < 0)
        {
            Turn(false);

        }

        else if (!_isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }

    }


    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            _isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }

        else
        {
            _isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }

    #endregion

    #region Collision Checks

    private void isGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetCoil.bounds.center.x, _feetCoil.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetCoil.bounds.size.x, MoveStats.GroundDetectionRaylength);

        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MoveStats.GroundDetectionRaylength, MoveStats.groundLayerMask);
        if (_groundHit.collider != null)
        {
            _isGrounded = true;
        }
        else { _isGrounded = false; }

        #region Debug Visualization

       
            Color rayColor;
            if (_isGrounded)
            {
                rayColor = Color.green;
            }
            else { rayColor = Color.red; }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MoveStats.GroundDetectionRaylength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MoveStats.GroundDetectionRaylength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - MoveStats.GroundDetectionRaylength), Vector2.right * boxCastSize.x, rayColor);

        
        #endregion
    }

    private void CollisionChecks()
        {
        isGrounded();
        }

    #endregion

    #region Jump


    private void Jump(Vector2 moveInput)
    {
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        if ((playerInput.actions["Jump"].WasPressedThisFrame()))
        {
            print("jump " + moveInput);
            if (_isGrounded)
            {
                GroundedJump(moveInput);
            }

            else
            {
                anim.SetBool("airJumped", true);
                AirJump(moveInput);
            }

           
        }
    }
    private void GroundedJump(Vector2 moveInput)
    {
        float jumpDirection = moveInput.x;
        _rb.AddForce(new Vector2(0, MoveStats.jumpUpForce).normalized, ForceMode2D.Impulse);
        // animation
        anim.SetTrigger("jumped");
    }

   private void AirJump(Vector2 moveInput)
    {
        _rb.linearVelocity = new Vector2(0f, 0f);
        float jumpdirectionX = moveInput.x;
        float jumpdirectionY = moveInput.y;
        Vector2 jumpDir = new Vector2(jumpdirectionX, jumpdirectionY);

        print("AirJump Direction: " + jumpDir);
        _rb.AddForce(jumpDir * MoveStats.jumpUpForce, ForceMode2D.Impulse);



    }


    #endregion

    #region Animation

    private void AnimationChecks(Vector2 moveInput)
    {

        if (_isGrounded)
        {
            anim.SetBool("airJumped", false);
            anim.SetBool("isGrounded", true);
        }
        else
        {
            anim.SetBool("isGrounded", false);
        }

        if (moveInput.x != 0)
        {
            anim.SetBool("isMoving", true);
            
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }

    #endregion

}
