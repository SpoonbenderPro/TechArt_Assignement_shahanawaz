using UnityEngine;

public class trafficloop : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private float respawnOffset = 80f;
    [SerializeField] private float recycleDistance = 20f;

    private void Update()
    {
        if (followTarget == null)
        {
            return;
        }

        if (transform.position.z < followTarget.position.z - recycleDistance)
        {
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                followTarget.position.z + respawnOffset
            );
        }
    }
}