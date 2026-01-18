using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class RandomManager : MonoBehaviour
{
    [Header("Camera")] public Camera cam;

    [Header("Post Processing")] public Volume volume;
    public Texture2D[] blooms;

    private BusManager _busManager;
    private LensDistortion _lens;
    private Vignette _vignette;
    private Exposure _exposure;
    private Bloom _bloom;
    private FilmGrain _filmGrain;

    public void Start()
    {
        _busManager = GetComponent<BusManager>();
        volume.profile.TryGet(out _lens);
        volume.profile.TryGet(out _vignette);
        volume.profile.TryGet(out _exposure);
        volume.profile.TryGet(out _bloom);
        volume.profile.TryGet(out _filmGrain);
    }

    public void Randomize()
    {
        var bus = _busManager.GetActiveBus();

        bus.Randomize();
        RandomizeCamera(bus);
        RandomizeVolume();
    }

    private void RandomizeCamera(IManagableBus bus)
    {
        var marker = GetRandomCameraMarker(bus);

        var localOffset = new Vector3(
            Random.Range(-marker.size.x * 0.5f, marker.size.x * 0.5f),
            Random.Range(-marker.size.y * 0.5f, marker.size.y * 0.5f),
            Random.Range(-marker.size.z * 0.5f, marker.size.z * 0.5f)
        );

        cam.transform.position = marker.transform.TransformPoint(Vector3.zero + localOffset);
        cam.transform.rotation = marker.transform.rotation;
        cam.transform.rotation *= Quaternion.Euler(
            Random.Range(5f, 30f),
            Random.Range(-15f, 15f),
            Random.Range(-5f, 5f)
        );
        cam.fieldOfView = Random.Range(60, 100);
    }

    private CameraMarker GetRandomCameraMarker(IManagableBus bus)
    {
        var markers = bus.GetCameraMarkers();
        var totalWeight = markers.Sum(m => m.weight);

        var r = Random.Range(0f, totalWeight);
        var cumulative = 0f;

        foreach (var m in markers)
        {
            cumulative += m.weight;

            if (r <= cumulative)
            {
                return m;
            }
        }

        return markers[Random.Range(0, markers.Length)];
    }

    private void RandomizeVolume()
    {
        _lens.intensity.value = Random.Range(-0.25f, 0.5f);

        _vignette.intensity.value = Random.Range(0.05f, 0.35f);
        _vignette.smoothness.value = Random.Range(0.3f, 0.7f);

        _exposure.compensation.value = Random.Range(-0.5f, 0.5f);

        _bloom.intensity.value = Random.Range(0f, 0.35f);
        _bloom.dirtTexture.value = blooms[Random.Range(0, blooms.Length)];
        _bloom.dirtIntensity.value = Random.Range(0f, 3f);

        _filmGrain.intensity.value = Random.Range(0f, 0.5f);
    }
}
