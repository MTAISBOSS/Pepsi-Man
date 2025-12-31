using DefaultNamespace;
using Script.Input;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float xBound = 2.5f;
    
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravityMultiplier = 2f;
    [SerializeField] private float jumpCooldown = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Collider Adjustment")]
    [SerializeField] private float jumpColliderHeight = 0.5f;
    [SerializeField] private float slideColliderHeight = 0.5f;
    [SerializeField] private Vector3 jumpColliderCenter = new Vector3(0, 0.25f, 0);
    [SerializeField] private Vector3 slideColliderCenter = new Vector3(0, 0.25f, 0);
    
    [Header("References")]
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private Animator animator;
    [SerializeField] private CapsuleCollider playerCollider;
    
    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private Transform groundCheckPoint;
    
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Slide = Animator.StringToHash("Slide");
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    private static readonly int JumpType = Animator.StringToHash("JumpType");
    
    private bool isGrounded;
    private bool isJumping;
    private bool isSliding;
    private float jumpTimer;
    private Vector3 originalColliderCenter;
    private float originalColliderHeight;
    private static readonly int Death = Animator.StringToHash("Death");
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        if (playerCollider == null)
            playerCollider = GetComponent<CapsuleCollider>();
            
        if (playerCollider != null)
        {
            originalColliderCenter = playerCollider.center;
            originalColliderHeight = playerCollider.height;
        }
    }
    
    private void Update()
    {
        CheckGroundStatus();
        UpdateAnimator();
        if (InputHandler.Instance.IsJumpButtonDown() && isGrounded && jumpTimer <= 0)
        {
            PerformJump();
        }
        if (InputHandler.Instance.IsSlideButtonDown() && isGrounded && !isJumping)
        {
            PerformSlide();
        }
        if (InputHandler.Instance.IsSlideButtonUp() && isSliding)
        {
            EndSlide();
        }
        
        if (jumpTimer > 0)
            jumpTimer -= Time.deltaTime;
    }
    
    private void FixedUpdate()
    {
        HandleHorizontalMovement();
        HandleGravity();
    }
    
    private void HandleHorizontalMovement()
    {
        float xInput = InputHandler.Instance.MoveInput.x;
        Vector3 movement = new Vector3(xInput * speed * Time.fixedDeltaTime, 0, 0);
        Vector3 newPosition = rigidbody.position + movement;
        newPosition.x = Mathf.Clamp(newPosition.x, -xBound, xBound);
        rigidbody.MovePosition(newPosition);
    }
    
    private void HandleGravity()
    {
        if (!isGrounded && !isJumping)
        {
            Vector3 gravity = Physics.gravity * gravityMultiplier;
            rigidbody.AddForce(gravity, ForceMode.Acceleration);
        }
    }
    
    private void CheckGroundStatus()
    {
        if (groundCheckPoint == null)
            return;
            
        RaycastHit hit;
        bool hitGround = Physics.Raycast(groundCheckPoint.position, Vector3.down, 
            out hit, groundCheckDistance, groundLayer);
            
        isGrounded = hitGround;
        
        if (isGrounded && isJumping && rigidbody.linearVelocity.y <= 0)
        {
            isJumping = false;
            
            if (!isSliding)
                ResetCollider();
        }
    }
    
    private void PerformJump()
    {
        isJumping = true;
        isGrounded = false;
        jumpTimer = jumpCooldown;
        
        rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, jumpForce, rigidbody.linearVelocity.z);
        
        int jumpType = Random.Range(0, 2);
        animator.SetTrigger(Jump);
        
        if (playerCollider != null)
        {
            playerCollider.height = jumpColliderHeight;
            playerCollider.center = jumpColliderCenter;
        }
    }
    
    private void PerformSlide()
    {
        isSliding = true;
        animator.SetTrigger(Slide);
        
        if (playerCollider != null)
        {
            playerCollider.height = slideColliderHeight;
            playerCollider.center = slideColliderCenter;
        }
    }
    
    private void EndSlide()
    {
        isSliding = false;
        animator.Play("Run");
        if (!isJumping)
            ResetCollider();
    }
    
    private void ResetCollider()
    {
        if (playerCollider != null)
        {
            playerCollider.height = originalColliderHeight;
            playerCollider.center = originalColliderCenter;
        }
    }
    
    private void UpdateAnimator()
    {
        animator.SetBool(Grounded, isGrounded && !_gameManager.isGameOver);
        if (isGrounded && !isJumping && !isSliding && !_gameManager.isGameOver)
        {
            animator.Play("Run");
        }
    }
    
    public void HandleGameOver()
    {
        animator.SetTrigger(Death);
    }
}