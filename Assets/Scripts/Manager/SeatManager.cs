using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SeatManager : MonoBehaviour
{
    public PersonManager personManager;
    public Transform spawnRoot;

    private IManagableBus _bus;
    private readonly List<Transform> _positions = new();
    private readonly List<GameObject> _people = new();

    public void Load(IManagableBus bus)
    {
        ClearPositions();
        _bus = bus;
        LoadPositions();
    }

    public void Spawn(float fillPercentage)
    {
        ClearPeople();
        ShufflePositions();

        var count = Mathf.FloorToInt(_positions.Count * fillPercentage / 100f);

        for (var i = 0; i < count; i++)
        {
            SpawnPerson(i, _positions[i]);
        }
    }

    public void Clear()
    {
        ClearPeople();
    }

    private void LoadPositions()
    {
        ClearPositions();

        foreach (var marker in _bus.GetSeatsMarkers())
        {
            _positions.Add(marker.transform);
        }
    }

    private void ClearPositions()
    {
        ClearPeople();
        _positions.Clear();
    }

    private void ShufflePositions()
    {
        for (var i = 0; i < _positions.Count; i++)
        {
            var j = Random.Range(i, _positions.Count);

            (_positions[i], _positions[j]) = (_positions[j], _positions[i]);
        }
    }

    private void SpawnPerson(int index, Transform position)
    {
        var person = personManager.GetRandomSittingPerson();
        var instance = Instantiate(person);
        var marker = instance.GetComponentInChildren<PersonMarker>();

        instance.transform.SetParent(spawnRoot, true);
        instance.transform.rotation = position.rotation;
        instance.transform.position += position.position - marker.transform.position;
        instance.SetActive(true);
        instance.name = $"SpawnedSittingPerson{index}";

        _people.Add(instance);
    }

    private void ClearPeople()
    {
        foreach (var person in _people)
        {
            person.SetActive(false);
            Destroy(person);
        }

        _people.Clear();
    }
}
