using System.Collections;
using UnityEngine;

public class SkeletonEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform wallDetector;
    [SerializeField] private Transform groundDetector;
    private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Detection")]
    [SerializeField] private float wallDistance = 0.1f;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 10f;
    [SerializeField] private GameObject skullCoin;

    [Header("Attack")]
    [SerializeField] private Transform playerDetector;
    [SerializeField] private float attackRange = 0.2f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1.5f;
    private bool isAttacking;
    private bool killedByPlayer;

    private bool canAttack = true;

    private Rigidbody2D rb;
    private Animator animator;

    private float timer;
    private int facingDirection;

    public enum SkeletonState
    {
        Spawning = 0,
        Walking = 1,
        Attacking = 2,
        Dying = 3
    }

    private SkeletonState currentState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        ChangeState(SkeletonState.Spawning);
        UpdateDirection();
    }

    private void Update()
    {
        if (currentState == SkeletonState.Dying)
            return;

        timer += Time.deltaTime;

        if (timer >= lifeTime)
        {
            Die();
            return;
        }

        if (currentState != SkeletonState.Walking)
            return;

        CheckAttack();
        CheckEnvironment();
    }

    private void FixedUpdate()
    {
        if (currentState != SkeletonState.Walking)
            return;

        rb.velocity = new Vector2(
            facingDirection * moveSpeed,
            rb.velocity.y
        );
    }

    private void UpdateDirection()
    {
        if (player == null)
            return;

        facingDirection = player.position.x > transform.position.x ? 1 : -1;

        Vector3 scale = transform.localScale;

        scale.x = Mathf.Abs(scale.x) * facingDirection;

        transform.localScale = scale;
    }
    private bool IsVulnerableToEnvironment()
    {
        return currentState == SkeletonState.Walking;
    }

    private void CheckEnvironment()
    {
        if (!IsVulnerableToEnvironment())
            return;
        Debug.DrawRay(
            wallDetector.position,
            Vector2.right * facingDirection * wallDistance,
            Color.red
        );

        Debug.DrawRay(
            groundDetector.position,
            Vector2.down * groundDistance,
            Color.green
        );
        bool wallAhead = Physics2D.Raycast(
            wallDetector.position,
            Vector2.right * facingDirection,
            wallDistance,
            groundLayer
        );

        if (wallAhead)
        {
            print("Wall ahead");
            Die();
            return;
        }

        bool groundAhead = Physics2D.Raycast(
            groundDetector.position,
            Vector2.down,
            groundDistance,
            groundLayer
        );

        if (!groundAhead)
        {
            print("No ground ahead");
            Die();
        }
    }

    private void CheckAttack()
    {
        if (!canAttack)
            return;

        Collider2D hit = Physics2D.OverlapCircle(
            playerDetector.position,
            attackRange
        );

        if (hit != null && hit.CompareTag("Player"))
        {
            if(player.gameObject.GetComponent<PlayerHealth>().isInvulnerable)
            {
                return;
            }
            StartAttack();
        }
    }
    private void StartAttack()
    {
        if (!canAttack)
            return;
        rb.velocity = Vector2.zero;
        StartCoroutine(AttackRoutine());
    }
    private IEnumerator AttackRoutine()
    {
        canAttack = false;

        ChangeState(SkeletonState.Attacking);

        animator.SetInteger("State", (int)SkeletonState.Attacking);

        yield return null; // deja arrancar animacion

        yield return new WaitForSeconds(attackCooldown);

        ChangeState(SkeletonState.Walking);

        canAttack = true;
    }
    public void AttackHit()
    {
        TryDamagePlayer();
    }
    private void TryDamagePlayer()
    {
        if (player == null)
            return;

        var playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        float horizontalDirection =
        player.position.x > transform.position.x ? 1f : -1f;

        Vector2 knockback = new Vector2(horizontalDirection * 6f, 8f);

        player.GetComponent<PlayerController>().TakeHit(knockback);

    }
    private void ChangeState(SkeletonState newState)
    {
        currentState = newState;

        animator.SetInteger(
            "State",
            (int)newState
        );
    }

    public void SpawnFinished()
    {
        ChangeState(SkeletonState.Walking);
        UpdateDirection();
    }

    public void TakeDamage()
    {
        if (currentState == SkeletonState.Dying || currentState == SkeletonState.Spawning)
            return;

        killedByPlayer = true;

        Die();
    }

    public void Die()
    {
        if (currentState == SkeletonState.Dying)
            return;
        ChangeState(SkeletonState.Dying);

        rb.velocity = Vector2.zero;
        rb.simulated = false;
    }

    public void DestroyEnemy()
    {
        if (killedByPlayer)
        {
            int roll = Random.Range(0, 3);

            if (roll == 0)
            {
                Instantiate(
                    skullCoin,
                    transform.position,
                    Quaternion.identity
                );
            }
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TryDamagePlayer();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (wallDetector != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(
                wallDetector.position,
                wallDetector.position +
                Vector3.right * wallDistance
            );
        }

        if (groundDetector != null)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawLine(
                groundDetector.position,
                groundDetector.position +
                Vector3.down * groundDistance
            );
        }
        if (playerDetector == null) return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(playerDetector.position, attackRange);
    }
}
