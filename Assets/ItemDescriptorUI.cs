using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDescriptorUI : MonoBehaviour
{
    public static ItemDescriptorUI instance;

    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Image icon;
    
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

    public void Set(ItemUI itemUI)
    {
        title.text = itemUI.itemName.text;
        description.text = itemUI.description;
        icon.sprite = itemUI.itemIcon.sprite;
    }

    public void Show(ItemUI itemUI)
    {
        Set(itemUI);
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
