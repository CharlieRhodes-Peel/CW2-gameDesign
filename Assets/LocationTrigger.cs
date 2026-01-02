using System;
using UnityEngine;

public class LocationTrigger : MonoBehaviour
{
    [SerializeField] private Location location;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LocationManager.LocationVisted(location);
        }
    }
}
