using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class FriendshipManager : MonoBehaviour
{
    public static FriendshipManager Instance;

    [MinMaxSlider(-50,50)] [SerializeField] private Vector2Int friendshipMeterRange;

    [SerializeField] private float updateDelay = 0.4f;
    
    [SerializeField] private List<Location> locations;
    [SerializeField] private Animator animator;

    [SerializeField] private RectTransform minPos;
    [SerializeField] private RectTransform maxPos;

    [SerializeField] private RectTransform pointer;
    
    private float pointerRatio;
    
    //Stores location and there respective friendship levels
    private static Dictionary<string, int> friendshipLevels =  new Dictionary<string, int>();
    
    private float minX;
    private float maxX;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else{Destroy(gameObject);}
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Location l in locations)
        {
            friendshipLevels.Add(l.Name, 0);
        }
        
        minX = minPos.position.x;
        maxX = maxPos.position.x;
        
        StartCoroutine(UpdatePointerPosition(0));
    }

    public void AddToFriendshipLevel(int amount)
    {
        if (amount == 0) { return;}
        
        string location = LocationManager.currentLocation.Name; 
        
        friendshipLevels[location] += amount;
        friendshipLevels[location] = Mathf.Clamp(friendshipLevels[location], friendshipMeterRange.x, friendshipMeterRange.y);
        
        Instance.animator.SetTrigger("LevelChanged");
        
        StartCoroutine(UpdatePointerPosition(friendshipLevels[LocationManager.currentLocation.Name]));
    }
    
    private IEnumerator UpdatePointerPosition(int friendshipLevel)
    {
        yield return new WaitForSeconds(updateDelay);
        // Map friendship level (-50 to 50) to position (minX to maxX)
        float normalizedValue = (friendshipLevel - friendshipMeterRange.x) / 
                                (float)(friendshipMeterRange.y - friendshipMeterRange.x);
        
        float targetX = Mathf.Lerp(minX, maxX, normalizedValue);
        
        // SET the position, don't ADD to it
        pointer.position = new Vector3(targetX, pointer.position.y, pointer.position.z);
    }

    public static int GetCurrentFriendshipLevel()
    {
        return friendshipLevels[LocationManager.currentLocation.Name];
    }
}
