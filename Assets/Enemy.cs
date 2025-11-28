using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health = 2;
    [SerializeField] private float damageFlashDuration;
    [SerializeField] private Color damageFlashColor;
    [SerializeField] private float timeStopDuration;
    
    private Rigidbody2D rb;
    private SpriteRenderer renderer;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void TakeDamage(float damage, float knockback, Vector2 knockbackForcePos)
    {
        //Damage
        health -= damage;
        
        //Force
        Vector2 direction = (rb.position - knockbackForcePos).normalized + Vector2.up;
        rb.AddForce(direction * knockback, ForceMode2D.Impulse);
        
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
        Destroy(gameObject);
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
}
