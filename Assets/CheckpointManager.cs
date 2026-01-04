using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private float respawnDelayTimer;
    private string checkpointSceneName;
    public static event Action<string> PlayerShouldRespawn;

    private void Start()
    {
        //Sets the current checkpoint in the first scene to be the default one
        checkpointSceneName = SceneManager.GetActiveScene().name; 
    }

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDeath += PlayerDied;
        Checkpoint.CheckpointActivated += CheckpointActivated;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDeath -= PlayerDied;
    }

    //Called on player death
    private void PlayerDied()
    {
        StartCoroutine(Respawn());
    }
    
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelayTimer);
        
        PlayerShouldRespawn?.Invoke(checkpointSceneName); // Tells scene manager that player should respawn
        GameObject[] doorBlocks = GameObject.FindGameObjectsWithTag("DoorBlock");

        foreach (GameObject doorBlock in doorBlocks)
        {
            doorBlock.SetActive(false);
        }
    }

    //Called anytime a player enters a checkpoint
    private void CheckpointActivated(Checkpoint checkpointEntered)
    {
        checkpointSceneName = SceneManager.GetActiveScene().name;
    }
}