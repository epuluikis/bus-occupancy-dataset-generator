using System.Collections.Generic;
using UnityEngine;

public class BusManager : MonoBehaviour
{
    public Transform busesRoot;

    private readonly List<IManagableBus> _buses = new();
    private IManagableBus _currentBus;

    public void Load()
    {
        foreach (Transform bus in busesRoot)
        {
            bus.gameObject.SetActive(false);
            _buses.Add(bus.GetComponent<IManagableBus>());
        }
    }

    public List<IManagableBus> Get()
    {
        return _buses;
    }

    public IManagableBus Get(int index)
    {
        return _buses[index];
    }

    public IManagableBus GetActiveBus()
    {
        return _currentBus;
    }

    public bool SetActive(IManagableBus bus)
    {
        if (_currentBus == bus)
        {
            return false;
        }

        _currentBus?.SetActive(false);
        _currentBus = bus;
        _currentBus.SetActive(true);

        return true;
    }
}
