
using UnityEngine;
using WiDiD.SceneManagement;
public abstract class AGameState : BaseState
{
    [SerializeField] private SceneSet sceneSet;

    public SceneSet SceneSet { get => sceneSet; }

    public override abstract void Enter();
    public override abstract void Exit();
    public override abstract void Update();
}
