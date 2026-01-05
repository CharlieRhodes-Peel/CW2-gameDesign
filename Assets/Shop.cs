using System;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public List<ItemData> shopItems = new List<ItemData>();

    private void Start()
    {
        foreach (ItemData item in shopItems)
        {
            MenuUI.Instance.AddItemToShopUI(item);
        }
    }

    public static void OpenShop()
    {
        MenuUI.Instance.OpenShop();
    }
}
