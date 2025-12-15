using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float health;
    [SerializeField] private float knockbackForce;
    
    [Header("Getting hit visuals")]
    
    [SerializeField] private float timeStopDuration;
    [SerializeField] private Color damageFlashColor;
    [SerializeField] private float damageFlashDuration;
    [SerializeField] private GameObject blobParticles;
    [SerializeField] private GameObject smokeParticles;
    
    [Header("Invulnerability")]
    [SerializeField] private float invulnerabilityTime;
    [SerializeField] private float invulFlashTime;
    [SerializeField] private Color invulFlashColor;
     
    [Header("Getting health")] 
    [SerializeField] private float healTime;
    [SerializeField] private GameObject playerHealingParticles;
    [SerializeField] private GameObject playerHealedParticles;
    
    [Header("References")]
    [SerializeField] private int playerLayerID;
    [SerializeField] private int enemyLayerID;
    
    [SerializeField] private HealthUI healthUI;
    [SerializeField] private SpriteRenderer renderer;
    
    private bool isInvulnerable = false;
    private bool isHealing = false;
    private bool invulnerableFlashing = false;
    private bool invulFlashFlag = true;
    private bool secondaryInvulFlag = false;

    private Color baseColor;
    
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
        
        SceneSwitchManager.onSceneLoaded += CheckRespawn; //Checks if the player needs to respawn when a new scene is loaded
        
        baseColor = renderer.color;
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
        
        //Visuals
        StartCoroutine(DamageFlash());
        StartCoroutine(TimeStop());
        
        Instantiate(blobParticles, transform.position, Quaternion.identity);
        Instantiate(smokeParticles, transform.position, Quaternion.identity);
        
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
        
        StartCoroutine(InvulFlash());
        
        yield return new WaitForSeconds(invulnerabilityTime);
        
        Physics2D.IgnoreLayerCollision(playerLayerID, enemyLayerID, false);
        isInvulnerable = false;
    }

    private void CheckRespawn()
    {
        if (health > 0) {return;} //Player did not die 
            
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
    
    private IEnumerator DamageFlash()
    {
        renderer.color = damageFlashColor;
        yield return new WaitForSeconds(damageFlashDuration);
        renderer.color = baseColor;
    }

    private IEnumerator TimeStop()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(timeStopDuration);
        Time.timeScale = 1;
    }

    private void CheckpointHealthRestore(Checkpoint NOTNEEDED)
    {
        StartCoroutine(HealPlayerTo(maxHealth));
    }

    private IEnumerator HealPlayerTo(float health)
    {
        if (this.health < health && !isHealing)
        {
            isHealing = true;
            
            Instantiate(playerHealingParticles, transform.position, Quaternion.identity, transform);
            yield return new WaitForSecondsRealtime(healTime);
        
            SetHealthTo(health);
            Instantiate(playerHealedParticles, transform.position, Quaternion.identity);
            
            isHealing = false;
        }
    }

    private IEnumerator InvulFlash()
    {
        if (renderer.color == baseColor)
        {
            renderer.color = invulFlashColor;
        }
        else if (renderer.color == invulFlashColor)
        {
            renderer.color = baseColor;
        }

        yield return new WaitForSecondsRealtime(invulFlashTime);
        if (isInvulnerable) { StartCoroutine(InvulFlash()); }
        else { renderer.color = baseColor; }
    }

    
    private void OnEnable()
    {
        Checkpoint.OnPlayerEnteredCheckpoint += CheckpointHealthRestore;
    }

    private void OnDisable()
    {
        Checkpoint.OnPlayerEnteredCheckpoint -= CheckpointHealthRestore;
    }
    
}
