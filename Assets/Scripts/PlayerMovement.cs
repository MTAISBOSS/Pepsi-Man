using System.Collections;
using AudioManagement;
using DefaultNamespace;
using Script.Input;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")] [SerializeField]
    private float speed = 5f;

    [SerializeField] private float xBound = 2.5f;

    [Header("Jump Settings")] [SerializeField]
    private float jumpForce = 10f;

    [SerializeField] private float gravityMultiplier = 2f;
    [SerializeField] private float jumpCooldown = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Collider Adjustment")] [SerializeField]
    private float jumpColliderHeight = 0.5f;

    [SerializeField] private float slideColliderHeight = 0.5f;
    [SerializeField] private Vector3 jumpColliderCenter = new Vector3(0, 0.25f, 0);
    [SerializeField] private Vector3 slideColliderCenter = new Vector3(0, 0.25f, 0);

    [Header("References")] [SerializeField]
    private Rigidbody rigidbody;

    [SerializeField] private Animator animator;
    [SerializeField] private CapsuleCollider playerCollider;

    [Header("Ground Check")] [SerializeField]
    private float groundCheckDistance = 0.1f;

    [SerializeField] private Transform groundCheckPoint;

    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Slide = Animator.StringToHash("Slide");
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    private static readonly int SpecialJump = Animator.StringToHash("SpecialJump");

    private bool _isGrounded;
    private bool _isJumping;
    private bool _isSliding;
    private float _jumpTimer;
    private Vector3 _originalColliderCenter;
    private float _originalColliderHeight;
    private static readonly int Death = Animator.StringToHash("Death");
    private GameManager _gameManager;
    private RaycastHit[] _results;
    [SerializeField] private LayerMask specialObstacleLayer;
    [SerializeField] private float specialObstacleDistance;

    private void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        if (playerCollider == null)
            playerCollider = GetComponent<CapsuleCollider>();

        if (playerCollider != null)
        {
            _originalColliderCenter = playerCollider.center;
            _originalColliderHeight = playerCollider.height;
        }
    }

    private void Update()
    {
        CheckGroundStatus();
        UpdateAnimator();
        if (InputHandler.Instance.IsJumpButtonDown() && _isGrounded && _jumpTimer <= 0)
        {
            PerformJump();
        }

        if (InputHandler.Instance.IsSlideButtonDown() && _isGrounded && !_isJumping)
        {
            PerformSlide();
        }

        if (_jumpTimer > 0)
            _jumpTimer -= Time.deltaTime;
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
        if (!_isGrounded && !_isJumping)
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

        _isGrounded = hitGround;

        if (_isGrounded && _isJumping && rigidbody.linearVelocity.y <= 0)
        {
            _isJumping = false;

            if (!_isSliding)
                ResetCollider();
        }
    }

    private void PerformJump()
    {
        _isJumping = true;
        _isGrounded = false;
        _jumpTimer = jumpCooldown;
        AudioManager.Instance.Play(Sound.Jump.ToString());
        rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, jumpForce, rigidbody.linearVelocity.z);
        _results = new RaycastHit[1];

        var forwardRay = Physics.RaycastNonAlloc(transform.position, transform.forward, _results,
            specialObstacleDistance, specialObstacleLayer);
        animator.SetTrigger(forwardRay > 0 ? SpecialJump : Jump);
        playerCollider.height = forwardRay > 0 ? 3f * jumpColliderHeight : jumpColliderHeight;
        playerCollider.center = forwardRay > 0 ? 3f * jumpColliderCenter : jumpColliderCenter;
    }

    private void PerformSlide()
    {
        _isSliding = true;
        AudioManager.Instance.Play(Sound.Slide.ToString());

        animator.SetTrigger(Slide);
        StartCoroutine(SlideCoroutine());
        if (playerCollider != null)
        {
            playerCollider.height = slideColliderHeight;
            playerCollider.center = slideColliderCenter;
        }
    }

    private IEnumerator SlideCoroutine()
    {
        // Wait for the slide animation to complete (90% of its duration)
        yield return new WaitUntil(() => 
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f &&
            animator.GetCurrentAnimatorStateInfo(0).IsName("Slide"));
        
        _isSliding = false;
        
        // Reset collider if not jumping
        if (!_isJumping)
            ResetCollider();
    }

    private void ResetCollider()
    {
        if (playerCollider != null)
        {
            playerCollider.height = _originalColliderHeight;
            playerCollider.center = _originalColliderCenter;
        }
    }

    private void UpdateAnimator()
    {
        animator.SetBool(Grounded, _isGrounded && !_gameManager.isGameOver);
        if (_isGrounded && !_isJumping && !_isSliding && !_gameManager.isGameOver)
        {
            animator.Play("Run");
        }
    }

    public void HandleGameOver()
    {
        animator.SetTrigger(Death);
    }
}