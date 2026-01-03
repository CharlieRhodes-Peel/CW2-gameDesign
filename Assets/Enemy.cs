using System;
using System.Collections;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Identifying")]
    [SerializeField] private bool nonNpcEnemy = true;
    [ShowIf("nonNpcEnemy")] [SerializeField] private string enemyName; //DON'T NEED to fill this in if NPC Actor, this assigns names to non npc enemies
    
    [Header("Stats")]
    [SerializeField] private float health = 2;

    [SerializeField] private int moneyOnDeath;
        
    [SerializeField] private float knockbackX;
    [SerializeField] private float knockbackY;
    
    [Header("Visuals")]
    [SerializeField] private float damageFlashDuration;
    [SerializeField] private Color damageFlashColor;
    [SerializeField] private float timeStopDuration;
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private GameObject deathParticles2;
    [SerializeField] private float disableMovementTime = 0.5f;
    
    [Header("References")]
    private Rigidbody2D rb;
    private NpcStates npcStates;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private GameObject moneySpawner;
    
    //Events
    public static event Action<string> OnEnemyDeathEvent; //Called to let other scripts know the name of the enemy that just died
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        npcStates = GetComponent<NpcStates>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void TakeDamage(float damage, Vector2 knockbackForcePos)
    {
        //Damage
        health -= damage;
        
        //Change States
        if (npcStates != null)
        { npcStates.SetCurrentState(NpcStates.State.Angry); }
        
        //Force
        StartCoroutine(DisableMovement());
        
        rb.linearVelocity = Vector2.zero; //Reset velocity
        Vector2 direction = (rb.position - knockbackForcePos).normalized;
        
        rb.AddForceX(direction.x * knockbackX, ForceMode2D.Impulse);
        rb.AddForceY(Vector2.up.y * knockbackY, ForceMode2D.Impulse);
        
        //Visual Impact
        StartCoroutine(DamageFlash());
        StartCoroutine(TimeStop());
        
        //Death Logic
        CheckHealth();
    }

    private void CheckHealth()
    {
        if (health <= 0)
        {
            StartCoroutine(Death());
        }
    }

    private IEnumerator Death()
    {
        yield return new WaitUntil(()=> Time.timeScale == 1);
        
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        if (deathParticles2 != null) {Instantiate(deathParticles2, transform.position, Quaternion.identity);}

        SpawnMoney();
        
        NpcActor actor = GetComponent<NpcActor>();
        if (actor != null) //If this is an Npc then take the npc name
        {
            OnEnemyDeathEvent?.Invoke(actor.Name);
        }
        else if (nonNpcEnemy)
        {
            OnEnemyDeathEvent?.Invoke(enemyName);
        }

        Destroy(gameObject);
    }

    private void SpawnMoney()
    {
        if (moneyOnDeath < 1) {return;}
        if (moneySpawner == null) {return;}

        MoneySpawner moneySpawnerInScene = Instantiate(moneySpawner, transform.position, Quaternion.identity).GetComponent<MoneySpawner>();
        moneySpawnerInScene.moneyToSpawn = moneyOnDeath;
        moneySpawnerInScene.Spawn();
    }

    private IEnumerator DamageFlash()
    {
        Color originalColour = renderer.color;
        renderer.color = damageFlashColor;
        yield return new WaitForSeconds(damageFlashDuration);
        renderer.color = originalColour;
    }

    private IEnumerator TimeStop()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(timeStopDuration);
        Time.timeScale = 1;
    }

    private IEnumerator DisableMovement()
    {
        FrogMovement frogMovement = GetComponent<FrogMovement>();

        if (!frogMovement.enabled) { yield break; }
        
        frogMovement.SetStopTo(true);
        yield return new WaitForSecondsRealtime(disableMovementTime);
        frogMovement.SetStopTo(false);
    }
}
