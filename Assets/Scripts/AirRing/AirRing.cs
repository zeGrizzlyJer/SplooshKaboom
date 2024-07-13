using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class AirRing : MonoBehaviour, IRequireCleanup
{
    private Collider _collider;
    private Animator _anim;
    [SerializeField] private WindSource windSource;
    [SerializeField] private BarrelSpawner barrelSpawner;
    public float delay = 1.5f;
    private float timer = 0f;
    private bool animating = false;
    public float minHeight = 5f;
    public float maxHeight = 20f;


    void Start()
    {
        _collider = GetComponent<Collider>();
        _anim = GetComponentInChildren<Animator>();
        if (!windSource) windSource = FindFirstObjectByType<WindSource>();
        GameManager.Instance.OnRoundStart += OnRoundStart;
        GameManager.Instance.OnApplicationCleanup += OnCleanup;
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
                Relocate();
            }
        }
    }

    private void Relocate()
    {
        Vector3 spawnDir = barrelSpawner.direction;
        float angle = Random.Range(0f, barrelSpawner.spawnAngle) * ((windSource.WindDirection.x > 0) ? 1 : -1);
        float range = Random.Range(0f, barrelSpawner.spawnRange);
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
        spawnDir = rotation * spawnDir;
        spawnDir *= range;
        Vector3 location = barrelSpawner.centerPos + spawnDir;
        location.y = Random.Range(minHeight, maxHeight);
        transform.position = location;
        if (_anim) _anim.SetTrigger("Grow");
        _collider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_anim) _anim.SetTrigger("Shrink");
        _collider.enabled = false;
        animating = true;

        Vector3 windDir = windSource.WindDirection;
        Vector3 newDirection = new Vector3(-windDir.x, windDir.y, windDir.z);
        windSource.ChangeWindDirection(newDirection);
    }

    public void OnRoundStart()
    {
        _collider.enabled = false;
        animating = true;
    }

    public void OnDisable()
    {
        if (!GameManager.cleanedUp) OnCleanup();
    }

    public void OnCleanup()
    {
        Debug.Log($"{name}: Unsubscribing in progress...");
        GameManager.Instance.OnRoundStart -= OnRoundStart;
        GameManager.Instance.OnApplicationCleanup -= OnCleanup;
    }
}
