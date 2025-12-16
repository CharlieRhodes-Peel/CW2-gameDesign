using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static event Action<Checkpoint> OnPlayerEnteredCheckpoint;

    [SerializeField] private GameObject wishPrompt;
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) { return; }
        
        wishPrompt.SetActive(true);
        wishPrompt.transform.position = transform.position;
        
        OnPlayerEnteredCheckpoint?.Invoke(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if  (!other.CompareTag("Player")) { return; }
        
        wishPrompt.SetActive(false);
    }

    public Vector3 GetCheckpointPosition(){
        return transform.position;
    }
}