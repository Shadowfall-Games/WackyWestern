using UnityEngine;

public class FPSUnlocker : MonoBehaviour
{
    [SerializeField] private int _targetFPS = 165;

    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = _targetFPS;
    }
}