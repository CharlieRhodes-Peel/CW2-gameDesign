using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractManager : MonoBehaviour
{
    public static InteractManager Instance;
    
    private static List<IInteractable> interactablesInRange = new List<IInteractable>();
    
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private Transform playerTransform;
    
    private static IInteractable closestInteractable;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    void Start()
    {
        StartCoroutine(WaitForTxtMeshProBug()); //Helps lag
    }

    // Update is called once per frame
    void Update()
    {
        if (interactablesInRange.Count < 1)
        {
            closestInteractable = null;
            DisablePopup();
            return;
        }
        
        EnablePopup();
        
        closestInteractable = FindClosestInteractable();

        if (closestInteractable != null)
        {
            PlaceText(closestInteractable.GetInteractPopupPosition());
            popupText.text = closestInteractable.GetInteractPopupText();
        }
    }

    private IInteractable FindClosestInteractable()
    {
        IInteractable closest = null;
        float closestDistance = float.MaxValue;
        
        foreach (IInteractable interactable in interactablesInRange)
        {
            // Cast to MonoBehaviour to access transform
            MonoBehaviour mb = interactable as MonoBehaviour;
            if (mb == null) continue;
            
            float distance = Vector3.Distance(mb.transform.position, playerTransform.position);

            if (distance < closestDistance)
            {
                closest = interactable; 
                closestDistance = distance;
            }
        }
        return closest;
    }

    private void PlaceText(Vector3 pos)
    {
        popupText.transform.position = Camera.main.WorldToScreenPoint(pos);
    }

    private void DisablePopup()
    {
        popupText.gameObject.SetActive(false);
    }

    private void EnablePopup()
    {
        popupText.gameObject.SetActive(true);
    }

    public static void RegisterInteractable(IInteractable interactable)
    {
        if (!interactablesInRange.Contains(interactable))
        {
            interactablesInRange.Add(interactable);
        }
    }

    public static void UnregisterInteractable(IInteractable interactable)
    {
        interactablesInRange.Remove(interactable);
    }
    

    public static IInteractable GetClosestInteractable()
    {
        return closestInteractable;
    }

    private IEnumerator WaitForTxtMeshProBug()
    {
        popupText.gameObject.SetActive(true);
        yield return new WaitForFixedUpdate();
        popupText.gameObject.SetActive(false);
    }
}
