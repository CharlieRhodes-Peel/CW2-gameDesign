using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera mainCamera;
    [SerializeField] private CinemachineCamera bossCamera;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        BossRoomCamera.playerEnteredBossRoom += EnteredBossRoom;
        BossRoomCamera.playerExitedBossRoom += ExitedBossRoom;
    }

    void OnDisable()
    {
        BossRoomCamera.playerEnteredBossRoom -= EnteredBossRoom;
        BossRoomCamera.playerExitedBossRoom -= ExitedBossRoom;
    }

    private void EnteredBossRoom()
    {
        mainCamera.Priority = 0;
        bossCamera.Priority = 1;
    }

    private void ExitedBossRoom()
    {
        mainCamera.Priority = 1;
        bossCamera.Priority = 0;
    }
}
