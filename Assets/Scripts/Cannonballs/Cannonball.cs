using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour, IProjectile
{
    Vector3 velocity;
    [SerializeField] private float minHeight = -5f;
    [SerializeField] private GameObject missEffect;
    [SerializeField] private AudioClip missSound;
    [SerializeField] private AudioClip waterSound;

    private void Update()
    {
        if (transform.position.y <= minHeight)
        {
            Sploosh();
            Deactivate();
            LoseGame();
        }

        Vector3 currentVel = velocity;
        currentVel += CustomGravity.GetGravity(transform.position) * Time.deltaTime;
        velocity = currentVel;
    }

    private void FixedUpdate()
    {
        transform.position = transform.position + velocity * Time.fixedDeltaTime;
    }
    public void Launch(Vector3 pos, Quaternion rot, float power)
    {
        transform.position = pos;
        transform.rotation = rot;
        gameObject.SetActive(true);

        velocity = gameObject.transform.forward * power;
    }

    public void Sploosh()
    {
        if (missEffect)
        {
            Vector3 spawnPoint = transform.position;
            spawnPoint.y = 1.2f;
            Quaternion rotation = Quaternion.Euler(-90f, 0, 0);
            GameObject temp = Instantiate(missEffect, spawnPoint, rotation);
            Destroy(temp, 3f);
        }
        if (missSound)
        {
            AudioManager.Instance.Play2DSFX(missSound);   
        }
        if (waterSound)
        {
            AudioManager.Instance.Play3DAudio(waterSound, transform.position);
        }
    }

    public void Deactivate()
    {
        GameManager.Instance.canShoot = true;
        gameObject.SetActive(false);
    }

    private void LoseGame()
    {
        if (GameManager.Instance.BarrelsRemaining != 0 && GameManager.Instance.ShotsRemaining <= 0)
        {
            GameManager.Instance.GameState = GameState.LOSE;
            SceneTransitionManager.Instance.LoadScene(3);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Deactivate();
    }
}
