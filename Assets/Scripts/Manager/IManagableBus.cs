using System.Collections.Generic;

public interface IManagableBus
{
    public void SetActive(bool active);

    public void SwitchLights(bool on);

    public SeatMarker[] GetSeatsMarkers();

    public FloorMarker[] GetFloorMarkers();

    public CameraMarker[] GetCameraMarkers();

    public void Randomize();
}
