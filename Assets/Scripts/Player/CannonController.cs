using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CannonController : MonoBehaviour, IRequireCleanup
{
    private GameUI gUI;
    private CannonballPool ballPool;
    private CannonTelemetry telemetry;
    [SerializeField] private GameObject shootEffect;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip unpauseSound;
    [SerializeField] private GameObject shootSpot;
    public GameControls input;
    [SerializeField, Range(-40f, 40f)] private float cannonRotation = 0f;
    [SerializeField, Range(0f, 80f)] private float cannonAngle = 45f;
    [Range(0.01f, 1f)] private float cannonPower = 0.5f;

    [SerializeField] private float rotateSpeed = 1f;
    [SerializeField] private float angleSpeed = 1f;
    [SerializeField] private float powerSpeed = 1f;
    [SerializeField] private float powerModifier = 1f;

    private float rotateDir = 0f;
    private float angleDir = 0f;
    private float powerDir = 0f;

    private void Start()
    {
        telemetry = GetComponent<CannonTelemetry>();
        gUI = GameObject.FindWithTag("GameUI").GetComponent<GameUI>();
        ballPool = GameObject.FindWithTag("BallPool").GetComponent<CannonballPool>();
        GameManager.Instance.OnGameStateChanged += TogglePlayerInput;
        GameManager.Instance.OnApplicationCleanup += OnCleanup;
        input = new GameControls();
        input.Enable();

        #region Delegates
        input.Pause.Activate.performed += ctx => Pause(ctx);
        input.Player.ChangeRotation.performed += ctx => RotateCannon(ctx);
        input.Player.ChangeRotation.canceled += ctx => RotateCannon(ctx);
        input.Player.ChangeAngle.performed += ctx => AngleCannon(ctx);
        input.Player.ChangeAngle.canceled += ctx => AngleCannon(ctx);
        input.Player.ChangePower.performed += ctx => PowerCannon(ctx);
        input.Player.ChangePower.canceled += ctx => PowerCannon(ctx);
        input.Player.Fire.performed += ctx => FireCannon(ctx);
        #endregion
    }

    // This can be optimized later - think of how to only call these functions specifically when needed.
    // It works as is, but... lazy.
    private void FixedUpdate()
    {
        UpdateRotation();
        UpdateAngle();
        UpdatePower();

        gameObject.transform.rotation = Quaternion.Euler(-cannonAngle, cannonRotation, 0);
        telemetry.UpdateTelemetry(shootSpot.transform.position, shootSpot.transform.forward * cannonPower * powerModifier);
    }

    private void UpdatePower()
    {
        // Power Updates
        if (powerDir == 0) return;
        cannonPower += powerDir * powerSpeed;
        cannonPower = Mathf.Clamp(cannonPower, 0.01f, 1f);
        gUI.UpdatePowerDisplay(cannonPower);
    }

    private void UpdateAngle()
    {
        // Angle Updates
        if (angleDir == 0) return;
        cannonAngle += angleDir * angleSpeed;
        cannonAngle = Mathf.Clamp(cannonAngle, 0f, 80f);
        gUI.UpdateAngleDisplay(cannonAngle);
    }

    private void UpdateRotation()
    {
        // Rotation Updates
        if (rotateDir == 0) return;
        cannonRotation += rotateDir * rotateSpeed;
        cannonRotation = Mathf.Clamp(cannonRotation, -40f, 40f);
        gUI.UpdateRotationDisplay(cannonRotation);
    }

    private void OnValidate()
    {
        gameObject.transform.rotation = Quaternion.Euler(-cannonAngle, cannonRotation, 0);
    }

    #region Cleanup
    public void OnDisable()
    {
        if (!GameManager.cleanedUp) OnCleanup();
    }

    public void OnCleanup()
    {
        Debug.Log($"{name}: Unsubscribing in progress...");
        GameManager.Instance.OnGameStateChanged -= TogglePlayerInput;
        GameManager.Instance.OnApplicationCleanup -= OnCleanup;
        input.Pause.Activate.performed -= ctx => Pause(ctx);
        input.Player.ChangeRotation.performed -= ctx => RotateCannon(ctx);
        input.Player.ChangeRotation.canceled -= ctx => RotateCannon(ctx);
        input.Player.ChangeAngle.performed -= ctx => AngleCannon(ctx);
        input.Player.ChangeAngle.canceled -= ctx => AngleCannon(ctx);
        input.Player.ChangePower.performed -= ctx => PowerCannon(ctx);
        input.Player.ChangePower.canceled -= ctx => PowerCannon(ctx);
        input.Player.Fire.performed -= ctx => FireCannon(ctx);
        input.Disable();
    }
    #endregion

    #region Input
    public void Pause(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance.GameState == GameState.PAUSE)
        {
            GameManager.Instance.GameState = GameState.PLAY;
            if (unpauseSound) AudioManager.Instance.Play2DSFX(unpauseSound);
        }
        else GameManager.Instance.GameState = GameState.PAUSE;
    }

    public void RotateCannon(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            rotateDir = 0f;
            return;
        }
        rotateDir = ctx.action.ReadValue<float>();
    }

    public void AngleCannon(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            angleDir = 0f;
            return;
        }
        angleDir = ctx.action.ReadValue<float>();
    }
    public void PowerCannon(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            powerDir = 0f;
            return;
        }
        powerDir = ctx.action.ReadValue<float>();
    }

    public void FireCannon(InputAction.CallbackContext ctx)
    {
        if (!GameManager.Instance.canShoot || GameManager.Instance.ShotsRemaining == 0 || cannonPower <= 0.01f) return;
        if (shootEffect)
        {
            Quaternion rotation = transform.rotation;
            GameObject temp = Instantiate(shootEffect, shootSpot.transform.position, rotation);
            Destroy(temp, 1.8f);
        }
        if (shootSound)
        {
            AudioManager.Instance.Play3DAudio(shootSound, shootSpot.transform.position);
        }
        GameManager.Instance.canShoot = false;
        GameManager.Instance.ShotsRemaining -= 1;
        ballPool.Launch(shootSpot.transform.position, transform.rotation, cannonPower * powerModifier);
    }
    #endregion

    #region Input Activating
    public void TogglePlayerInput()
    {
        if (GameManager.Instance.GameState == GameState.PLAY) input.Player.Enable();
        else input.Player.Disable();
    }

    public void EnablePauseInput()
    {
        input.Pause.Enable();
    }

    public void DisablePauseInput()
    {
        input.Pause.Disable();
    }
    #endregion
}
