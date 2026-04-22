using UnityEngine;

public class carhitbox : MonoBehaviour
{
    [SerializeField] private CarController carController;
    [SerializeField] private string obstacleLayerName = "obstacle";

    private int obstacleLayer = -1;

    private void Reset()
    {
        if (carController == null)
        {
            carController = GetComponentInParent<CarController>();
        }
    }

    private void Awake()
    {
        if (carController == null)
        {
            carController = GetComponentInParent<CarController>();
        }

        obstacleLayer = LayerMask.NameToLayer(obstacleLayerName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (carController == null || other == null)
        {
            return;
        }

        bool isObstacle =
            other.gameObject.layer == obstacleLayer ||
            other.transform.root.gameObject.layer == obstacleLayer;

        if (isObstacle)
        {
            carController.ForceCrash();
        }
    }
}