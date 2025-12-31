using System;
using UnityEngine;

public class LevelParabolaManager : MonoBehaviour
{
    [SerializeField] private Material[] levelMaterial;

    [SerializeField, Range(-0.02f, 0.02f)]
    private float xThreshold;
    [SerializeField, Range(-0.02f, 0.02f)]
    private float yThreshold = 0;

    private static readonly int SidewayStrength = Shader.PropertyToID("_Sideway_Strength");
    private static readonly int BackwardStrength = Shader.PropertyToID("_Backward_Strength");

    private void OnValidate()
    {
        foreach (var material in levelMaterial)
        {
            material.SetFloat(SidewayStrength,xThreshold);
            material.SetFloat(BackwardStrength,yThreshold);
        }
    }
}
