using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using Debug = UnityEngine.Debug;
using System;
using Cysharp.Threading.Tasks;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace WiDiD.SceneManagement
{
    /// <summary>
    /// This component allows copying of all renderers/lods group in <see cref="sceneReference"/> and instantiate them in current scene.
    /// The final purpose is to split a massive scene into smaller ones in order to reduce the weight of each scene, to quicken editors save/load manipulation and performances.
    /// </summary>
    public class SceneImpostor : MonoBehaviour, IAsyncLoadable
    {
        [SerializeField] private SceneReference sceneReference = null;
        [SerializeField] private bool unloadSubScenes = false;
        [Header("PLAYMODE ONLY")]
        [SerializeField] private bool _debugLoadScene = false;
        [SerializeField] private bool _debugUnloadScene = false;
        [SerializeField] public UnityEvent OnSceneLoaded = null;
        [SerializeField] public UnityEvent OnSceneUnloaded = null;
        [Header("EDITOR ONLY")]
        [SerializeField] private Transform impostorParent = null;
        [SerializeField] private bool removeSceneWhenOver = false;
        [SerializeField] private bool _resetImpostor;
        [SerializeField] private bool _instantiate;
        [SerializeField] private bool _cancelInstantiation = false;

        private AsyncOperation loadingOperation = null;
        private Action<AsyncOperation> OnLoadingOperationStarted = null;

        public AsyncOperation AsyncOperation => loadingOperation;
        public Action<AsyncOperation> OnAsyncLoadingStarted { get => OnLoadingOperationStarted; set => OnLoadingOperationStarted = value; }

        public bool IsSceneLoaded
        {
            get
            {
                TryGetScene(out Scene scene);
                return scene.isLoaded;
            }
        }

        public SceneReference SceneReference { get => sceneReference; }

        #region Public Methods
        public void LoadScene()
        {
            if (IsSceneLoaded == false && loadingOperation == null)
            {
                loadingOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneReference.ScenePath, LoadSceneMode.Additive);
                loadingOperation.completed += OnSceneLoadingOperationEndedCallback;
                OnAsyncLoadingStarted?.Invoke(loadingOperation);
            }
            else if (IsSceneLoaded)
            {
                OnSceneLoadingOperationEndedCallback(null);
            }
        }
        public async UniTask LoadSceneAsync()
        {
            if (IsSceneLoaded == false)
            {
                await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneReference.ScenePath, LoadSceneMode.Additive).ToUniTask();
            }
            OnSceneLoadingOperationEndedCallback(null);
        }
        public void UnloadScene()
        {
            if (IsSceneLoaded && loadingOperation == null)
            {
                if (unloadSubScenes)
                {
                    SceneImpostor[] subSceneImpostors = GetSubSceneImpostors();
                    for (int i = 0; i < subSceneImpostors.Length; i++)
                    {
                        subSceneImpostors[i].UnloadScene();
                    }
                }
                loadingOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneReference.ScenePath, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                loadingOperation.completed += OnSceneUnloadingOperationEndedCallback;
            }
        }
        public async UniTask UnloadSceneAsync()
        {
            if (!IsSceneLoaded) return;

            if (unloadSubScenes)
            {
                SceneImpostor[] subSceneImpostors = GetSubSceneImpostors();
                for (int i = 0; i < subSceneImpostors.Length; i++)
                {
                    await subSceneImpostors[i].UnloadSceneAsync();
                }
            }

            await UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneReference.ScenePath, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects).ToUniTask();
            OnSceneUnloadingOperationEndedCallback(null);
        }
        public void RestartScene()
        {
            if (IsSceneLoaded && loadingOperation == null)
            {
                if (unloadSubScenes)
                {
                    SceneImpostor[] subSceneImpostors = GetSubSceneImpostors();
                    for (int i = 0; i < subSceneImpostors.Length; i++)
                    {
                        subSceneImpostors[i].UnloadScene();
                    }
                }
                loadingOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneReference.ScenePath, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                loadingOperation.completed += OnSceneRestartingOperationEndedCallback;
            }
        }

        public void ResetImpostor()
        {
            if (impostorParent != null)
            {
                DestroyImmediate(impostorParent.gameObject);
            }
            impostorParent = new GameObject("ImpostorParent").transform;
            impostorParent.parent = this.transform;
        }


        #endregion
        #region Private Methods
        private void OnSceneLoadingOperationEndedCallback(AsyncOperation loadingOperation)
        {
            SceneEventSender sceneEventSender = GetSceneEventSender(UnityEngine.SceneManagement.SceneManager.GetSceneByPath(sceneReference.ScenePath));
            sceneEventSender.RaiseStart(this);

            impostorParent.gameObject.SetActive(false);
            if (loadingOperation != null)
                loadingOperation.completed -= OnSceneLoadingOperationEndedCallback;
            this.loadingOperation = null;

            OnSceneLoaded?.Invoke();
        }
        private void OnSceneUnloadingOperationEndedCallback(AsyncOperation loadingOperation)
        {
            impostorParent.gameObject.SetActive(true);
            if (loadingOperation != null)
            {
                loadingOperation.completed -= OnSceneUnloadingOperationEndedCallback;
                this.loadingOperation = null;
            }
            OnSceneUnloaded?.Invoke();
        }
        private void OnSceneRestartingOperationEndedCallback(AsyncOperation loadingOperation)
        {
            loadingOperation.completed -= OnSceneRestartingOperationEndedCallback;
            this.loadingOperation = null;
            LoadScene();
        }
        private SceneEventSender GetSceneEventSender(Scene scene)
        {
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            SceneEventSender eventSender;
            for (int i = 0; i < rootGameObjects.Length; i++)
            {
                if (rootGameObjects[i].TryGetComponent<SceneEventSender>(out eventSender))
                {
                    return eventSender;
                }
            }
            Debug.LogError("There is no SceneEventSender in your root gameObjects of Scene " + scene.name + "\n Make sure this scene have a SceneEventSender component on one of your root GameObject");
            return null;
        }
        private SceneImpostor[] GetSubSceneImpostors()
        {
            if (!IsSceneLoaded)
            {
                Debug.LogError("You cannot get subscene impostors from an inactive scene");
                return null;
            }

            if (!TryGetScene(out Scene scene))
            {
                Debug.LogError("Scene is invalid");
                return null;
            }

            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            List<SceneImpostor> subSceneImpostors = new List<SceneImpostor>();
            for (int i = 0; i < rootGameObjects.Length; i++)
            {
                subSceneImpostors.AddRange(rootGameObjects[i].GetComponentsInChildren<SceneImpostor>());
            }
            return subSceneImpostors.ToArray();
        }
        private bool TryGetScene(out Scene scene)
        {
            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
            scene = new Scene();
            for (int i = 0; i < sceneCount; i++)
            {
                if (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).path == sceneReference.ScenePath)
                {
                    scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region UNITY_EDITOR
#if UNITY_EDITOR
        public async void InstantiateImpostor()
        {
            ResetImpostor();

            Scene scene = EditorSceneManager.OpenScene(sceneReference.ScenePath, OpenSceneMode.Additive);
            while (scene.isLoaded == false)
            {
                await UniTask.Yield();
            }

            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            GameObject[] sortedGameObjects = null;

            for (int i = 0; i < rootGameObjects.Length; i++)
            {
                UniTask<GameObject[]> sortingTask = AsyncRecursiveSortGameObjectsByType(rootGameObjects[i].GetComponentsInChildren<Transform>());
                sortedGameObjects = await sortingTask;
                if (sortedGameObjects == null)
                {
                    Debug.Log("Sorting process canceled");
                    return;
                }
                await CloneSortedGameObjects(sortedGameObjects);
            }

            EditorSceneManager.MarkSceneDirty(gameObject.scene);
            if (removeSceneWhenOver)
                EditorSceneManager.CloseScene(scene, removeSceneWhenOver);
        }

        private async UniTask CloneSortedGameObjects(GameObject[] sortedGameObjects)
        {
            for (int i = 0; i < sortedGameObjects.Length; i++)
            {
                GameObject impostorGameObject = Instantiate(sortedGameObjects[i], impostorParent, true);
                if (i % 100 == 0)
                {
                    Debug.Log("Cloning progress : " + Mathf.CeilToInt(((float)i / sortedGameObjects.Length) * 100) + "%", this);
                    await UniTask.Yield();
                }
            }
        }
        private async UniTask<GameObject[]> AsyncRecursiveSortGameObjectsByType(Transform[] goTransforms)
        {
            if (goTransforms == null || goTransforms.Length == 0)
            {
                return new GameObject[0];
            }

            List<GameObject> sortedGameObjects = new List<GameObject>();
            List<Renderer> includedRenderers = new List<Renderer>();

            for (int i = 0; i < goTransforms.Length; i++)
            {
                GameObject sortedGameObject = null;
                Renderer renderer;
                LODGroup lodGroup;
                if (i % 1000 == 0)
                {
                    Debug.Log("Sorting progress : " + Mathf.CeilToInt(((float)i / goTransforms.Length) * 100) + "%", this);
                    await UniTask.Yield();
                }

                //Mesh renderers inclusion/exclusion
                if (goTransforms[i].TryGetComponent(out renderer))
                {
                    if (renderer.enabled && renderer.gameObject.activeSelf)
                    {
                        if (!sortedGameObjects.Contains(renderer.gameObject) && !includedRenderers.Contains(renderer))
                            sortedGameObject = renderer.gameObject;
                    }
                }
                //LOD Group inclusion/exclusion
                else if (goTransforms[i].TryGetComponent(out lodGroup))
                {
                    if (IsLodGroupDisplayed(lodGroup))
                    {
                        sortedGameObject = lodGroup.gameObject;
                    }
                }

                if (sortedGameObject == null)
                {
                    continue;
                }

                includedRenderers.AddRange(sortedGameObject.GetComponentsInChildren<Renderer>());
                sortedGameObjects.Add(sortedGameObject);
            }
            return sortedGameObjects.ToArray();
        }

        /// <summary>
        /// Call this to verify inclusion compatibility of a LODGroup to the scene impostor.
        /// A LODGroup is added if it's active and has atleast one active meshRenderer in its LOD
        /// </summary>
        /// <param name="lodGroup">Lod group to check inclusion</param>
        /// <param name="lodCount">Out int value of the number of linked lodCount</param>
        /// <returns>Returns true if LODGroup must be included, else if not included</returns>
        private bool IsLodGroupDisplayed(LODGroup lodGroup)
        {
            bool include = false;
            foreach (LOD lod in lodGroup.GetLODs())
            {
                foreach (Renderer renderer in lod.renderers)
                {
                    if (renderer == null) continue;
                    if (renderer.enabled && renderer.gameObject.activeInHierarchy)
                    {
                        include = true;
                    }
                }
            }
            return include;
        }
        private void Reset()
        {
            ResetImpostor();
        }
#endif
        #endregion
    }
}
