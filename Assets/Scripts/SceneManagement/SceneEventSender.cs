using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Globalization;


namespace WiDiD.SceneManagement
{
    /// <summary>
    /// Works in pair with <see cref="SceneImpostor"/>. Must be held by loaded scene of <see cref="SceneImpostor"/>
    /// </summary>
    public class SceneEventSender : MonoBehaviour
    {
        private SceneImpostor linkedSceneImpostor = null;

        [SerializeField] private bool raiseOnEnable = false;
        [SerializeField] private float raiseDelay = 0.5f;
        [SerializeField] private UnityEvent OnStart = null;
        [SerializeField] private UnityEvent OnRestart = null;
        [SerializeField] private UnityEvent OnStop = null;

        private bool isStarted = false;
        public void RaiseStart(SceneImpostor sceneImpostor)
        {
            if (isStarted)
                return;
            linkedSceneImpostor = sceneImpostor;
            isStarted = true;
            StartCoroutine(RaiseStartBehaviour());
        }
        public void RaiseStop()
        {
            isStarted = false;
            OnStop?.Invoke();
            StartCoroutine(RaiseStopBehaviour());
        }
        public void RaiseRestart()
        {
            OnRestart?.Invoke();
            StartCoroutine(RaiseRestartBehaviour());
        }
        private void OnEnable()
        {
            if (raiseOnEnable)
                RaiseStart(null);
        }
        private IEnumerator RaiseStartBehaviour()
        {
            yield return new WaitForSeconds(raiseDelay);
            OnStart?.Invoke();
            yield break;
        }

        private IEnumerator RaiseRestartBehaviour()
        {
            yield return new WaitForSeconds(raiseDelay);
            linkedSceneImpostor.RestartScene();
            yield break;
        }

        private IEnumerator RaiseStopBehaviour()
        {
            yield return new WaitForSeconds(raiseDelay);
            linkedSceneImpostor.UnloadScene();
            yield break;
        }

    }
}