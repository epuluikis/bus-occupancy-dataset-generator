using UnityEngine;

[CreateAssetMenu(menuName = "Lighting/LightingPreset")]
public class LightingPreset : ScriptableObject
{
    [Header("Sun")]
    public float sunRotationX;
    public float sunRotationY;
    public float sunIntensity;
    public float sunTemperature;

    [Header("Left light")]
    public float leftLightIntensity;
    public float leftLightTemperature;
    public float leftLightRange;

    [Header("Right light")]
    public float rightLightIntensity;
    public float rightLightTemperature;
    public float rightLightRange;

    [Header("Front light")]
    public float frontLightIntensity;
    public float frontLightTemperature;
    public float frontLightRange;
}
