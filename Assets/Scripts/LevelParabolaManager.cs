using System;
using UnityEngine;

public class LevelParabolaManager : MonoBehaviour
{
    [SerializeField] private Material[] levelMaterial;
    
    [SerializeField, Range(-0.02f, 0.02f)]
    private float yThreshold = 0;
    
    private float xThreshold = 0f;
    private float targetXThreshold = 0f;
    private float transitionSpeed = 0.5f;
    
    private static readonly int SidewayStrength = Shader.PropertyToID("_Sideway_Strength");
    private static readonly int BackwardStrength = Shader.PropertyToID("_Backward_Strength");

    private void OnValidate()
    {
        UpdateMaterials();
    }

    private void Start()
    {
        xThreshold = 0f;
        targetXThreshold = 0f;
        UpdateMaterials();
    }

    private void Update()
    {
        // Smoothly transition to the target xThreshold
        if (!Mathf.Approximately(xThreshold, targetXThreshold))
        {
            xThreshold = Mathf.Lerp(xThreshold, targetXThreshold, Time.deltaTime * transitionSpeed);
            
            // If we're very close, snap to the target
            if (Mathf.Abs(xThreshold - targetXThreshold) < 0.001f)
            {
                xThreshold = targetXThreshold;
            }
            
            UpdateMaterials();
        }
    }

    // Called by GameManager to set a new target xThreshold
    public void SetTargetXThreshold(float target)
    {
        // Clamp to valid range
        targetXThreshold = Mathf.Clamp(target, -0.02f, 0.02f);
    }

    public float GetCurrentXThreshold()
    {
        return xThreshold;
    }

    public float GetTargetXThreshold()
    {
        return targetXThreshold;
    }

    private void UpdateMaterials()
    {
        foreach (var material in levelMaterial)
        {
            material.SetFloat(SidewayStrength, xThreshold);
            material.SetFloat(BackwardStrength, yThreshold);
        }
    }
}