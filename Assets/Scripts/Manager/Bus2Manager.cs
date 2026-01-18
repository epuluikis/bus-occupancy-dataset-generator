using System.Collections.Generic;
using UnityEngine;

public class Bus2Manager : BusManagerBase
{
    private Renderer _emissiveRenderer;
    private readonly List<Renderer> _seatRenderers = new();
    private readonly List<Renderer> _interiorRenderers = new();
    private readonly List<Renderer> _holderRenderers = new();
    private Renderer _floorRenderer;

    public void Start()
    {
        foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
        {
            if (r.gameObject.name == "lampadas-internas_0")
            {
                _emissiveRenderer = r;
            }

            if (r.gameObject.name.StartsWith("bancos"))
            {
                _seatRenderers.Add(r);
            }

            if (r.gameObject.name.StartsWith("interior_"))
            {
                _interiorRenderers.Add(r);
            }

            if (r.gameObject.name.StartsWith("balaustres"))
            {
                _holderRenderers.Add(r);
            }

            if (r.gameObject.name == "piso_0")
            {
                _floorRenderer = r;
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
        var seatColor = seatColors[Random.Range(0, seatColors.Length)];

        foreach (var r in _seatRenderers)
        {
            r.material.SetColor("baseColorFactor", seatColor);
        }

        var interiorColor = Random.Range(0.5f, 1f);

        foreach (var r in _interiorRenderers)
        {
            r.material.SetColor("baseColorFactor", new Color(interiorColor, interiorColor, interiorColor, 1f));
        }

        var holderColor = seatColors[Random.Range(0, seatColors.Length)];

        foreach (var r in _holderRenderers)
        {
            r.material.SetColor("baseColorFactor", holderColor);
        }

        var floorColor = Random.Range(0.5f, 1f);

        _floorRenderer.material.SetColor("baseColorFactor", new Color(floorColor, floorColor, floorColor, 1f));
    }
}
