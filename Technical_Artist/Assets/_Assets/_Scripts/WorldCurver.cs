using UnityEngine;

[ExecuteAlways]
public class worldcurver : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float curveStart = 28f;
    [SerializeField] private float curveStrength = 0.00055f;

    private static readonly int CurveStrengthID = Shader.PropertyToID("_CurveStrength");
    private static readonly int CurveStartID = Shader.PropertyToID("_CurveStart");

    private void Update()
    {
        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
        {
            return;
        }

        // These are global, same behavior across all curved materials.
        Shader.SetGlobalFloat(CurveStrengthID, curveStrength);
        Shader.SetGlobalFloat(CurveStartID, curveStart);
    }
}