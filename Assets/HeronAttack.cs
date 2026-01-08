using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NpcStates))]
[RequireComponent(typeof(NpcActor))]
public class HeronAttack : MonoBehaviour
{
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private float spawnDelay = 0.5f;

    [MinMaxSlider(0, 20)] [SerializeField] private Vector2 randomAttackEvery;
    
    [SerializeField] private Transform[] spawnPoints;


    private NpcStates bossState;
    private NpcActor npcActor;
    private void Start()
    {
        bossState = GetComponent<NpcStates>();  
        npcActor = GetComponent<NpcActor>();
    }

    //Called whenever the state of an npc is changed
    private void StateChanged(string bossName, NpcStates.State state)
    {
        //Check that we should be doing something
        if (npcActor.Name != bossName || state != NpcStates.State.Angry) { return; }
        
        //Start attack coroutine
        StartCoroutine(BossAttack(0));
    }

    private IEnumerator BossAttack(int count)
    {
        Transform spawnPos;
        
        //Do a random one every now and then
        if (count >= Random.Range(randomAttackEvery.x, randomAttackEvery.y))
        {
            spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)];
            count = 0;
        }
        else
        {
            spawnPos = GetClosestPointToPlayer();
            count++;
        }
        
        //Spawn the object
        Instantiate(attackPrefab,  spawnPos.position, Quaternion.identity);
        
        //Wait
        yield return new WaitForSeconds(spawnDelay);
        //Check if dead
        if (gameObject == null) { yield break; }
        //Call function again
        StartCoroutine(BossAttack(count));
    }

    private Transform GetClosestPointToPlayer()
    {
        //Get the player transform
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        
        //Create stores
        Transform closestPoint = null;
        float closestDistance = float.MaxValue;
        //Go through all the points and get the smallest distance
        foreach (Transform point in spawnPoints)
        {
            float distance = Vector2.Distance(player.position, point.position);
            
            if (distance < closestDistance) { closestDistance = distance; closestPoint = point; }
        }
        //Return closets point to player
        return closestPoint;
    }

    private void OnEnable()
    {
        NpcStates.OnStateChangedTo += StateChanged;
    }

    private void OnDisable()
    {
        NpcStates.OnStateChangedTo -= StateChanged;
    }
}
