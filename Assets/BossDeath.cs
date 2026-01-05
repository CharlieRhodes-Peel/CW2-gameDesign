using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class BossDeath : MonoBehaviour
{
    [SerializeField] private List<Dialogue> dialoguesToMakeInactive;
    [SerializeField] private List<Dialogue> dialoguesToMakeActive;
    [SerializeField] private GameObject doorBlock;
    [SerializeField] private bool spawnsObjectOnDeath = false;
    [ShowIf("spawnsObjectOnDeath")] [SerializeField] private GameObject objectSpawnOnDeath;
    [SerializeField] private BossManager.Bosses bossType;

    public void DoBossDeathDialoguesToggles()
    {
        foreach (Dialogue dialogue in dialoguesToMakeInactive)
        {
            NpcActor.MakeDialogueInactive(dialogue);
        }

        foreach (Dialogue dialogue in dialoguesToMakeActive)
        {
            NpcActor.MakeDialogueActive(dialogue);
        }
        
        doorBlock.SetActive(false);

        if (spawnsObjectOnDeath) { Instantiate(objectSpawnOnDeath, transform.position, Quaternion.identity); }

        BossManager.Instance.BossKilled(bossType);
    }
}
