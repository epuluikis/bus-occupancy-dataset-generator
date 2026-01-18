using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
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

        foreach (var marker in _bus.GetFloorMarkers())
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
        var prefab = personManager.GetRandomStandingPerson();
        var instance = Instantiate(prefab);

        var offsetY = GetPersonYOffset(prefab);
        var instancePosition = position.transform.position;
        instancePosition.y += offsetY;

        instance.transform.SetParent(spawnRoot, true);
        instance.transform.position = instancePosition;
        instance.transform.rotation = GetRandomRotation(prefab);
        instance.name = $"SpawnedStandingPerson{index}";
        instance.SetActive(true);

        _people.Add(instance);
    }

    private void ClearPeople()
    {
        foreach (var p in _people)
        {
            p.SetActive(false);
            Destroy(p);
        }

        _people.Clear();
    }

    private float GetPersonYOffset(GameObject person)
    {
        var lowest = float.PositiveInfinity;

        foreach (var meshFilter in person.GetComponentsInChildren<MeshFilter>())
        {
            var sharedMesh = meshFilter.sharedMesh;

            if (!sharedMesh)
            {
                continue;
            }

            foreach (var v in sharedMesh.vertices)
            {
                var world = meshFilter.transform.TransformPoint(v);

                if (world.y < lowest)
                    lowest = world.y;
            }
        }

        foreach (var skinnedMesh in person.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            var bakedMesh = new Mesh();
            skinnedMesh.BakeMesh(bakedMesh);

            foreach (var v in bakedMesh.vertices)
            {
                var world = skinnedMesh.transform.TransformPoint(v);

                if (world.y < lowest)
                {
                    lowest = world.y;
                }
            }
        }

        return person.transform.position.y - lowest;
    }

    private Quaternion GetRandomRotation(GameObject prefab)
    {
        var e = prefab.transform.rotation.eulerAngles;

        return Quaternion.Euler(
            e.x,
            Random.Range(0f, 360f),
            e.z
        );
    }
}
