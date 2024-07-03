using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindVisuals : MonoBehaviour
{
    private ParticleSystem[] VFXSystem;
    [SerializeField] private WindSource windSource;
    [SerializeField] private float lifeTimeConst = 50f;
    [SerializeField] private float lifeTimeRatio = 10f;
    [SerializeField] private float curveFactor = 5f;

    private void Start()
    {
        if (!windSource) windSource = FindFirstObjectByType<WindSource>();
        VFXSystem = GetComponentsInChildren<ParticleSystem>();
        windSource.windDirectionChanged += SetWindVisualDirection;
        SetWindVisualDirection(windSource.WindDirection);
    }

    private void SetWindVisualDirection(Vector3 direction)
    {
        if (direction.magnitude <= 1)
        {
            foreach (ParticleSystem vfx in VFXSystem)
            {
                var main = vfx.main;
                main.startLifetime = 0f;
            }
            return;
        }
        foreach (ParticleSystem vfx in VFXSystem)
        {
            var main = vfx.main;
            var vel = vfx.velocityOverLifetime;
            main.startLifetime = (lifeTimeConst - direction.magnitude) / lifeTimeRatio;
            main.startSpeed = direction.magnitude;
            if (vel.enabled)
            {
                var yCurve = vel.y;
                yCurve.curveMultiplier = direction.magnitude * curveFactor;
                vel.y = yCurve;
                var zCurve = vel.z;
                zCurve.curveMultiplier = direction.magnitude * curveFactor;
                vel.z = zCurve;
            }
        }
        transform.LookAt(transform.position + direction);
    }

    private void OnDisable()
    {
        windSource.windDirectionChanged -= SetWindVisualDirection;
    }
}
