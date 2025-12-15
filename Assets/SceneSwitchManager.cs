using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitchManager : MonoBehaviour
{
    private static SceneSwitchManager instance;
    
    private static DoorTrigger.DoorToSpawnAt targetDoor = DoorTrigger.DoorToSpawnAt.None;
    
    [SerializeField] private Transform player;
    [SerializeField] private CinemachineConfiner2D camBoundaryComponent;
    [SerializeField] private CinemachineCamera camBrain;
    [SerializeField] private CinemachineCamera bossCamera;
    
    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup fadeOut;
    [SerializeField] private float fadeTime;
    
    //Private flags for player positioning logic
    private bool playerToDoor = false;
    private bool playerToCheckpoint = false;
    
    public static event Action onSceneLoaded;
    public static event Action onSceneExit;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnEnable()
    {
        DoorTrigger.OnDoorTriggered += DoorTriggered;
        SceneManager.sceneLoaded += OnSceneLoaded;
        CheckpointManager.PlayerShouldRespawn += PlayerRespawning;
    }

    private void OnDisable()
    {
        DoorTrigger.OnDoorTriggered -= DoorTriggered;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        CheckpointManager.PlayerShouldRespawn -= PlayerRespawning;
    }

    //Called when a door is triggered
    private void DoorTriggered(string sceneToLoad, DoorTrigger door)
    {
        playerToDoor = true;
        playerToCheckpoint = false;
        StartCoroutine(SceneExit(sceneToLoad, door));
    }
    
    //Called when the player should respawn
    private void PlayerRespawning(string sceneToLoad)
    {
        playerToDoor = false;
        playerToCheckpoint = true;
        StartCoroutine(SceneExit(sceneToLoad, null));
    }
    
    
    private IEnumerator SceneExit(string sceneToLoad, DoorTrigger door)
    {
        if (playerToDoor) {targetDoor = door.doorToSpawnAt;}
        
        camBoundaryComponent.InvalidateBoundingShapeCache(); //Gets rid of the previous bounding cache
        
        yield return StartCoroutine(Fade(1));
        onSceneExit?.Invoke();
        
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SceneInit());
    }

    private IEnumerator SceneInit()
    {
        yield return null; //Waits one frame so scene is properly loaded

        if (playerToDoor) //If the player when through the door then do the following
        {
            DoorTrigger[] doors = FindObjectsOfType<DoorTrigger>();
            foreach (DoorTrigger door in doors)
            {
                if (door.currentDoor == targetDoor)
                {
                    MovePlayerAndCamTo(door.exitOffset.position);

                    yield return null; //For camera to move
                
                    CameraSceneInitProcedure();
                    
                    break;
                }
            }
        }
        
        else if (playerToCheckpoint) //Else if the scene is loading because the player is respawning do the following
        {
            Checkpoint checkpoint = FindObjectOfType<Checkpoint>();
            MovePlayerAndCamTo(checkpoint.GetCheckpointPosition());
            
            yield return null;
            CameraSceneInitProcedure();
        }
        
        StartCoroutine(Fade(0));
        
        yield return new WaitForSeconds(fadeTime);
        camBoundaryComponent.Damping = 1;
        
        onSceneLoaded?.Invoke();
    }

    private IEnumerator Fade(float targetAlpha)
    {
        LeanTween.cancel(fadeOut.gameObject);
        
        LeanTween.alphaCanvas(fadeOut, targetAlpha, fadeTime);
        
        yield return new WaitForSeconds(fadeTime);
    }

    private void CameraSceneInitProcedure()
    {
        //The most expensive single call known to man
        camBoundaryComponent.BoundingShape2D = GameObject.FindGameObjectWithTag("CameraBounds").GetComponent<CompositeCollider2D>();
        camBoundaryComponent.Damping = 0;
        camBoundaryComponent.InvalidateBoundingShapeCache(); //Just in case
        camBoundaryComponent.BakeBoundingShape(camBrain, fadeTime);

        GameObject bossRoomCamPos = GameObject.FindGameObjectWithTag("BossRoomCamera");
        if (bossRoomCamPos != null) //If there is a boss room camera
        {
            bossCamera.Follow = bossRoomCamPos.transform;
        }
    }
    private void MovePlayerAndCamTo(Vector3 targetPosition)
    {
        player.position = targetPosition;
        camBrain.ForceCameraPosition(targetPosition, Quaternion.identity);
    }
}
