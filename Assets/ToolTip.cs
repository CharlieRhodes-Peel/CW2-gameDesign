using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ToolTip : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    
    public LayoutElement layoutElement;

    public int characterWrapLimit;
    
    public void SetText(string titleText, string descriptionText = "")
    {
        if (string.IsNullOrEmpty(titleText))
        {
            title.gameObject.SetActive(false);
        }
        else
        {
            title.gameObject.SetActive(true);
            title.text = titleText;
        }
        
        description.text = descriptionText;
    }

    private void Update()
    {
        int titleLength = title.text.Length;
        int descriptionLength = description.text.Length;
        
        layoutElement.enabled = (titleLength > characterWrapLimit || descriptionLength > characterWrapLimit) ? true : false;
        
        Vector2 position = Input.mousePosition;
        transform.position = position;
    }
}
