using System;
using System.Collections;
using UnityEngine;

public class ParticleCleaner : MonoBehaviour
{
    [SerializeField] private float timeTillDeath;

    private void Start()
    {
        StartCoroutine(Dying());
    }

    private IEnumerator Dying()
    {
        yield return new WaitForSeconds(timeTillDeath);
        Destroy(gameObject);
    }
}
