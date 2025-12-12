using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fullHeart;
    [SerializeField] private Image noHeart;
    [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;
    
    //Gets called whenever the player is hit, is also just callable from anywhere
    public void UpdateHealthUITo(float newHealth)
    {
        
        //Get rid of all existing hearts
        foreach (Transform child in horizontalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        //Fill up the full hearts with how much health we have
        for (int i = 0; i < newHealth; i++)
        {
            Instantiate(fullHeart, horizontalLayoutGroup.transform);
        }
        
        //Full up the rest of the heart we what health we don't have
        for (int i = 0; i < PlayerHealth.maxHealth - newHealth; i++)
        {
            Instantiate(noHeart, horizontalLayoutGroup.transform);
        }
    }

    private void OnEnable()
    {
        PlayerHealth.OnPlayerHit += UpdateHealthUITo;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerHit += UpdateHealthUITo;
    }
}
