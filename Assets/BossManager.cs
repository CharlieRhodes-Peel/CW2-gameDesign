using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance;
    
    //Stores bosses names
    public static HashSet<Bosses> bossesHelped = new HashSet<Bosses>();
    public static HashSet<Bosses> bossesKilled = new HashSet<Bosses>();

    public enum Bosses
    {
        None,
        Log,
        Eel,
        Heron
    }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    
        Instance = this;
    }

    public void BossKilled(Bosses boss)
    {
        bossesKilled.Add(boss);
        
        CheckEnding();
    }

    public void BossHelped(Bosses boss)
    {
        bossesHelped.Add(boss);
        CheckEnding();
    }

    public void CheckEnding()
    {
        if (bossesHelped.Count + bossesKilled.Count >= 3)
        {
            EndingManager.StartEndingQuest();
        }
    }
    
    public bool HasBossBeenHelped(Bosses boss)
    {
        return bossesHelped.Contains(boss);
    }

    public bool HasBossBeenKilled(Bosses boss)
    {
        return bossesKilled.Contains(boss);
    }
}
