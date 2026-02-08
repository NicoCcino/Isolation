using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using WiDiD.SceneManagement;
using UnityEngine;

public class GameStateManager : BaseFSM<EGameState, AGameState>
{
    public static GameStateManager Instance;
    [SerializeField] private SceneSet coreSet;
    [Header("States")]
    [SerializeField] private MainMenuState mainMenuState;
    [SerializeField] private PlayingState playingState;
    [SerializeField] private GameOverState gameoverState;
    [SerializeField] private VictoryState victoryState;

    [Header("Debug")]
    [SerializeField] private EGameState startState;

    private void Start()
    {
        Instance = this;
        InitStates();
        SceneManager.Instance.LoadSceneSet(coreSet, true, OnCoreSetLoaded);
    }

    public override void ChangeState(EGameState newState)
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

        SceneManager.Instance.LoadSceneSet(stateDictionary[newState].SceneSet);
        SceneManager.Instance.UnloadSceneSet(stateDictionary[CurrentState].SceneSet);
        CurrentState = newState;
        // Enter the new state
        if (stateDictionary.ContainsKey(CurrentState))
        {
            stateDictionary[CurrentState].Enter();
        }
    }

    public override void InitStates()
    {
        if (stateDictionary.Count == 0)
        {
            // Initialize the state dictionary
            stateDictionary = new Dictionary<EGameState, AGameState>
             {
                        { EGameState.Default, mainMenuState },
                        { EGameState.MainMenu, mainMenuState },
                        { EGameState.Playing, playingState },
                        {EGameState.GameOver,gameoverState},
                        {EGameState.Victory, victoryState}
             }
        ;
        }
    }
    public void SetCoreSceneSet(SceneSet sceneSet)
    {
        coreSet = sceneSet;
    }
    private void OnCoreSetLoaded()
    {
        ChangeState(startState);
    }
}
