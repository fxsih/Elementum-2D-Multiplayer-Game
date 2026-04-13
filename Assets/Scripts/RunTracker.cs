using UnityEngine;

public class RunTracker : MonoBehaviour
{
    void Start()
    {
        PlayerStatsManager.AddRun();
        Debug.Log("🎮 Run Started");
    }
}