using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class AirRing : MonoBehaviour
{
    private Collider _collider;
    private Animator _anim;
    [SerializeField] private WindSource windSource;
    [SerializeField] private BarrelSpawner barrelSpawner;
    public float delay = 1f;
    private float timer = 0f;
    private bool animating = false;
    public float minHeight = 5f;
    public float maxHeight = 20f;


    void Start()
    {
        _collider = GetComponent<Collider>();
        _anim = GetComponentInChildren<Animator>();
        if (!windSource) windSource = FindFirstObjectByType<WindSource>();
    }
    private void Update()
    {
        if (animating)
        {
            timer += Time.deltaTime;
            if (timer > delay)
            {
                timer = 0f;
                animating = false;
                Relocate(windSource.WindDirection);
            }
        }
    }

    private void Relocate(Vector3 windDirection)
    {
        Vector3 newDirection = new Vector3(-windDirection.x, windDirection.y, windDirection.z);


    }

    private void OnTriggerEnter(Collider other)
    {
        if (_anim) _anim.SetTrigger("Shrink");
        animating = true;
    }
}
