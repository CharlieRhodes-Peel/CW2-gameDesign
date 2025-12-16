using UnityEngine;

public class DialogueQuestBridge : MonoBehaviour
{
    public static DialogueQuestBridge Instance;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    
    
}
