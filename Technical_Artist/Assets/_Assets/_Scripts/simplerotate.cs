using UnityEngine;

public class simplerotate : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private Vector3 rotationAxis = new Vector3(0f, 1f, 0f);
    [SerializeField] private float fullRotationDuration = 20f;
    [SerializeField] private bool rotateAutomatically = true;

    private void Update()
    {
        if (!rotateAutomatically)
        {
            return;
        }

        if (fullRotationDuration <= 0.01f)
        {
            fullRotationDuration = 0.01f;
        }

        float rotationSpeed = 360f / fullRotationDuration;
        transform.Rotate(rotationAxis.normalized * rotationSpeed * Time.deltaTime, Space.Self);
    }
}