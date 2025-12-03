using System;
using UnityEngine;

public class BossRoomCamera : MonoBehaviour
{
    public static event Action playerEnteredBossRoom;
    public static event Action playerExitedBossRoom;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerEnteredBossRoom?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerExitedBossRoom?.Invoke();
        }
    }
}
