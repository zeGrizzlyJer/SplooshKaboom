using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TornadoSource))]
public class TornadoController : MonoBehaviour, IRequireCleanup
{
    [SerializeField] private BarrelSpawner barrelSpawner; // For convenient position finding
    private TornadoSource tornado;
    private ParticleSystem[] pfxSystems;
    private MeshRenderer[] renderers;
    private float[] pfxOriginalDurations;
    private Material[] materials;
    private float[] matOriginalDissolve;
    [SerializeField] private float dissolveInTimer = 1f;
    [SerializeField] private float dissolveOutTimer = 1f;
    public Vector3 target;
    public float speed;
    public string dissolveID = "_Dissolve";
    [HideInInspector] public int previousCount = 0;

    private float[] targetDissolveValue;
    private float[] previousDissolveValue;
    private float targetTimer;
    private bool dissolveInEffect;
    private float timer = 0f;

    private void Awake()
    {
        tornado = GetComponent<TornadoSource>();
        pfxSystems = GetComponentsInChildren<ParticleSystem>();
        PrimeMaterials();
        pfxOriginalDurations = new float[pfxSystems.Length];
        for (int i = 0; i < pfxSystems.Length; i++)
        {
            pfxOriginalDurations[i] = pfxSystems[i].main.startLifetime.constant;
        }
        FadeOut();
        if (barrelSpawner) target = barrelSpawner.GetAccessiblePosition();
        GameManager.Instance.OnRoundStart += FadeOut;
        GameManager.Instance.OnBarrelValueChange += FadeIn;
        GameManager.Instance.OnApplicationCleanup += OnCleanup;
    }

    private void Update()
    {
        if (!dissolveInEffect) return;
        float t = timer / targetTimer;
        float dissolveValue;
        for (int i = 0; i < materials.Length; i++)
        {
            dissolveValue = Mathf.Lerp(previousDissolveValue[i], targetDissolveValue[i], t);
            materials[i].SetFloat(dissolveID, dissolveValue);
        }
        timer += Time.deltaTime;

        if (timer > targetTimer)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetFloat(dissolveID, targetDissolveValue[i]);
            }
            dissolveInEffect = false;
        }
    }

    private void FixedUpdate()
    {
        if (!barrelSpawner || speed == 0) return;

        Vector3 direction = target - transform.position;
        float distance = direction.magnitude;
        if (distance <= 1f)
        {
            target = barrelSpawner.GetAccessiblePosition();
            direction = target - transform.position;
        }
        Vector3 velocity = direction.normalized * speed;

        transform.position = transform.position + velocity * Time.fixedDeltaTime;
    }


    public void FadeIn(int barrelsRemaining)
    {
        int spawnAmount = GetSpawnAmount();
        if (spawnAmount <= 0 || barrelsRemaining != spawnAmount || previousCount <= barrelsRemaining)
        {
            previousCount = barrelsRemaining;
            return;
        }
        tornado.enabled = true;
        for(int i = 0; i < pfxSystems.Length; i++)
        {
            var main = pfxSystems[i].main;
            main.startLifetime = pfxOriginalDurations[i];
        }
        for(int i = 0; i < previousDissolveValue.Length; i++)
        {
            previousDissolveValue[i] = 1f;
        }
        for (int i = 0; i < targetDissolveValue.Length; i++)
        {
            targetDissolveValue[i] = matOriginalDissolve[i];
        }
        dissolveInEffect = true;
        targetTimer = dissolveInTimer;
        timer = 0f;
        previousCount = barrelsRemaining;
    }

    public void FadeOut()
    {
        tornado.enabled = false;
        foreach (ParticleSystem pfx in pfxSystems)
        {
            var main = pfx.main;
            main.startLifetime = 0f;
        }
        for (int i = 0; i < targetDissolveValue.Length; i++)
        {
            targetDissolveValue[i] = 1f;
        }
        for (int i = 0; i < previousDissolveValue.Length; i++)
        {
            previousDissolveValue[i] = materials[i].GetFloat(dissolveID);
        }
        dissolveInEffect = true;
        targetTimer = dissolveOutTimer;
        timer = 0f;
    }

    private int GetSpawnAmount()
    {
        int spawnBarrel = 3;
        spawnBarrel += GameManager.Instance.GameRound % 2;
        return GameManager.Instance.maxBarrels - spawnBarrel;
    }

    private void PrimeMaterials()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
        int count = 0;
        foreach(MeshRenderer mesh in renderers)
        {
            count += mesh.materials.Length;
        }
        materials = new Material[count];
        matOriginalDissolve = new float[count];
        targetDissolveValue = new float[count];
        previousDissolveValue = new float[count];
        for(int i = 0; i < renderers.Length; i++)
        {
            for (int j = 0; j < renderers[i].materials.Length; j++)
            {
                materials[i + j] = renderers[i].materials[j];
                matOriginalDissolve[i + j] = materials[i + j].GetFloat(dissolveID);
                Debug.Log("Prime: " + matOriginalDissolve[i + j]);
            }
        }
    }

    #region Cleanup
    public void OnDisable()
    {
        if (!GameManager.cleanedUp) OnCleanup();
    }

    public void OnCleanup()
    {
        GameManager.Instance.OnRoundStart -= FadeOut;
        GameManager.Instance.OnBarrelValueChange -= FadeIn;
        GameManager.Instance.OnApplicationCleanup -= OnCleanup;
    }
    #endregion
}
