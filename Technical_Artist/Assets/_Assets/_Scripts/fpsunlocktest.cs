using UnityEngine;
using QualitySettings = UnityEngine.QualitySettings;

public class fpsunlocktest : MonoBehaviour
{
    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        UnityEngine.Application.targetFrameRate = 120;

        UnityEngine.Debug.Log("targetFrameRate: " + UnityEngine.Application.targetFrameRate);
        UnityEngine.Debug.Log("vSyncCount: " + QualitySettings.vSyncCount);
        UnityEngine.Debug.Log("refreshRate: " + Screen.currentResolution.refreshRateRatio.value);
    }
}