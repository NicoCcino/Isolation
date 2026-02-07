using System;
using UnityEngine;
public class StartButton : ASimpleButton
{
    protected override void OnClickCallback()
    {
        GameStateManager.Instance.ChangeState(EGameState.Playing);
    }

}
