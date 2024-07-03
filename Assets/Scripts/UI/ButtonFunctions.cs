using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour
{
    #region UI Elements
    [SerializeField] Menu menu;
    [SerializeField] Button startBtn;
    [SerializeField] Button settingsBtn;
    [SerializeField] Button backBtn;
    [SerializeField] Button quitBtn;
    [SerializeField] Button resumeBtn;
    [SerializeField] Button menuBtn;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    #endregion
    #region SFX
    [SerializeField] AudioClip startGameSound;
    [SerializeField] AudioClip clickSound;
    [SerializeField] AudioClip hoverSound;
    [SerializeField] AudioClip returnSound;
    [SerializeField] AudioClip resumeSound;
    #endregion

    public float ButtonTimer = 1f;
    private float btnTimer;

    private void Start()
    {
        List<Button> buttons = new List<Button>();

        if (startBtn)
        {
            startBtn.onClick.AddListener(StartGame);
            buttons.Add(startBtn);
        }
        if (settingsBtn)
        {
            settingsBtn.onClick.AddListener(OpenSettings);
            buttons.Add(settingsBtn);
        }
        if (backBtn)
        {
            backBtn.onClick.AddListener(OpenMenu);
            buttons.Add(backBtn);
        }
        if (quitBtn)
        {
            quitBtn.onClick.AddListener(QuitGame);
            buttons.Add(quitBtn);
        }
        if (resumeBtn)
        {
            resumeBtn.onClick.AddListener(ResumeGame);
            buttons.Add(resumeBtn);
        }
        if (menuBtn)
        {
            menuBtn.onClick.AddListener(GoToMenu);
            buttons.Add(menuBtn);
        }
        if (masterSlider)
        {
            masterSlider.onValueChanged.AddListener(MasterVolumeCallback);
            masterSlider.value = PlayerPrefs.GetFloat("MasterVol", 1f);
            MasterVolumeCallback(masterSlider.value);
        }
        if (musicSlider)
        {
            musicSlider.onValueChanged.AddListener(MusicVolumeCallback);
            musicSlider.value = PlayerPrefs.GetFloat("MusicVol", 0.2f);
            MusicVolumeCallback(musicSlider.value);
        }
        if (sfxSlider)
        {
            sfxSlider.onValueChanged.AddListener(SFXVolumeCallback);
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 0.5f);
            SFXVolumeCallback(sfxSlider.value);
        }

        foreach (Button button in buttons)
        {
            EventTrigger trigger = button.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = button.gameObject.AddComponent<EventTrigger>();
            }
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entry.callback.AddListener((data) => { OnButtonHovered(); });
            trigger.triggers.Add(entry);
        }
    }

    private void Update()
    {
        if (btnTimer < ButtonTimer) btnTimer = Mathf.Clamp(btnTimer + Time.deltaTime, 0, ButtonTimer);
    }

    public void OnButtonHovered()
    {
        if (hoverSound && btnTimer >= ButtonTimer)
        {
            AudioManager.Instance.Play2DSFX(hoverSound);
            btnTimer = 0f;
        }
    }

    private void GoToMenu()
    {
        if (clickSound) AudioManager.Instance.Play2DSFX(clickSound);
        SceneTransitionManager.Instance.LoadScene(0);
    }

    private void SFXVolumeCallback(float value)
    {
        AudioManager.Instance.SetMixerVolume("SFXVol", value);
    }

    private void MusicVolumeCallback(float value)
    {
        AudioManager.Instance.SetMixerVolume("MusicVol", value);
    }

    private void MasterVolumeCallback(float value)
    {
        AudioManager.Instance.SetMixerVolume("MasterVol", value);
    }

    private void ResumeGame()
    {
        if (resumeSound) AudioManager.Instance.Play2DSFX(resumeSound);
        GameManager.Instance.GameState = GameState.PLAY;
    }

    private void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    private void OpenMenu()
    {
        if (returnSound) AudioManager.Instance.Play2DSFX(returnSound);
        menu.MenuState = MenuState.MAIN;
    }

    private void OpenSettings()
    {
        if (clickSound) AudioManager.Instance.Play2DSFX(clickSound);
        menu.MenuState = MenuState.SETTINGS;
    }

    private void StartGame()
    {
        if (startGameSound) AudioManager.Instance.Play2DSFX(startGameSound);
        SceneTransitionManager.Instance.LoadScene(1);
    }
}
