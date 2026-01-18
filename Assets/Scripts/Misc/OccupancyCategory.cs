using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class OccupancyCategory
{
    public OccupancyStatus status;

    public float seatMinFillPercentage;
    public float seatMaxFillPercentage;
    public float seatNoise;

    public float floorMinFillPercentage;
    public float floorMaxFillPercentage;
    public float floorNoise;

    public (float seat, float floor) Generate()
    {
        var seat = Random.Range(seatMinFillPercentage, seatMaxFillPercentage);
        var floor = Random.Range(floorMinFillPercentage, floorMaxFillPercentage);

        seat += Random.Range(-seatNoise, seatNoise);
        floor += Random.Range(-floorNoise, floorNoise);

        seat = Mathf.Clamp(seat, 0f, 100f);
        floor = Mathf.Clamp(floor, 0f, 100f);

        return (seat, floor);
    }
}
