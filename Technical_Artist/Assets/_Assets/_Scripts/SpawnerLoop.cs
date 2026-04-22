using UnityEngine;

public class spawnerloop : MonoBehaviour
{
    [System.Serializable]
    public class roadchunk
    {
        public Transform root;
        public Transform backpoint;
        public Transform frontpoint;
    }

    [SerializeField] private roadchunk[] movingboddies;
    [SerializeField] private float recycleZ = -30f;

    private void Update()
    {
        LoopBodies();
    }

    private void LoopBodies()
    {
        if (movingboddies == null || movingboddies.Length == 0)
        {
            return;
        }

        int backIndex = GetBackMostIndex();
        int frontIndex = GetFrontMostIndex();

        if (backIndex == frontIndex)
        {
            return;
        }

        roadchunk backChunk = movingboddies[backIndex];
        roadchunk frontChunk = movingboddies[frontIndex];

        if (backChunk.root == null || backChunk.backpoint == null || backChunk.frontpoint == null)
        {
            return;
        }

        if (frontChunk.root == null || frontChunk.backpoint == null || frontChunk.frontpoint == null)
        {
            return;
        }

        if (backChunk.frontpoint.position.z <= recycleZ)
        {
            Vector3 delta = frontChunk.frontpoint.position - backChunk.backpoint.position;
            backChunk.root.position += delta;
        }
    }

    private int GetBackMostIndex()
    {
        int index = 0;
        float minZ = movingboddies[0].frontpoint.position.z;

        for (int i = 1; i < movingboddies.Length; i++)
        {
            if (movingboddies[i].frontpoint.position.z < minZ)
            {
                minZ = movingboddies[i].frontpoint.position.z;
                index = i;
            }
        }

        return index;
    }

    private int GetFrontMostIndex()
    {
        int index = 0;
        float maxZ = movingboddies[0].frontpoint.position.z;

        for (int i = 1; i < movingboddies.Length; i++)
        {
            if (movingboddies[i].frontpoint.position.z > maxZ)
            {
                maxZ = movingboddies[i].frontpoint.position.z;
                index = i;
            }
        }

        return index;
    }
}