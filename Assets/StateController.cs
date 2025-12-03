using System;
using Unity.VisualScripting;
using UnityEngine;

public class StateController : MonoBehaviour
{
    IState currentState;

    private void Update()
    {
        currentState.UpdateState();
    }

    public void ChangeState(IState newState)
    {
        currentState.OnExit();
        currentState = newState;
        
        newState.OnEnter();
    }
}


public interface IState
{
    public void OnEnter();
    public void UpdateState();
    public void OnExit();
}