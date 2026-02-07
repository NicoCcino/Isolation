using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using WiDiD.SceneManagement;
using UnityEngine;

public class EnemyStateManager : BaseFSM<EEnemyState, AEnemyState>
{

    [Header("States")]
    [SerializeField] private PatrollingState patrollingState;
    [SerializeField] private WarnedState warnedState;
    [SerializeField] private HuntingState huntingState;
    [SerializeField] private AttackingState attackingState;

    [Header("Debug")]
    [SerializeField] private EEnemyState startState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        InitStates();
        ChangeState(startState);
    }

    public override void ChangeState(EEnemyState newState)
    {
        // Exit the current state (if any)
        if (stateDictionary == null || newState == CurrentState)
        {
            return;
        }
        if (stateDictionary.ContainsKey(CurrentState))
        {
            stateDictionary[CurrentState].Exit();
        }

        // Update the current state
        CurrentState = newState;

        // Enter the new state
        if (stateDictionary.ContainsKey(newState))
        {
            stateDictionary[newState].Enter();
        }
    }

    public override void InitStates()
    {
        if (stateDictionary.Count == 0)
        {
            // Initialize the state dictionary
            stateDictionary = new Dictionary<EEnemyState, AEnemyState>
             {
                        { EEnemyState.Patrolling, patrollingState },
                        { EEnemyState.Warned, warnedState },
                        { EEnemyState.Hunting, huntingState },
                        { EEnemyState.Attacking, attackingState }
             };
        }
    }
    [Button("Set Start State")]
    private void SetStartState()
    {
        ChangeState(startState);
    }
}
