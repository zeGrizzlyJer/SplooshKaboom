using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour, IProjectile
{
    private Vector3 velocity;
    private Vector3 acceleration;
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
    }

    private void FixedUpdate()
    {
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = currentPosition + velocity * Time.fixedDeltaTime + 0.5f * acceleration * Mathf.Pow(Time.fixedDeltaTime, 2);
        Vector3 newAcceleration = CustomGravity.GetGravity(newPosition);

        transform.position = newPosition;
        velocity = velocity + 0.5f * (acceleration + newAcceleration) * Time.fixedDeltaTime;
        acceleration = newAcceleration;
    }
    public void Launch(Vector3 pos, Quaternion rot, float power)
    {
        transform.position = pos;
        transform.rotation = rot;
        gameObject.SetActive(true);
        velocity = gameObject.transform.forward * power;
        acceleration = CustomGravity.GetGravity(transform.position);
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
        if (other.GetComponent<AirRing>() != null) return;
        Deactivate();
    }
}
