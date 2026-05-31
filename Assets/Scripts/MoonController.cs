using System.Collections;
using UnityEngine;

public class MoonController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float minTime = 10f;
    [SerializeField] private float maxTime = 20f;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(RandomAnimationRoutine());
    }

    private IEnumerator RandomAnimationRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minTime, maxTime);

            yield return new WaitForSeconds(waitTime);
            print("Triggering moon animation after " + waitTime + " seconds");
            animator.SetTrigger("Animate");
        }
    }
}
