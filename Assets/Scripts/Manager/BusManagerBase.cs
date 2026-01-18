using System.Collections.Generic;
using UnityEngine;

public abstract class BusManagerBase : MonoBehaviour, IManagableBus
{
    public Color[] seatColors;

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public abstract void SwitchLights(bool on);

    public SeatMarker[] GetSeatsMarkers()
    {
        return gameObject.GetComponentsInChildren<SeatMarker>();
    }

    public FloorMarker[] GetFloorMarkers()
    {
        return gameObject.GetComponentsInChildren<FloorMarker>();
    }

    public CameraMarker[] GetCameraMarkers()
    {
        return gameObject.GetComponentsInChildren<CameraMarker>();
    }

    public abstract void Randomize();
}
