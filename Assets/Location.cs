using System;
using UnityEngine;

public class Location : MonoBehaviour
{
    [SerializeField] private string locationName;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LocationManager.LocationVisted(locationName);
        }
    }
}
