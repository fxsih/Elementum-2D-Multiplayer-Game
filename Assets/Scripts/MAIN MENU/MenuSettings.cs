using UnityEngine;
using UnityEngine.UI;

public class MenuSettings : MonoBehaviour
{
    [Header("UI")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle fullscreenToggle;

    void Start()
    {
        LoadSettings();

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    // ================= SET =================

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    // ================= LOAD =================

    void LoadSettings()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        masterSlider.value = master;
        musicSlider.value = music;
        sfxSlider.value = sfx;
        fullscreenToggle.isOn = fullscreen;

        AudioListener.volume = master;
        Screen.fullScreen = fullscreen;
    }
}