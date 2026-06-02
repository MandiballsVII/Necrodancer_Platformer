using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float maxFallSpeed = 25f;

    [Header("Ground Check (BoxCast)")]
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float groundDistance = 0.1f;
    [SerializeField] private Vector2 groundBoxSizeOffset = new Vector2(0.1f, 0.0f);
    [SerializeField] private Vector2 groundBoxCenterOffset = new Vector2(0f, 0f);

    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.6f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int attackDamage = 1;
    private bool useAttack1 = true;
    private bool isAttacking;

    [Header("Hit & Death")]
    [SerializeField] private BoxCollider2D hitbox;
    [SerializeField] private BoxCollider2D deathCollider;
    PlayerHealth playerHealth;

    private bool isHit;
    private bool isDead;
    public int FacingDirection { get; private set; } = 1;

    private bool jumpLocked;
    private float jumpDirection;
    private bool wasGrounded;
    public float VelocityY => rb.velocity.y;

    private BoxCollider2D coll;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 originalScale;

    private float moveInput;
    private bool isGrounded;
    public bool IsGrounded => isGrounded;

    public enum PlayerState
    {
        Idle,
        Running,
        Jumping,
        Falling
    }

    public PlayerState currentState = PlayerState.Idle;
    private PlayerState airStateLock;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
        coll = GetComponent<BoxCollider2D>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        HandleInput();
        HandleJump();
        HandleAttack();
        //UpdateAnimations();
        FlipCharacter();
        isGrounded = CheckGrounded();

        if (!wasGrounded && isGrounded)
        {
            jumpLocked = false;
        }

        wasGrounded = isGrounded;
        if (!isHit)
            ChangePlayerState();
    }

    private void FixedUpdate()
    {
        Move();
        ClampFallSpeed();
    }

    private void HandleInput()
    {
        if (isHit || isDead) return;
        moveInput = Input.GetAxisRaw("Horizontal");
    }

    private void Move()
    {
        if (isHit || isDead)
            return;
        if (isAttacking && isGrounded)
        {
            rb.velocity = new Vector2(
                0f,
                rb.velocity.y
            );

            return;
        }

        if (isGrounded)
        {
            rb.velocity = new Vector2(
                moveInput * moveSpeed,
                rb.velocity.y
            );
        }
        else if (jumpLocked)
        {
            rb.velocity = new Vector2(
                jumpDirection * moveSpeed,
                rb.velocity.y
            );
        }
    }
    private void ClampFallSpeed()
    {
        if (rb.velocity.y < -maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);
        }
    }

    private void HandleJump()
    {
        if (isHit || isDead) return;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            jumpLocked = true;
            jumpDirection = moveInput;
        }
    }

    private void HandleAttack()
    {
        if (isAttacking || isHit || isDead)
            return;

        if (Input.GetKeyDown(KeyCode.P) || Input.GetMouseButtonDown(0))
        {
            StartAttack();
        }
    }
    private void StartAttack()
    {
        isAttacking = true;
        moveInput = 0;
        if (useAttack1)
            animator.SetTrigger("Attack");
        else
            animator.SetTrigger("Attack2");

        useAttack1 = !useAttack1;
    }
    public void DealDamage()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            enemyLayer
        );

        foreach (Collider2D enemy in enemiesHit)
        {
            SkeletonEnemy skeleton = enemy.GetComponent<SkeletonEnemy>();

            if (skeleton != null)
            {
                skeleton.TakeDamage();
            }
        }
    }
    public void EndAttack()
    {
        isAttacking = false;
    }

    public void TakeHit(Vector2 knockback)
    {
        if (isHit || isDead) return;
        isHit = true;
        isAttacking = false;
        if (playerHealth.health > 0)
            animator.SetBool("Hit", true);
        else
            animator.SetTrigger("Die");
        StartCoroutine(HitRoutine(knockback));
    }
    private IEnumerator HitRoutine(Vector2 knockback)
    {

        rb.velocity = Vector2.zero;

        rb.AddForce(knockback, ForceMode2D.Impulse);

        // espera a que acabe animacion (o tiempo fijo simple)
        yield return new WaitForSeconds(0.6f);
        animator.SetBool("Hit", false);
        isHit = false;
    }
    public void Die()
    {
        if (isDead) return;

        isDead = true;
        isHit = true;
        isAttacking = false;

        rb.velocity = Vector2.zero;

        animator.SetTrigger("Die");
    }
    public void ChangeColliders()
    {
        hitbox.enabled = false;
        deathCollider.enabled = true;
    }
    public void OnDeathAnimationEnd()
    {
        StartCoroutine(LevelRestartWaiter());
    }
    private IEnumerator LevelRestartWaiter()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Instance.RestartLevel();
    }

    private void FlipCharacter()
    {
        if (isAttacking || isHit || isDead)
            return;
        if (moveInput > 0)
        {
            FacingDirection = 1;
            transform.localScale = new Vector3(
                Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
        else if (moveInput < 0)
        {
            FacingDirection = -1;
            transform.localScale = new Vector3(
                -Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
    }

    private bool CheckGrounded()
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

    private void ChangePlayerState()
    {
        if (isHit || isDead) return;
        if (isGrounded)
        {
            airStateLock = PlayerState.Idle;

            if (rb.velocity.x != 0)
                currentState = PlayerState.Running;
            else
                currentState = PlayerState.Idle;
        }
        else
        {
            if (rb.velocity.y > 0)
                airStateLock = PlayerState.Jumping;
            else if (rb.velocity.y < 0)
                airStateLock = PlayerState.Falling;

            currentState = airStateLock;
        }
        ChangePlayerAnimation();
    }

    private void ChangePlayerAnimation()
    {
        if (isHit || isDead)
        {
            return;
        }
        switch (currentState)
        {
            case PlayerState.Idle:
                animator.SetInteger("State", (int)PlayerState.Idle);
                break;
            case PlayerState.Running:
                animator.SetInteger("State", (int)PlayerState.Running);
                break;
            case PlayerState.Jumping:
                animator.SetInteger("State", (int)PlayerState.Jumping);
                break;
            case PlayerState.Falling:
                animator.SetInteger("State", (int)PlayerState.Falling);
                break;
        }
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
