using UnityEngine;

public class OccupancyGenerator : MonoBehaviour
{
    public OccupancyCategory[] categories;

    public (float seat, float floor) Generate(OccupancyStatus occupancyStatus)
    {
        foreach (var category in categories)
        {
            if (category.status == occupancyStatus)
            {
                return category.Generate();
            }
        }

        throw new System.Exception($"No occupancy category for {occupancyStatus}");
    }
}
