using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

void Awake()
{
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }

    Instance = this;
    DontDestroyOnLoad(gameObject);
}

    void Start()
    {
        ApplySettings();
    }

    public void ApplySettings()
    {
        AudioListener.volume = GetMasterVolume();
        Screen.fullScreen = GetFullscreen();
    }

    // 🎚 MASTER
public void SetMasterVolume(float value)
{
    Debug.Log("Saving Master: " + value);

    PlayerPrefs.SetFloat("MASTER_VOL", value);
    PlayerPrefs.Save();

    AudioListener.volume = value; // 🔥 ADD THIS
}

    public float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat("MASTER_VOL", 1f);
    }

    // 🎵 MUSIC
   public void SetMusicVolume(float value)
{
    PlayerPrefs.SetFloat("MUSIC_VOL", value);
    PlayerPrefs.Save();
}

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat("MUSIC_VOL", 1f);
    }

    // 🔊 SFX
   public void SetSFXVolume(float value)
{
    PlayerPrefs.SetFloat("SFX_VOL", value);
    PlayerPrefs.Save();
}

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat("SFX_VOL", 1f);
    }

    // 🖥 FULLSCREEN
    public void SetFullscreen(bool value)
{
    PlayerPrefs.SetInt("FULLSCREEN", value ? 1 : 0);
    PlayerPrefs.Save();
    Screen.fullScreen = value;
}

    public bool GetFullscreen()
    {
        return PlayerPrefs.GetInt("FULLSCREEN", 1) == 1;
    }
}