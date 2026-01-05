using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance;

    [SerializeField] private int positiveRelationsFriendshipAffect = 20;
    [SerializeField] private int negativeRelationsFriendshipAffect = -20;
    
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshProUGUI locationText;
    
    private static HashSet<Location> visitedLocations = new HashSet<Location>();

    public static Location currentLocation; //Current region

    public static event Action<string> OnLocationVisited;

    [SerializeField] private Location startingLocation;

    public static event Action<Location> OnLocationChanged;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else{Destroy(gameObject);}
    }

    private void Start()
    {
        currentLocation = startingLocation;
    }

    public static void LocationVisted(Location location)
    {
        currentLocation = location;
        
        OnLocationChanged?.Invoke(location);

        foreach (Location visitedLocation in visitedLocations)
        {
            Debug.Log("We have already visited: " + visitedLocation.name);
        }

        if (visitedLocations.Contains(location)) {return;} //If we have already visited the trigger then don't do anything!
        
        visitedLocations.Add(location);
        
        Instance.locationText.text = location.Name;
        Instance.animator.SetTrigger("showLocation");
        
        DoLocationRelationFriendshipLevel(location);

        OnLocationVisited?.Invoke(location.Name);
    }

    private static void DoLocationRelationFriendshipLevel(Location location)
    {
        //Negative affect
        if (BossManager.Instance.HasBossBeenHelped(location.dislikes) || BossManager.Instance.HasBossBeenKilled(location.likes))
        {
            FriendshipManager.Instance.AddToFriendshipLevel(Instance.negativeRelationsFriendshipAffect);
        }

        //Positive Affect
        if (BossManager.Instance.HasBossBeenHelped(location.likes) || BossManager.Instance.HasBossBeenKilled(location.dislikes))
        {
            FriendshipManager.Instance.AddToFriendshipLevel(Instance.positiveRelationsFriendshipAffect);
        }
    }
}
