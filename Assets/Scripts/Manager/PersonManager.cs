using System.Collections.Generic;
using UnityEngine;

public class PersonManager : MonoBehaviour
{
    public Transform sittingPeopleRoot;
    public Transform standingPeopleRoot;

    private readonly List<GameObject> _sittingPeople = new();
    private readonly List<GameObject> _standingPeople = new();

    public void Load()
    {
        foreach (Transform person in sittingPeopleRoot)
        {
            _sittingPeople.Add(person.gameObject);
        }

        foreach (Transform person in standingPeopleRoot)
        {
            _standingPeople.Add(person.gameObject);
        }
    }

    public GameObject GetRandomSittingPerson()
    {
        return _sittingPeople[Random.Range(0, _sittingPeople.Count)];
    }

    public GameObject GetRandomStandingPerson()
    {
        return _standingPeople[Random.Range(0, _standingPeople.Count)];
    }
}
