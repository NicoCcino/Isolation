using System.Collections.Generic;
using UnityEngine;

public abstract class BaseFSM<StateType, StateObject> : MonoBehaviour where StateObject : BaseState
{
    protected Dictionary<StateType, StateObject> stateDictionary = new Dictionary<StateType, StateObject>();

    public StateType CurrentState { get; protected set; }
    public abstract void ChangeState(StateType stateType);

    public abstract void InitStates();

    private void Update()
    {
        if (stateDictionary != null)
        {
            // Update the current state
            if (stateDictionary.ContainsKey(CurrentState))
            {
                stateDictionary[CurrentState].Update();
            }
        }
    }

}
