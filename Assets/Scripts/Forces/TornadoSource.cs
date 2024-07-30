using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TornadoSource : GravitySource
{
    [SerializeField] private float radius = 5f;
    [SerializeField] private float windStrength = 5f;
    [SerializeField] private bool clockwise = true;
    [SerializeField] private int segments = 20;
    [SerializeField] private float height = 20f;

    public override Vector3 GetGravity(Vector3 position)
    {
        float distance = new Vector3(transform.position.x - position.x, 0f, transform.position.z - position.z).magnitude;
        if (distance > radius) return new Vector3();

        Vector3 direction = new Vector3(transform.position.x - position.x, 0, transform.position.z - position.z);
        Vector3 wind = new Vector3(direction.z, 0, -direction.x).normalized * (clockwise? 1 : -1) * windStrength;

        return wind;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.green;

        float angleStep = 360f / segments;

        Vector3[] topCircle = new Vector3[segments];
        Vector3[] bottomCircle = new Vector3[segments];

        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            topCircle[i] = transform.position + (transform.up * height) + new Vector3(x, 0, z);
            bottomCircle[i] = transform.position + new Vector3(x, 0, z);
        }

        for (int i = 0; i < segments; i++)
        {
            int nextIndex = (i + 1) % segments;
            Handles.DrawLine(topCircle[i], topCircle[nextIndex]);
        }
        for (int i = 0; i < segments; i++)
        {
            int nextIndex = (i + 1) % segments;
            Handles.DrawLine(bottomCircle[i], bottomCircle[nextIndex]);
        }
        for (int i = 0; i < segments; i++)
        {
            int nextIndex = (i + 1) % segments;
            Handles.DrawLine(topCircle[i], bottomCircle[i]);
        }
    }
#endif
}