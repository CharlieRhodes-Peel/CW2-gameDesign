using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public Image questIcon;
    public TextMeshProUGUI questName;
    public string description;
    public Quest.QuestType questType;
    public string questGiver;
    public int amountNeeded;
    public int currentProgress;
    
    [SerializeField] private AudioClip[] clickSounds;
    
    public void RemoveUI()
    {
        Destroy(gameObject);
    }

    public void QuestSelected()
    {
        QuestDescriptorUI.instance.Show(this);
        SoundManager.Instance.PlayRandomSoundEffect(clickSounds, transform, 1);
    }
}
