using System;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoneySpawner : MonoBehaviour
{
    [SerializeField] public int moneyToSpawn;
    [SerializeField] private GameObject moneyPrefab;

    [MinMaxSlider(-50f, 50f)] [SerializeField] private Vector2 xForceRange;
    [MinMaxSlider(-50f, 50f)] [SerializeField] private Vector2 yForceRange;

    public void Spawn()
    {
        for (int i = 0; i < moneyToSpawn; i++)
        {
            GameObject newMoney = Instantiate(moneyPrefab, transform.position, Quaternion.identity);
            
            //Get random force between range
            float randomX = UnityEngine.Random.Range(xForceRange.x, xForceRange.y);
            float randomY = UnityEngine.Random.Range(yForceRange.x, yForceRange.y);
            
            newMoney.GetComponent<Rigidbody2D>().AddForce(new Vector2(randomX, randomY), ForceMode2D.Impulse);
        }
    }
}
