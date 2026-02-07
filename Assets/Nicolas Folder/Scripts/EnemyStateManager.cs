using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class EnemyStateManager : BaseFSM<EEnemyState, AEnemyState>
{
    [SerializeField] public EnemyController enemyController;
    [SerializeField] public EnemyVision enemyVision;
    [SerializeField] public EnemyData enemyData;
    [SerializeField] public EnemyStateManager enemyStateManager;


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
                        { EEnemyState.Patrolling, new PatrollingState(enemyController,enemyVision,enemyData,enemyStateManager, patrollingState.waitTime)},
                        { EEnemyState.Warned, new WarnedState(enemyController,enemyVision,enemyData,enemyStateManager,warnedState.warnedTime ) },
                        { EEnemyState.Hunting, new HuntingState(enemyController,enemyVision,enemyData,enemyStateManager) },
                        { EEnemyState.Attacking, new AttackingState(enemyController,enemyVision,enemyData,enemyStateManager) }
             };
        }
    }
    [Button("Set Start State")]
    private void SetStartState()
    {
        ChangeState(startState);
    }
}
