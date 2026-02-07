using UnityEngine;

namespace WiDiD.SceneManagement
{
    /// <summary>
    /// This component must exist in decorative scenes that wants to force their skybox to the main scene
    /// </summary>
    public class SkyboxSetter : MonoBehaviour
    {
        [SerializeField] private Material skyboxMat = null;
        [SerializeField] private bool enableFog = false;

        private void Start()
        {
            RenderSettings.skybox = skyboxMat;
            RenderSettings.fog = enableFog;
            DynamicGI.UpdateEnvironment();
        }
    }
}
