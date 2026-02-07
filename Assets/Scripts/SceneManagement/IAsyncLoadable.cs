using UnityEngine;
using System;

public interface IAsyncLoadable
{
    public AsyncOperation AsyncOperation { get; }
    public Action<AsyncOperation> OnAsyncLoadingStarted { get; set; }


}
