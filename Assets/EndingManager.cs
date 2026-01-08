using System;
using UnityEngine;

public class EndingManager : MonoBehaviour
{    
    public static EndingManager Instance;

    [SerializeField] private Color logColour;
    [SerializeField] private Color eelColour;
    [SerializeField] private Color heronColour;

    private bool ending = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    
        Instance = this;
    }
    public Quest quest;

    private void Start()
    {
        GameObject[] lights = GameObject.FindGameObjectsWithTag("Light");

        foreach (GameObject light in lights)
        {
            light.GetComponent<Light>().intensity = logColour.r;
        }
    }

    public static void StartEndingQuest()
    {
        NpcActor.StartQuest(Instance.quest);
        Instance.ending = true;
    }
}
