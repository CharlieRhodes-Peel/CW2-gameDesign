using UnityEngine;

public enum LocationType
{
    StartingArea,
    GreenArea,
    RedArea,
    BlueArea
}

[CreateAssetMenu(fileName = "New Location Data", menuName = "Location/New Location")]
public class Location : ScriptableObject
{
    public LocationType locationType;
    public string Name;
}
