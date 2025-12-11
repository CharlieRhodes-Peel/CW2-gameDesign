using System;
using UnityEngine;

public class SpeakRange : MonoBehaviour
{
    private GameObject parent;
    private NpcActor npc;
    private void Start()
    {
        parent = transform.parent.gameObject;
        npc = parent.GetComponent<NpcActor>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            npc.PlayerEnterSpeakingRange(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            npc.PlayerExitSpeakingRange(other.transform);
        }
    }
}
