using System.Collections.Generic;
using UnityEngine;

public class EnemyPermaDeathManager : MonoBehaviour
{
    public static HashSet<string> permaDeadEnemies = new HashSet<string>();

    public static void GrantPermaDeath(string uniqueID)
    {
        permaDeadEnemies.Add(uniqueID);
    }
}
