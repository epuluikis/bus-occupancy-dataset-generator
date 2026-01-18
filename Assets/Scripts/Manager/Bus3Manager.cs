using UnityEngine;

public class Bus3Manager : BusManagerBase
{
    private Renderer _emissiveRenderer;
    private Renderer _seatRenderer;
    private Renderer _floorRenderer;
    private Renderer _ceilingRenderer;
    private Renderer _holderRenderer;

    public void Start()
    {
        foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
        {
            if (r.gameObject.name == "Object_31")
            {
                _emissiveRenderer = r;
            }

            if (r.gameObject.name == "Object_8")
            {
                _seatRenderer = r;
            }

            if (r.gameObject.name == "Object_29")
            {
                _floorRenderer = r;
            }

            if (r.gameObject.name == "Object_19")
            {
                _ceilingRenderer = r;
            }

            if (r.gameObject.name == "Object_22")
            {
                _holderRenderer = r;
            }
        }
    }

    public override void SwitchLights(bool on)
    {
        foreach (var l in gameObject.GetComponentsInChildren<Light>())
        {
            l.enabled = on;
        }

        if (on)
        {
            _emissiveRenderer.material.SetColor("emissiveFactor", new Color(6135.966f, 6135.966f, 6135.966f, 0));
        }
        else
        {
            _emissiveRenderer.material.SetColor("emissiveFactor", new Color(500, 500, 500, 0));
        }
    }

    public override void Randomize()
    {
        _seatRenderer.material.SetColor("baseColorFactor", seatColors[Random.Range(0, seatColors.Length)]);

        var floorColor = Random.Range(0.5f, 1f);
        _floorRenderer.material.SetColor("baseColorFactor", new Color(floorColor, floorColor, floorColor, 1f));

        var ceilingColor = Random.Range(0.5f, 1f);
        _ceilingRenderer.material.SetColor("baseColorFactor", new Color(ceilingColor, ceilingColor, ceilingColor, 1f));

        _holderRenderer.material.SetColor("baseColorFactor", seatColors[Random.Range(0, seatColors.Length)]);
    }
}
