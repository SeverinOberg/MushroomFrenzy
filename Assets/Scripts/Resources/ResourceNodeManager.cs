using System.Collections;
using UnityEngine;

public class ResourceNodeManager : MonoBehaviour
{
    [SerializeField] private ResourceNodeData[] resourceNodeData;

    private ResourceNode[] resourceNodes;
    private int activeCount;

    [SerializeField] private int maxActiveNodes = 12;

    [SerializeField] private int minRespawnTime = 60;
    [SerializeField] private int maxRespawnTime = 120;

    public static System.Action OnNodeGathered;

    private void Start()
    {
        resourceNodes = GetComponentsInChildren<ResourceNode>(true);
        InitializeRandomResourceNodes();
    }

    private void OnEnable()
    {
        OnNodeGathered += OnNodeGatheredCallback;
    }

    private void OnDisable()
    {
        OnNodeGathered -= OnNodeGatheredCallback;
    }

    private void InitializeRandomResourceNodes()
    {
        while (activeCount < maxActiveNodes)
        {
            InitializeRandomResourceNode();
        }
    }

    private void InitializeRandomResourceNode()
    {
        if (activeCount >= resourceNodes.Length)
            return;

        ResourceNode randomNode = null;
        bool foundInactiveNode = false;
        while (!foundInactiveNode)
        {
            int randomIndex = Random.Range(0, resourceNodes.Length);
            randomNode = resourceNodes[randomIndex];
            if (!randomNode.gameObject.activeSelf)
            {
                foundInactiveNode = true;
            }
        }

        // Initialize a random inactive resource node child with random resource data
        randomNode.Initialize(resourceNodeData[Random.Range(0, resourceNodeData.Length)]);
        activeCount++;
    }

    private void OnNodeGatheredCallback()
    {
        activeCount--;
        StartCoroutine(InitializeRandomResourceNodeAfterSeconds(GetRandomRespawnTime()));
    }

    private IEnumerator InitializeRandomResourceNodeAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        InitializeRandomResourceNode();
    }

    private float GetRandomRespawnTime()
    {
        return Random.Range(minRespawnTime, maxRespawnTime);
    }

}
