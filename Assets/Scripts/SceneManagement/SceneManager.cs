using UnityEngine;


namespace WiDiD.SceneManagement
{
	public interface ISceneManager
	{
#if UNITY_EDITOR
		void UpdateSceneList();
#endif
	}

	[DisallowMultipleComponent]
	public class SceneManager : Singleton<SceneManager>
	{
		[SerializeField]
		private bool m_ShowDebugLog = true;
		[SerializeField]
		GameObject m_LoadingScreenCanvas;

		// There are errors while trying to activate a scene when multiple scenes are loading, we wait for the last scene to load before setting the active scene
		int m_ScenesCurrentlyLoading = 0;
		int m_ScenesCurrentlyUnloading = 0;

		public void LoadSceneSet(SceneSet set, bool safeLoad = true, System.Action OnSetLoaded = null)
		{
			if (m_ShowDebugLog) Debug.Log($"Loading {set.Scenes.Count} scenes...");

			System.Action<AsyncOperation> callback = null;
			if (OnSetLoaded != null)
			{
				m_ScenesCurrentlyLoading = set.Scenes.Count;
				callback = (ao) => SetSceneActiveCallback(set.ActiveScene, OnSetLoaded);
			}

			foreach (var scene in set.Scenes)
			{
				LoadScene(scene, safeLoad, callback);
			}
		}

		public void UnloadSceneSet(SceneSet set, bool safeUnload = true, bool destroyAllObjects = false, System.Action OnSetUnloaded = null)
		{
			if (m_ShowDebugLog) Debug.Log($"Unloading {set.Scenes.Count} scenes...");

			System.Action<AsyncOperation> callback = null;
			if (OnSetUnloaded != null)
			{
				m_ScenesCurrentlyUnloading = set.Scenes.Count;
				callback = (ao) => UnloadCallback(OnSetUnloaded);
			}

			foreach (var scene in set.Scenes)
			{
				UnloadScene(scene, safeUnload, destroyAllObjects, callback);
			}
		}

		/// <summary>
		/// Unload the given scene
		/// </summary>
		/// <param name="sceneName"></param>
		/// <param name="destroyAllObjects">Set true to enable UnloadAllEmbeddedSceneObjects option <seealso cref="UnityEngine.SceneManagement.UnloadSceneOptions.UnloadAllEmbeddedSceneObjects"/></param>
		public void UnloadScene(string sceneName, bool safeUnload = true, bool destroyAllObjects = false, System.Action<AsyncOperation> onCompleted = null)
		{
			if (safeUnload)
			{
				if (!IsSceneLoaded(sceneName))
				{
					if (onCompleted != null)
					{
						m_ScenesCurrentlyUnloading--;
						if (m_ShowDebugLog) Debug.Log($"{sceneName} is not loaded, so it's ignored. {m_ScenesCurrentlyUnloading} remaining scenes to unload.");
					}
					else
					{
						if (m_ShowDebugLog) Debug.Log($"{sceneName} is not loaded, so it's ignored.");
					}

					return;
				}
			}

			if (m_ShowDebugLog) Debug.Log($"Unloading {sceneName}... (safeUnload is " + safeUnload + ")");

			var asyncOp = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName, destroyAllObjects ? UnityEngine.SceneManagement.UnloadSceneOptions.UnloadAllEmbeddedSceneObjects : UnityEngine.SceneManagement.UnloadSceneOptions.None);
			if (onCompleted != null)
				asyncOp.completed += onCompleted;
		}

		/// <summary>
		/// Load a scene
		/// </summary>
		/// <param name="sceneName">Scene name</param>
		/// <param name="safeLoad">Check if scene is already loaded to avoid double</param>
		protected void LoadScene(string sceneName, bool safeLoad = true, System.Action<AsyncOperation> onCompleted = null)
		{
			if (safeLoad)
			{
				if (IsSceneLoaded(sceneName))
				{
					if (onCompleted != null)
					{
						onCompleted?.Invoke(null);
						if (m_ShowDebugLog) Debug.Log($"{sceneName} is already loaded, so it's ignored. {m_ScenesCurrentlyLoading} remaining scenes to load.");
					}
					else
					{
						if (m_ShowDebugLog) Debug.Log($"{sceneName} is already loaded, so it's ignored.");
					}

					return;
				}
			}
			// Protect against warning for scenes not included in build
#if !UNITY_EDITOR
			if (Application.CanStreamedLevelBeLoaded(sceneName))
#endif
			{
				if (m_ShowDebugLog) Debug.Log($"Loading {sceneName}... (safeLoad is " + safeLoad + ")");

				var asyncOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
				if (onCompleted != null)
					asyncOp.completed += onCompleted;
			}
		}



		private void SetSceneActive(SceneReference activeScene)
		{
			int count = UnityEngine.SceneManagement.SceneManager.sceneCount;
			for (int i = 0; i < count; i++)
			{
				var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

				if (scene.path.Equals(activeScene.ScenePath))
				{
					// Set the first found scene as active
					UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);

					// Exit
					return;
				}
			}
		}

		private void SetSceneActiveCallback(SceneReference sceneActive, System.Action OnSetLoaded = null)
		{
			m_ScenesCurrentlyLoading--;
			if (m_ShowDebugLog) Debug.Log($"{m_ScenesCurrentlyLoading} remaining scenes");

			if (m_ScenesCurrentlyLoading == 0)
			{
				if (m_ShowDebugLog) Debug.Log("The last scene of the bunch was loaded");
				if (m_ShowDebugLog && sceneActive != null) Debug.Log("Now setting " + sceneActive + " scene as active ");
				this.ExecuteAtNextFrame(() =>
				{
					if (sceneActive != null)
						SetSceneActive(sceneActive);
					LightProbes.TetrahedralizeAsync();
					// Hide VR loading
					if (m_LoadingScreenCanvas != null)
						m_LoadingScreenCanvas.SetActive(false);
					OnSetLoaded?.Invoke();
				});
			}
		}
		private void UnloadCallback(System.Action OnSetUnloaded = null)
		{
			m_ScenesCurrentlyUnloading--;
			if (m_ShowDebugLog) Debug.Log($"{m_ScenesCurrentlyUnloading} remaining scenes");

			if (m_ScenesCurrentlyUnloading == 0)
			{
				if (m_ShowDebugLog) Debug.Log("The last scene of the bunch was unloaded");
				this.ExecuteAtNextFrame(() =>
				{
					OnSetUnloaded?.Invoke();
				});
			}
		}

		/// <summary>
		/// Check if the scene is already loaded
		/// </summary>
		/// <returns>True if the scene is loaded</returns>
		private bool IsSceneLoaded(string pSceneName)
		{
			int count = UnityEngine.SceneManagement.SceneManager.sceneCount;
			for (int i = 0; i < count; i++)
			{
				var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
				if (scene.name.Equals(pSceneName) || scene.path.Equals(pSceneName))
					return true;
			}

			return false;
		}


	}
}