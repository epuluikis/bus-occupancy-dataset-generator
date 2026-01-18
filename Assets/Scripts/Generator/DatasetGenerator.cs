using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering;

public class DatasetGenerator : MonoBehaviour
{
    public bool autostart;
    public bool exitOnFinish;
    public bool saveImages = true;
    public string imagesPath;
    public int imagesPerOccupancy;
    public Camera captureCamera;
    public int cameraHeight;
    public int cameraWidth;
    [Range(0f, 24f)] public float nightStart;
    [Range(0f, 24f)] public float nightEnd;
    public float nightPercentage;

    private PersonManager _personManager;
    private BusManager _busManager;
    private SeatManager _seatManager;
    private FloorManager _floorManager;
    private LightingManager _lightingManager;
    private OccupancyGenerator _occupancyGenerator;
    private SkyManager _skyManager;
    private RandomManager _randomManager;
    private readonly Dictionary<OccupancyStatus, int> _counts = new();
    private RenderTexture _rt;
    private Texture2D _tex;

    public IEnumerator Start()
    {
        _personManager = GetComponent<PersonManager>();
        _busManager = GetComponent<BusManager>();
        _seatManager = GetComponent<SeatManager>();
        _floorManager = GetComponent<FloorManager>();
        _lightingManager = GetComponent<LightingManager>();
        _occupancyGenerator = GetComponent<OccupancyGenerator>();
        _skyManager = GetComponent<SkyManager>();
        _randomManager = GetComponent<RandomManager>();

        _personManager.Load();
        _busManager.Load();

        if (autostart)
        {
            yield return GenerateRoutine();
        }
    }

    public void Generate()
    {
        StartCoroutine(GenerateRoutine());
    }

    private IEnumerator GenerateRoutine()
    {
        _counts.Clear();

        var outputPath = GetOutputPath();
        Directory.CreateDirectory(outputPath);

        var logPath = Path.Join(outputPath, "log.txt");

        void Log(string logString, string stackTrace, LogType type)
        {
            File.AppendAllText(logPath, $"[{type}] {logString}\n");
        }

        Application.logMessageReceived += Log;

        Debug.Log("[DatasetGenerator] Starting to generate...");

        var buses = _busManager.Get();
        var count = imagesPerOccupancy / buses.Count;

        for (var index = 0; index < buses.Count; index++)
        {
            var bus = buses[index];
            yield return GenerateForBus(bus, index, count, outputPath);
        }

        Debug.Log("[DatasetGenerator] Finished generating!");

        Application.logMessageReceived -= Log;

        if (exitOnFinish)
        {
            yield return new WaitForSecondsRealtime(2f);

            Application.Quit();
        }
    }

    private IEnumerator GenerateForBus(IManagableBus bus, int busIndex, int count, string outputPath)
    {
        Debug.Log($"[DatasetGenerator] Generating for bus: {bus}...");

        if (_busManager.SetActive(bus))
        {
            yield return new WaitForEndOfFrame();

            _seatManager.Load(bus);
            _floorManager.Load(bus);
            _lightingManager.Load(bus);
        }

        foreach (var status in Enum.GetValues(typeof(OccupancyStatus)))
        {
            var occupancyOutputPath = Path.Join(outputPath, status.ToString());
            Directory.CreateDirectory(occupancyOutputPath);

            yield return GenerateForOccupancy((OccupancyStatus)status,  busIndex, count, occupancyOutputPath);
        }
    }

    private IEnumerator GenerateForOccupancy(OccupancyStatus occupancyStatus, int busIndex, int count, string outputPath)
    {
        Debug.Log($"[DatasetGenerator] Generating for occupancy: {occupancyStatus}...");

        var stepSize = Mathf.Max(1, count / 20);

        for (var i = 0; i < count; i++)
        {
            if ((i + 1) % stepSize == 0 || i == count - 1)
            {
                var percent = (i + 1) * 100f / count;
                Debug.Log(
                    $"[DatasetGenerator] Generating for occupancy: {occupancyStatus}... ({i + 1}/{count}, {percent:0.#}%)");
            }

            _lightingManager.SetCurrentTime(GetRandomTime(i, count));
            _skyManager.Randomize();
            _randomManager.Randomize();

            var (seat, floor) = _occupancyGenerator.Generate(occupancyStatus);

            yield return new WaitForEndOfFrame();

            _seatManager.Spawn(seat);
            _floorManager.Spawn(floor);

            yield return CaptureImage(occupancyStatus, busIndex, outputPath);
        }
    }

    private IEnumerator CaptureImage(OccupancyStatus occupancyStatus, int busIndex, string outputPath)
    {
        InitBuffers();
        yield return WaitForHdrpFrame();
        yield return new WaitForEndOfFrame();

        var currentCount = _counts.GetValueOrDefault(occupancyStatus, 0);
        _counts[occupancyStatus] = ++currentCount;

        var filePath = Path.Join(outputPath, $"{currentCount:D5}_{busIndex}");

        var prevRT = captureCamera.targetTexture;
        captureCamera.targetTexture = _rt;

        captureCamera.Render();

        var prevActive = RenderTexture.active;
        RenderTexture.active = _rt;
        _tex.ReadPixels(new Rect(0, 0, cameraWidth, cameraHeight), 0, 0);
        _tex.Apply();
        RenderTexture.active = prevActive;

        if (saveImages)
        {
            var bytes = _tex.EncodeToJPG(95);
            File.WriteAllBytes(filePath + ".jpg", bytes);
        }

        captureCamera.targetTexture = prevRT;

        if (currentCount % 500 == 0)
        {
            _rt.Release();
            _rt.Create();
        }
    }

    private float GetRandomTime(int index, int count)
    {
        if (index / (float)count * 100f < nightPercentage)
        {
            if (nightStart <= nightEnd)
            {
                return UnityEngine.Random.Range(nightStart, nightEnd);
            }

            var firstSegmentLength = 24f - nightStart;
            var secondSegmentLength = nightEnd;
            var totalLength = firstSegmentLength + secondSegmentLength;

            var r = UnityEngine.Random.Range(0f, totalLength);

            if (r < firstSegmentLength)
            {
                return nightStart + r;
            }

            return r - firstSegmentLength;
        }

        if (nightStart <= nightEnd)
        {
            var firstSegmentLength = nightStart;
            var secondSegmentLength = 24f - nightEnd;
            var totalLength = firstSegmentLength + secondSegmentLength;

            var r = UnityEngine.Random.Range(0f, totalLength);

            if (r < firstSegmentLength)
            {
                return r;
            }

            return nightEnd + (r - firstSegmentLength);
        }

        return UnityEngine.Random.Range(nightEnd, nightStart);
    }

    private string GetOutputPath()
    {
        return Path.Combine(imagesPath, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    private IEnumerator WaitForHdrpFrame()
    {
        var done = false;

        void OnEndFrame(ScriptableRenderContext ctx, Camera[] cams)
        {
            done = true;

            RenderPipelineManager.endFrameRendering -= OnEndFrame;
        }

        RenderPipelineManager.endFrameRendering += OnEndFrame;

        while (!done)
            yield return null;
    }

    private void InitBuffers()
    {
        if (_rt == null)
            _rt = new RenderTexture(cameraWidth, cameraHeight, 24);

        if (_tex == null)
            _tex = new Texture2D(cameraWidth, cameraHeight, TextureFormat.RGB24, false);
    }
}
