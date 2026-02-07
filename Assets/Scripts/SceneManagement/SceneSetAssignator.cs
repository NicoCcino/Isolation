using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace WiDiD.SceneManagement
{
    /// <summary>
    /// OnEnable, if autoLoadScenes component doesn't have any scenes to load, will retrieve Scenes enabled in BuildSettings
    /// and try to find any scene named with suffixe "_Main" to retrieve its name and then load <see cref="SceneSet"/> containing the same name.
    /// </summary>
    [DefaultExecutionOrder(100)]
    public class SceneSetAssignator : MonoBehaviour
    {
        [SerializeField] private SceneSet[] allSceneSets = null;
        [SerializeField] private AutoLoadScenes autoLoadScenes = null;
        private void OnEnable()
        {
            if (autoLoadScenes == null || autoLoadScenes.ScenesToLoad != null)
                return;

            string moduleName = "";
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);

                string sceneName = scenePath.Split('/').LastOrDefault();
                if (sceneName.Contains("_Main"))
                {
                    moduleName = sceneName.Split('_')[0];
                }
            }

            if (moduleName == "")
                return;

            for (int i = 0; i < allSceneSets.Length; i++)
            {

                if (allSceneSets[i].name.ToLower().Contains(moduleName.ToLower()))
                {
                    autoLoadScenes.Load(allSceneSets[i]);
                    return;
                }
            }
        }
    }
}
