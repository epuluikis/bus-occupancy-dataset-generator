using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class LightingManager : MonoBehaviour
{
    [Range(0f, 24f)] public float currentTime;

    [Header("Sun")] public Light sunLight;
    public float sunPosition;
    public float sunIntensity;
    public AnimationCurve sunIntensityCurve;
    public float sunTemperature;
    public AnimationCurve sunTemperatureCurve;

    [Header("Additional lights")] public Light[] additionalLights;
    public float additionalLightIntensity;
    public AnimationCurve additionalLightIntensityCurve;
    public float additionalLightTemperature;
    public AnimationCurve additionalLightTemperatureCurve;

    [Header("Sky")] public Volume sky;
    public float skyExposure;
    public AnimationCurve skyExposureCurve;
    public float skyExposureCorrection;
    public AnimationCurve skyExposureCorrectionCurve;

    [Header("Bus")] [Range(0f, 24f)] public float busTurnOnLightsTime;
    [Range(0f, 24f)] public float busTurnOffLightsTime;

    private IManagableBus _bus;

    public void Load(IManagableBus bus)
    {
        _bus = bus;
        UpdateBusLighting();
    }

    public void Start()
    {
        UpdateLighting();
    }

    public void OnValidate()
    {
        UpdateLighting();
    }

    public void SetCurrentTime(float time)
    {
        currentTime = time;

        UpdateLighting();
    }

    private void UpdateLighting()
    {
        UpdateSun();
        UpdateAdditionalLights();
        UpdateSky();
        UpdateBusLighting();
    }

    private void UpdateSun()
    {
        var normalizedTime = GetNormalizedTime();
        var rotation = GetNormalizedTime() * 360f;

        sunLight.transform.rotation = Quaternion.Euler(rotation - 90f, sunPosition, 0f);
        sunLight.GetComponent<HDAdditionalLightData>().intensity =
            Math.Max(0, sunIntensity * sunIntensityCurve.Evaluate(normalizedTime));
        sunLight.colorTemperature = sunTemperature * sunTemperatureCurve.Evaluate(normalizedTime);
    }

    private void UpdateAdditionalLights()
    {
        var normalizedTime = GetNormalizedTime();

        var intensity = Math.Max(
            0,
            additionalLightIntensity * additionalLightIntensityCurve.Evaluate(normalizedTime)
        );
        var temperature = additionalLightTemperature * additionalLightTemperatureCurve.Evaluate(normalizedTime);

        foreach (var additionalLight in additionalLights)
        {
            additionalLight.GetComponent<HDAdditionalLightData>().intensity = intensity;
            additionalLight.colorTemperature = temperature;
        }
    }

    private void UpdateSky()
    {
        var normalizedTime = GetNormalizedTime();

        if (sky.sharedProfile.TryGet<HDRISky>(out var hdriSky))
        {
            hdriSky.exposure.value = skyExposureCorrection * skyExposureCorrectionCurve.Evaluate(normalizedTime);
        }

        if (sky.sharedProfile.TryGet<Exposure>(out var exposure))
        {
            exposure.fixedExposure.value = skyExposure * skyExposureCurve.Evaluate(normalizedTime);
        }
    }

    private void UpdateBusLighting()
    {
        if (_bus == null)
        {
            return;
        }

        if (busTurnOnLightsTime < busTurnOffLightsTime)
        {
            _bus.SwitchLights(currentTime >= busTurnOnLightsTime && currentTime < busTurnOffLightsTime);
        }
        else
        {
            _bus.SwitchLights(currentTime >= busTurnOnLightsTime || currentTime < busTurnOffLightsTime);
        }
    }

    private float GetNormalizedTime()
    {
        return currentTime / 24f;
    }
}
