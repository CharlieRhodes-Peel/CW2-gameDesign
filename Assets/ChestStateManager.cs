using System.Collections.Generic;
using UnityEngine;

public class ChestStateManager : MonoBehaviour
{
    //Holds a unique id of the opened chest
    public static HashSet<string> openedChests = new HashSet<string>();

    public static void ChestOpened(string chestID)
    {
        openedChests.Add(chestID);
    }
}
