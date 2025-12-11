using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static event Action<Checkpoint> OnPlayerEnteredCheckpoint;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) { return; }
        
        OnPlayerEnteredCheckpoint?.Invoke(this);
    }
    
    public Vector3 GetCheckpointPosition(){
        return transform.position;
    }
}