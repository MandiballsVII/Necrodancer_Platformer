using System.Collections;
using UnityEngine;

public class FallKill : MonoBehaviour
{
    private GameObject player;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    IEnumerator PlayerKillerWaiter()
    {
        yield return new WaitForSeconds(0.2f);
        player.GetComponent<PlayerController>().Die();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;
        Camera.main.GetComponent<CameraFollow>().enabled = false;
        StartCoroutine(PlayerKillerWaiter());
    }

}
