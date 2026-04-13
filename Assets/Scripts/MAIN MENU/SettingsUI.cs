using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle fullscreenToggle;

    bool isLoading = false;

    void Start()
{
    masterSlider.onValueChanged.AddListener(OnMasterChanged);
    musicSlider.onValueChanged.AddListener(OnMusicChanged);
    sfxSlider.onValueChanged.AddListener(OnSFXChanged);
    fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
}

    void OnEnable()
    {
        isLoading = true;

        // 🔥 VERY IMPORTANT: no event trigger
        masterSlider.SetValueWithoutNotify(SettingsManager.Instance.GetMasterVolume());
        musicSlider.SetValueWithoutNotify(SettingsManager.Instance.GetMusicVolume());
        sfxSlider.SetValueWithoutNotify(SettingsManager.Instance.GetSFXVolume());
        fullscreenToggle.SetIsOnWithoutNotify(SettingsManager.Instance.GetFullscreen());

        isLoading = false;
    }

    public void OnMasterChanged(float value)
    {
        if (isLoading) return;

        Debug.Log("Saving Master: " + value);
        SettingsManager.Instance.SetMasterVolume(value);
    }

    public void OnMusicChanged(float value)
    {
        if (isLoading) return;

        SettingsManager.Instance.SetMusicVolume(value);
    }

    public void OnSFXChanged(float value)
    {
        if (isLoading) return;

        SettingsManager.Instance.SetSFXVolume(value);
    }

    public void OnFullscreenChanged(bool value)
    {
        if (isLoading) return;

        SettingsManager.Instance.SetFullscreen(value);
    }
}