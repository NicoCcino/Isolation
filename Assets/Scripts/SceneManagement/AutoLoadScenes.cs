using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WiDiD.SceneManagement
{
	public class AutoLoadScenes : MonoBehaviour
	{
		[SerializeField] SceneSet _scenesToLoad;
		[SerializeField] SceneSet _allScenesSet;

		private Coroutine m_LoadSceneCoroutineRef;
		private SceneSet m_CurrentSceneSetToLoad;

		public SceneSet ScenesToLoad { get => _scenesToLoad; set => _scenesToLoad = value; }
		public Action OnScenesLoaded = null;

		private void Start()
		{
			if (_scenesToLoad != null)
			{
				Load(_scenesToLoad);
			}
		}

		public void Load(SceneSet pSceneSetToLoad)
		{
			var lListToUnLoad = new List<string>();
			foreach (var lScene in _allScenesSet.Scenes)
			{
				SceneReference sceneReference = pSceneSetToLoad.Scenes.FirstOrDefault(s => s.ScenePath == lScene.ScenePath);
				if (sceneReference != null) continue;
				lListToUnLoad.Add(lScene);
			}

			m_CurrentSceneSetToLoad = pSceneSetToLoad;

			TryLoadNewScene(pSceneSetToLoad, lListToUnLoad);
		}

		private void TryLoadNewScene(SceneSet pScenesToLoad, List<string> pScenesToUnload)
		{
			if (!WiDiD.SceneManagement.SceneManager.Instance)
			{
				Debug.LogError("SceneManager is missing");
				return;
			}

			if (m_LoadSceneCoroutineRef != null)
				StopCoroutine(m_LoadSceneCoroutineRef);
			//m_LoadSceneCoroutineRef = StartCoroutine(LoadScenesCoroutine(pScenesToLoad, pScenesToUnload));

			LoadScenes(pScenesToLoad, pScenesToUnload);
		}

		private void LoadScenes(SceneSet pScenesToLoad, List<string> pScenesToUnload)
		{
			if (pScenesToUnload != null)
			{
				foreach (var item in pScenesToUnload)
				{
					//WiDiD.SceneManagement.SceneManager.Instance.UnloadScene(item);
				}
			}
			if (pScenesToLoad != null)
			{
				WiDiD.SceneManagement.SceneManager.Instance.LoadSceneSet(pScenesToLoad, true, OnLoadingFinished);
			}
		}

		private void OnLoadingFinished()
		{
			m_CurrentSceneSetToLoad = null;
			OnScenesLoaded?.Invoke();
		}
	}
}