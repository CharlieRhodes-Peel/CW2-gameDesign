using System;
using System.Collections.Generic;
using UnityEngine;

public class BossDeath : MonoBehaviour
{
    [SerializeField] private List<Dialogue> dialoguesToMakeInactive;
    [SerializeField] private List<Dialogue> dialoguesToMakeActive;

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
    }
}
