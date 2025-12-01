using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    public enum DoorToSpawnAt
    {
        None,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
    }
    
    [SerializeField] private string sceneToLoad;
    public DoorToSpawnAt doorToSpawnAt;
    public Transform exitOffset;
    
    [Header("This door")]
    public DoorToSpawnAt currentDoor;
    
    [Header("References")]
    [SerializeField] private LayerMask playerLayer;
    
    //Events
    public static event Action<string, DoorTrigger> OnDoorTriggered;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnDoorTriggered?.Invoke(sceneToLoad, this);
        }
    }
}
