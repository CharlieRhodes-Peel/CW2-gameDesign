using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private GameObject doorBlock;
    [SerializeField] private bool spawnsObjectOnDeath = false;
    [ShowIf("spawnsObjectOnDeath")] [SerializeField] private GameObject objectSpawnOnDeath;
    [SerializeField] private BossManager.Bosses bossType;

    public void DoBossDeathDialoguesToggles()
    {
        doorBlock.SetActive(false);

        if (spawnsObjectOnDeath) { Instantiate(objectSpawnOnDeath, transform.position, Quaternion.identity); }

        BossManager.Instance.BossKilled(bossType);
    }

    public void BossHelped()
    {
        BossManager.Instance.BossHelped(bossType);
    }
}
