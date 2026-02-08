using System.Collections;
using UnityEngine;

public class LightShaderLerp : MonoBehaviour
{
    [Header("Target Components")]
    public Light targetLight;
    public Renderer targetRenderer;
    public string shaderParamName = "_Emission";

    [Header("Light Settings")]
    public float lightMin = 0f;
    public float lightMax = 10f; // Example: Lights often need high intensity

    [Header("Shader Settings")]
    public float shaderMin = 0f;
    public float shaderMax = 1f;

    [Header("Animation")]
    [Tooltip("Time in seconds to complete the transition")]
    public float transitionDuration = 1.0f;

    // Internal state: 0 = fully at Min, 1 = fully at Max
    private float _currentProgress = 0f;
    private Coroutine _activeCoroutine;

    // Shader optimization
    private int _propertyID;
    private MaterialPropertyBlock _propBlock;

    void Awake()
    {
        // 1. Setup Shader Property ID
        if (!string.IsNullOrEmpty(shaderParamName))
            _propertyID = Shader.PropertyToID(shaderParamName);

        _propBlock = new MaterialPropertyBlock();

        // 2. Initialize state based on current light intensity
        // We try to guess the current "progress" (0 to 1) based on where the light is currently.
        if (targetLight != null)
        {
            // Inverse Lerp calculates where 'intensity' sits between min and max (returns 0 to 1)
            _currentProgress = Mathf.InverseLerp(lightMin, lightMax, targetLight.intensity);
        }
    }

    /// <summary>
    /// Pass true to Lerp to Max values. Pass false to Lerp to Min values.
    /// </summary>
    public void SetState(bool turnOn)
    {
        float targetProgress = turnOn ? 1.0f : 0.0f;

        if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
        _activeCoroutine = StartCoroutine(LerpRoutine(targetProgress));
    }

    private IEnumerator LerpRoutine(float targetProgress)
    {
        float startProgress = _currentProgress;
        float timeElapsed = 0f;

        while (timeElapsed < transitionDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / transitionDuration;

            // Optional: SmoothStep makes the start and end of the animation softer
            // t = Mathf.SmoothStep(0f, 1f, t);

            // Interpolate the abstract "progress" value
            _currentProgress = Mathf.Lerp(startProgress, targetProgress, t);

            ApplyValues(_currentProgress);

            yield return null;
        }

        // Ensure we hit the exact target at the end
        _currentProgress = targetProgress;
        ApplyValues(_currentProgress);
        _activeCoroutine = null;
    }

    private void ApplyValues(float progress)
    {
        // 1. Apply to Light (Lerp between Light Min/Max)
        if (targetLight != null)
        {
            targetLight.intensity = Mathf.Lerp(lightMin, lightMax, progress);
        }

        // 2. Apply to Shader (Lerp between Shader Min/Max)
        if (targetRenderer != null)
        {
            targetRenderer.GetPropertyBlock(_propBlock);

            float currentShaderVal = Mathf.Lerp(shaderMin, shaderMax, progress);
            _propBlock.SetFloat(_propertyID, currentShaderVal);

            targetRenderer.SetPropertyBlock(_propBlock);
        }
    }
}