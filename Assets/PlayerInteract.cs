using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerInteract : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference interactInput;

    //Events
    [HideInInspector] public static event Action<Actor> PlayerInteractWith;
    
    //This gets triggered everytime the player presses the interact key
    private void Interact(InputAction.CallbackContext ctx)
    {
        Actor closestActor = InteractManager.GetClosestActor();

        if (closestActor == null) { return; }
        
        PlayerInteractWith?.Invoke(closestActor); //Talk to the closest actor
    }

    //Subscribes to input events when player is enabled and vice versa
    private void OnEnable()
    {
        interactInput.action.started += Interact;
    }
    
    private void OnDisable()
    {
        interactInput.action.started -= Interact;
    }
}
