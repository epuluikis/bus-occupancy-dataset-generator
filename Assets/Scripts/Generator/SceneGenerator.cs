using System.Collections;
using UnityEngine;

public class SceneGenerator : MonoBehaviour
{
    public bool autostart;
    public int busIndex;
    public int skyIndex;
    public int seatFillPercentage = 50;
    public int floorFillPercentage = 10;
    public OccupancyStatus occupancyStatus = OccupancyStatus.ManySeatsAvailable;

    private PersonManager _personManager;
    private BusManager _busManager;
    private SeatManager _seatManager;
    private FloorManager _floorManager;
    private LightingManager _lightingManager;
    private OccupancyGenerator _occupancyGenerator;
    private SkyManager _skyManager;
    private RandomManager _randomManager;

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
            yield return SpawnByStatusRoutine();
        }
    }

    public void SpawnByPercentage()
    {
        StartCoroutine(SpawnByPercentageRoutine());
    }

    public void SpawnByStatus()
    {
        StartCoroutine(SpawnByStatusRoutine());
    }

    public void Clear()
    {
        _seatManager.Clear();
        _floorManager.Clear();
    }

    private IEnumerator SpawnByPercentageRoutine()
    {
        yield return SpawnRoutine(seatFillPercentage, floorFillPercentage);
    }

    private IEnumerator SpawnByStatusRoutine()
    {
        var (seat, floor) = _occupancyGenerator.Generate(occupancyStatus);

        yield return SpawnRoutine(seat, floor);
    }

    private IEnumerator SpawnRoutine(float seat, float floor)
    {
        var sky = _skyManager.Get(skyIndex);
        _skyManager.SetActive(sky);

        var bus = _busManager.Get(busIndex);

        if (_busManager.SetActive(bus))
        {
            yield return new WaitForEndOfFrame();

            _seatManager.Load(bus);
            _floorManager.Load(bus);
            _lightingManager.Load(bus);
        }

        yield return new WaitForEndOfFrame();

        _randomManager.Randomize();
        _seatManager.Spawn(seat);
        _floorManager.Spawn(floor);
    }
}
