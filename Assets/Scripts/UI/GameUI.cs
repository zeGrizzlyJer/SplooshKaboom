using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour, IRequireCleanup
{
    [SerializeField] private float displayTime = 4f;

    [Header("Text Fields")]
    public TMP_Text barrelCounter;
    public TMP_Text shotCounter;
    public TMP_Text powerDisplay;
    public TMP_Text angleDisplay;
    public TMP_Text rotationDisplay;
    public TMP_Text roundDisplay;

    [Header("Iconography")]
    public Slider powerSlider;
    public Image angleIcon;
    public Image rotationIcon;

    private void Awake()
    {
        GameManager.Instance.OnCannonShot += UpdateShotCounter;
        GameManager.Instance.OnBarrelValueChange += UpdateBarrelCounter;
        GameManager.Instance.OnRoundStart += UpdateRoundDisplay;
        GameManager.Instance.OnApplicationCleanup += OnCleanup;
    }

    public void OnDisable()
    {
        if (!GameManager.cleanedUp) OnCleanup();
    }

    public void OnCleanup()
    {
        Debug.Log($"{name}: Unsubscribing in progress...");
        GameManager.Instance.OnCannonShot -= UpdateShotCounter;
        GameManager.Instance.OnBarrelValueChange -= UpdateBarrelCounter;
        GameManager.Instance.OnRoundStart -= UpdateRoundDisplay;
        GameManager.Instance.OnApplicationCleanup -= OnCleanup;
    }

    public void UpdateRoundDisplay()
    {
        if (GameManager.Instance.GameRound > GameManager.Instance.roundsToWin) return;
        roundDisplay.text = "ROUND " + GameManager.Instance.GameRound.ToString();
        StartCoroutine(ShowDisplayForSeconds());
    }

    IEnumerator ShowDisplayForSeconds()
    {
        roundDisplay.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        roundDisplay.gameObject.SetActive(false);
    }

    public void UpdateBarrelCounter(int value)
    {
        barrelCounter.text = value.ToString() + " / " + GameManager.Instance.maxBarrels.ToString();
    }

    public void UpdateShotCounter(int value)
    {
        shotCounter.text = value.ToString();
    }

    public void UpdateAngleDisplay(float value)
    {
        angleDisplay.text = ((int)value).ToString() + "*";
        angleIcon.rectTransform.localRotation = Quaternion.Euler(0, 0, value);
    }

    public void UpdatePowerDisplay(float value)
    {
        powerDisplay.text = ((int)(value * 100)).ToString();
        powerSlider.value = value;
    }

    public void UpdateRotationDisplay(float value)
    {
        rotationDisplay.text = ((int)value).ToString() + "*";
        rotationIcon.rectTransform.localRotation = Quaternion.Euler(0, 0, value - 90);
    }
}
