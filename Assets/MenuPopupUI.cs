using TMPro;
using UnityEngine;

public class MenuPopupUI : MonoBehaviour
{
    public static MenuPopupUI Instance;
    
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private Animator animator;
    
    [SerializeField] private AudioClip[] openPopupSounds;
    [SerializeField] private AudioClip[] closePopupSounds;
    
    private void Awake()
    {
        // Singleton pattern to ensure only one instance of DialogueManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void ShowPopup(ItemData item)
    {
        popupText.text = $"Item: {item.itemName} \nAcquired [TAB]";
        DoVisualsAndSound();
    }

    public void ShowPopup(Quest quest, bool acquired)
    {
        if (acquired)
        {
            popupText.text = $"Quest: {quest.questName} \nStarted [TAB]";
        }
        else
        {
            popupText.text = $"Quest: {quest.questName} \nCompleted";
        }
        DoVisualsAndSound();
    }

    private void DoVisualsAndSound()
    {
        animator.SetTrigger("Show");
        OpenSound();
    }

    public void OpenSound()
    {
        SoundManager.Instance.PlayRandomSoundEffect(openPopupSounds, transform, 1);
    }

    public void CloseSound()
    {
        SoundManager.Instance.PlayRandomSoundEffect(closePopupSounds, transform, 1);
    }
}
