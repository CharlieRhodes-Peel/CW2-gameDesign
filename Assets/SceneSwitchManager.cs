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
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        DoorTrigger.OnDoorTriggered += DoorTriggered;
        SceneManager.sceneLoaded += OnSceneLoaded;
        
    }

    private void OnDisable()
    {
        DoorTrigger.OnDoorTriggered -= DoorTriggered;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Called when a door is triggered
    private void DoorTriggered(string sceneToLoad, DoorTrigger door)
    {
        StartCoroutine(SceneExit(sceneToLoad, door));
    }
    
    private IEnumerator SceneExit(string sceneToLoad, DoorTrigger door)
    {
        targetDoor = door.doorToSpawnAt;
        camBoundaryComponent.InvalidateBoundingShapeCache(); //Gets rid of the previous bounding cache
        
        yield return StartCoroutine(Fade(1));
        
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SceneInit());
    }

    private IEnumerator SceneInit()
    {
        yield return null; //Waits one frame so scene is properly loaded
        
        DoorTrigger[] doors = FindObjectsOfType<DoorTrigger>();
        foreach (DoorTrigger door in doors)
        {
            if (door.currentDoor == targetDoor)
            {
                player.position = door.exitOffset.position;
                
                camBrain.ForceCameraPosition(door.exitOffset.position, Quaternion.identity);

                yield return null; //For camera to move
                
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
                
                break;
            }
        }
        
        
        StartCoroutine(Fade(0));
        
        yield return new WaitForSeconds(fadeTime);
        camBoundaryComponent.Damping = 1;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        LeanTween.cancel(fadeOut.gameObject);
        
        LeanTween.alphaCanvas(fadeOut, targetAlpha, fadeTime);
        
        yield return new WaitForSeconds(fadeTime);
    }
}
