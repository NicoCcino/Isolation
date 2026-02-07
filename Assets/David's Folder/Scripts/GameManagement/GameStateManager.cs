using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using WiDiD.SceneManagement;
using UnityEngine;

public class GameStateManager : BaseFSM<EGameState, AGameState>
{
    public static GameStateManager Instance;
    [Header("States")]
    [SerializeField] private MainMenuState mainMenuState;
    [SerializeField] private PlayingState playingState;

    [Header("Debug")]
    [SerializeField] private EGameState startState;

    private void Start()
    {
        Instance = this;
        InitStates();
        ChangeState(startState);
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
        var scenesToUnload = stateDictionary[CurrentState].SceneSet.Scenes.Except(stateDictionary[newState].SceneSet.Scenes, new SceneComparer());
        foreach (var scene in scenesToUnload)
            SceneManager.Instance.UnloadScene(scene);

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
            stateDictionary = new Dictionary<EGameState, AGameState>
             {
                        { EGameState.Default, mainMenuState },
                        { EGameState.MainMenu, mainMenuState },
                        { EGameState.Playing, playingState },
             };
        }
    }
    [Button("Set Start State")]
    private void SetStartState()
    {
        ChangeState(startState);
    }
}
