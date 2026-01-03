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

    public void SetText(string titleText, string descriptionText = "")
    {
        title.text = titleText;
        description.text = descriptionText;
    }

    public void Show(string title, string description)
    {
        SetText(title, description);
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
