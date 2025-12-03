using System;
using System.Collections;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float knockbackForce;
    
    [SerializeField] private float invulnerabilityTime;
    
    [SerializeField] private int playerLayerID;
    [SerializeField] private int enemyLayerID;
    
    private bool isInvulnerable = false;
    
    private Rigidbody2D rb;

    public static event Action OnPlayerHit;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (isInvulnerable) { return; }
        
        if (other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1, other);
            OnPlayerHit?.Invoke();
        }
    }

    private void TakeDamage(float damage, Collision2D from)
    {
        //Health
        health -= damage;
        
        //Death check
        if (health <= 0) { Die(); }

        //Knockback
        Vector2 dir = (transform.position - from.gameObject.transform.position).normalized;
        rb.AddForceX(dir.x * knockbackForce,  ForceMode2D.Impulse);
        rb.AddForceY(knockbackForce, ForceMode2D.Impulse);
        
        //Invulnerability
        StartCoroutine(Invulnerability());
    }

    private void Die()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    private IEnumerator Invulnerability()
    {
        isInvulnerable = true;
        Physics2D.IgnoreLayerCollision(playerLayerID, enemyLayerID, true);
        
        yield return new WaitForSeconds(invulnerabilityTime);
        
        Physics2D.IgnoreLayerCollision(playerLayerID, enemyLayerID, false);
        isInvulnerable = false;
    }
}
