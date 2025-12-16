using System;
using UnityEngine;

public class ToolTipSystem : MonoBehaviour
{
    public static ToolTipSystem instance;

    public ToolTip tooltip;

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
        instance.tooltip.gameObject.SetActive(false);
    }

    public static void Show(string title, string description)
    {
        instance.tooltip.SetText(title, description);
        
        instance.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        instance.tooltip.gameObject.SetActive(false);
    }
}
