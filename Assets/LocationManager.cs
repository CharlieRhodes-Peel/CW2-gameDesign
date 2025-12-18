using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance;
    
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshProUGUI locationText;
    
    private static HashSet<string> visitedLocations = new HashSet<string>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else{Destroy(gameObject);}
    }

    public static void LocationVisted(string location)
    {
        location = location.ToLower();

        foreach (string visitedLocation in visitedLocations)
        {
            Debug.Log("We have already visited: " + visitedLocation);
        }

        if (visitedLocations.Contains(location)) {return;} //If we have already visited the trigger then don't do anything!
        
        visitedLocations.Add(location);
        
        Instance.locationText.text = location;
        
        Instance.animator.SetTrigger("showLocation");
    }
}
