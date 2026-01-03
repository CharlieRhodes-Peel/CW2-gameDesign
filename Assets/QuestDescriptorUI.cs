using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestDescriptorUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI questGiverName;
    [SerializeField] private TextMeshProUGUI currentProgress;
    [SerializeField] private TextMeshProUGUI amountNeeded;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI titleText;
    
    public static QuestDescriptorUI instance;
    
    public GameObject backbutton;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else {Destroy(gameObject);}
    }

    private void Start()
    {
        instance.gameObject.SetActive(false);
    }

    public void SetText(QuestUI quest)
    {
        titleText.text = quest.questName.text;
        icon.sprite = quest.questIcon.sprite;
        description.text = quest.description;
        questGiverName.text = quest.questGiver;
        currentProgress.text = MenuUI.Instance.QueryQuestProgress(quest).ToString();
        amountNeeded.text = quest.amountNeeded.ToString();
    }

    public void Show(QuestUI questUI)
    {
        SetText(questUI);
        instance.gameObject.SetActive(true);
        
        //Select Back button
        EventSystem.current.SetSelectedGameObject(instance.backbutton);
    }

    public static void Hide()
    {
        instance.gameObject.SetActive(false);
        
        MenuUI.Instance.SelectDefaultButton();
    }
}
