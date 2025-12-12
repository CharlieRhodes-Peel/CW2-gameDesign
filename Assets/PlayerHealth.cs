using System;
using System.Collections;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float knockbackForce;
    
    [SerializeField] private float invulnerabilityTime;
    
    [SerializeField] private int playerLayerID;
    [SerializeField] private int enemyLayerID;
    
    [SerializeField] private HealthUI healthUI;
    
    private bool isInvulnerable = false;
    
    private Rigidbody2D rb;

    public static float maxHealth;
    
    
    //Events
    public static event Action<float> OnPlayerHit; //int represents the new health the player is now on
    public static event Action OnPlayerDeath;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        maxHealth = health;
        SetHealthTo(health);
        
        SceneSwitchManager.onSceneLoaded += CheckRespawn; //Checks if the player needs to respawn when the a new scene is loaded
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (isInvulnerable) { return; }
        
        if (other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1, other);
        }
    }

    private void TakeDamage(float damage, Collision2D from)
    {
        //Health
        SetHealthTo(health - damage);
        
        //Death check
        if (health <= 0) { Die(); }

        //Knockback
        Vector2 dir = (transform.position - from.gameObject.transform.position).normalized;
        rb.AddForceX(dir.x * knockbackForce,  ForceMode2D.Impulse);
        rb.AddForceY(knockbackForce, ForceMode2D.Impulse);
        
        //Invulnerability
        StartCoroutine(Invulnerability());
        
        OnPlayerHit?.Invoke(health);
    }

    private void Die()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
        OnPlayerDeath?.Invoke();
    }

    private IEnumerator Invulnerability()
    {
        isInvulnerable = true;
        Physics2D.IgnoreLayerCollision(playerLayerID, enemyLayerID, true);
        
        yield return new WaitForSeconds(invulnerabilityTime);
        
        Physics2D.IgnoreLayerCollision(playerLayerID, enemyLayerID, false);
        isInvulnerable = false;
    }

    private void CheckRespawn()
    {
        gameObject.SetActive(true);
        
        StartCoroutine(Invulnerability()); //Makes the player init invulnerable
        
        SetHealthTo(maxHealth);
    }

    private void SetHealthTo(float value)
    {
        health = value;
        healthUI.UpdateHealthUITo(health);
    }
}
