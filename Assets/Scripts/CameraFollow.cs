using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float deadZoneWidth = 3f;
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private PlayerController player;
    [SerializeField] private float verticalThreshold = 1f;
    [SerializeField] private float fallFollowDelay = 0.3f;

    private float fallTimer;

    private float targetY;

    private bool followEnabled = true;

    private void Start()
    {
        targetY = target.position.y;
    }

    private void LateUpdate()
    {
        if (target == null || !followEnabled)
            return;

        Vector3 desiredPosition = transform.position;
        bool followY = player.currentState != PlayerController.PlayerState.Jumping;

        //----------------------------------
        // HORIZONTAL (dead zone)
        //----------------------------------

        float deltaX = target.position.x - transform.position.x;

        if (Mathf.Abs(deltaX) > deadZoneWidth * 0.5f)
        {
            float excess =
                Mathf.Abs(deltaX) - deadZoneWidth * 0.5f;

            desiredPosition.x +=
                Mathf.Sign(deltaX) * excess;
        }

        //----------------------------------
        // VERTICAL
        //----------------------------------

        bool isFalling = player.VelocityY < -0.1f;
        bool isGrounded = player.IsGrounded;

        if (isGrounded)
        {
            fallTimer = 0f;
            targetY = target.position.y;
        }
        else if (isFalling)
        {
            fallTimer += Time.deltaTime;

            if (fallTimer >= fallFollowDelay)
            {
                targetY = target.position.y;
            }
        }
        else
        {
            // si está subiendo en salto -> no seguimos
            fallTimer = 0f;
        }

        desiredPosition.y = targetY;

        //----------------------------------
        // MOVE CAMERA
        //----------------------------------

        // límite izquierdo del mapa
        desiredPosition.x = Mathf.Max(0f, desiredPosition.x);

        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(
                desiredPosition.x,
                desiredPosition.y,
                transform.position.z
            ),
            smoothSpeed * Time.deltaTime
        );
    }
    public void StopFollowing(float duration)
    {
        StartCoroutine(StopFollowingRoutine(duration));
    }

    private System.Collections.IEnumerator StopFollowingRoutine(float duration)
    {
        followEnabled = false;

        yield return new WaitForSeconds(duration);

        followEnabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(deadZoneWidth, 5f, 0)
        );
    }
}