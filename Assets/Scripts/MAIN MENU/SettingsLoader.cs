using UnityEngine;

public class SettingsLoader : MonoBehaviour
{
    void Start()
    {
        ApplySettings();
    }

    void ApplySettings()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        AudioListener.volume = master;
        Screen.fullScreen = fullscreen;

        Debug.Log("🔊 Settings Applied");
    }
}