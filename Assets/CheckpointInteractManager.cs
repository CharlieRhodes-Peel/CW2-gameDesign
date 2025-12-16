using UnityEngine;

public class CheckpointInteractManager : MonoBehaviour
{
    public static CheckpointInteractManager Instance;

    private void Awake() // Singleton stuff
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    
    
    
}
