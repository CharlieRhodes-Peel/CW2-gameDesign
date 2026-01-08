using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveRight : MonoBehaviour
{
    [SerializeField] private float acceleration = 1.05f ;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new  Vector2(1, 0);
        
        //Clean up
        Destroy(gameObject, 10f);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocityX * acceleration, 0);
    }
}