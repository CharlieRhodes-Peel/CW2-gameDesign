using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Checkpoint : MonoBehaviour, IInteractable
{
    [Header("Checkpoint Settings")]
    [SerializeField] private Transform playerSpawnPos;
    [SerializeField] private Transform wishPromptPos;
    [SerializeField] private string wishPromptText;
    
    [Header("Visuals")]
    [SerializeField] private float anticipationTime;
    [SerializeField] private GameObject checkpointActiviatedParticles;
    [SerializeField] private GameObject checkpointAnticipationParticles;
    
    public static event Action<Checkpoint> CheckpointActivated;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) { return; }
        
        InteractManager.RegisterInteractable(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if  (!other.CompareTag("Player")) { return; }
        
        InteractManager.UnregisterInteractable(this);
    }

    public Vector3 GetCheckpointPosition(){
        return playerSpawnPos.position;
    }

    public void Interact()
    {
        CheckpointActivated?.Invoke(this);
        StartCoroutine(Particles());
        InteractManager.UnregisterInteractable(this);
    }

    public Vector3 GetInteractPopupPosition()
    {
        return wishPromptPos.position;
    }

    public string GetInteractPopupText()
    {
        return wishPromptText;
    }

    private IEnumerator Particles()
    {
        Instantiate(checkpointAnticipationParticles, transform.position, Quaternion.identity);
        
        yield return new WaitForSecondsRealtime(anticipationTime);
        Instantiate(checkpointActiviatedParticles, transform.position, Quaternion.identity);
    }
}