using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.6f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int attackDamage = 1;

    [Header("Ground Check (BoxCast)")]
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float groundDistance = 0.1f;
    [SerializeField] private Vector2 groundBoxSizeOffset = new Vector2(0.1f, 0.0f);
    [SerializeField] private Vector2 groundBoxCenterOffset = new Vector2(0f, 0f);

    private BoxCollider2D coll;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 originalScale;

    private float moveInput;
    private bool isGrounded;
    private bool isAttacking;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
        coll = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        HandleInput();
        HandleJump();
        HandleAttack();
        UpdateAnimations();
        FlipCharacter();
        print(isGrounded);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void HandleInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
    }

    private void Move()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    private void HandleJump()
    {
        isGrounded = IsGrounded();

        if ((Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");

            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRadius,
                enemyLayer
            );

            foreach (Collider2D enemy in enemiesHit)
            {
                Debug.Log("Enemy hit: " + enemy.name);

                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage);
                }
            }
        }
    }

    private void FlipCharacter()
    {
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
    }

    private void UpdateAnimations()
    {
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("Grounded", isGrounded);
        animator.SetFloat("VerticalVelocity", rb.velocity.y);
    }
    private bool IsGrounded()
    {
        Bounds bounds = coll.bounds;

        Vector2 boxSize = new Vector2(
            bounds.size.x - groundBoxSizeOffset.x,
            bounds.size.y
        );

        Vector2 boxCenter = new Vector2(
            bounds.center.x + groundBoxCenterOffset.x,
            bounds.center.y + groundBoxCenterOffset.y
        );

        RaycastHit2D hit = Physics2D.BoxCast(
            boxCenter,
            boxSize,
            0f,
            Vector2.down,
            groundDistance,
            groundLayer
        );

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        if (coll == null) return;

        Gizmos.color = Color.green;

        Bounds bounds = coll.bounds;

        Vector2 boxSize = new Vector2(
            bounds.size.x - groundBoxSizeOffset.x,
            bounds.size.y
        );

        Vector2 boxCenter = new Vector2(
            bounds.center.x + groundBoxCenterOffset.x,
            bounds.center.y + groundBoxCenterOffset.y
        );

        Gizmos.DrawWireCube(
            boxCenter + Vector2.down * groundDistance,
            boxSize
        );
    }
}
