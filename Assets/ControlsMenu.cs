using UnityEngine;

public class ControlsMenu : MonoBehaviour
{
    //script to toggle menu control menu on and off

    public GameObject inst;

    void Update()
    {
        //if ESCAPE then close control menu
        if (inst.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            inst.SetActive(false);
        }
    }

    //activate control menu on button click
    public void ShowControls()
    {
        inst.SetActive(true);
    }
}
