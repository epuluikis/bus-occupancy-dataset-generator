using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SkyManager : MonoBehaviour
{
    public Volume sky;
    public Skybox[] skyboxes;

    private Skybox _currentSkybox;

    public Skybox Get(int index)
    {
        return skyboxes[index];
    }

    public void SetActive(Skybox skybox)
    {
        if (_currentSkybox == skybox)
        {
            return;
        }

        _currentSkybox = skybox;

        if (sky.sharedProfile.TryGet<HDRISky>(out var hdriSky))
        {
            hdriSky.hdriSky.value = skybox.cubemap;
            hdriSky.rotation.value = skybox.rotation;
        }
    }

    public void Randomize()
    {
        SetActive(skyboxes[Random.Range(0, skyboxes.Length)]);
    }
}
