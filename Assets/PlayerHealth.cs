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
    private PlayerAttack playerAttack;

    public static float maxHealth;
    
    
    //Events
    public static event Action<float> OnPlayerHit; //int represents the new health the player is now on
    public static event Action OnPlayerDeath;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAttack = GetComponent<PlayerAttack>();
        
        maxHealth = health;
        SetHealthTo(health);
        
        SceneSwitchManager.onSceneLoaded += CheckRespawn; //Checks if the player needs to respawn when the a new scene is loaded
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isInvulnerable) { return; }
        if (playerAttack.IsWindup()) { return; }
        
        if (other.gameObject.CompareTag("EnemyAttack"))
        {
            TakeDamage(1, other);
        }
    }

    private void TakeDamage(float damage, Collider2D from)
    {
        //Health
        SetHealthTo(health - damage);
        
        //Death check
        if (health <= 0) { Die(); }

        DoKnockback(from);
        
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

    private void DoKnockback(Collider2D from)
    {
        //Reset velocity
        rb.linearVelocity = Vector2.zero;
        
        //Flip around knockback x if attacking from other direction
        Vector2 dir = (transform.position - from.gameObject.transform.position).normalized;
        float knockbackX = knockbackForce;
        if (dir.x <= 0) {knockbackX *= -1; }
        
        //Apply force
        rb.AddForce(new Vector2(knockbackX, knockbackForce * 2), ForceMode2D.Impulse);
    }
}
