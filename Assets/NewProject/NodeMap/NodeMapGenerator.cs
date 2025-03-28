using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NodeMapGenerator : MonoBehaviour
{
    public class Node
    {
        public NodeType type;
        public string destination;
        public Vector2 Position;
        public List<Node> Children = new List<Node>();
        public Node Parent;
        public bool cleared = false;
        public bool enabled = false;
    }

    private class Connection
    {
        public Node parent;
        public Node child;
        public LineRenderer lineRenderer;
    }

    [Header("Level Settings")]
    public List<string> playableLevels = new List<string>();
    public List<string> eventLevels = new List<string>();
    public string shopLevel;

    [Header("Prefabs and Sprites")]
    public GameObject buttonPrefab;
    public GameObject linePrefab;
    public Sprite playableLevelsSprite;
    public Sprite eventLevelsSprite;
    public Sprite shopLevelSprite;

    [Header("Map Data")]
    public NodeMapData nodeMapData;

    [Header("Camera Settings")]
    public float cameraSpeed = 4.0f;

    private List<Node> nodes = new List<Node>();
    private Dictionary<Node, Button> nodeButtons = new Dictionary<Node, Button>();
    private List<Connection> connections = new List<Connection>();
    private Node lastClearedNode = null;

    void Start()
    {
#if UNITY_EDITOR
        if (nodeMapData == null)
        {
            nodeMapData = Resources.Load<NodeMapData>("NodeMapData");
        }
#else
        NodeMapData loadedData = NodeMapData.LoadFromDisk();
        if (loadedData != null && loadedData.nodes != null && loadedData.nodes.Count > 0)
        {
            nodeMapData = loadedData;
        }
        else if (nodeMapData == null)
        {
            nodeMapData = ScriptableObject.CreateInstance<NodeMapData>();
        }
#endif

        bool shouldRebuild = nodeMapData != null &&
                            nodeMapData.nodes != null &&
                            nodeMapData.nodes.Count > 0;

        if (shouldRebuild)
        {
            RebuildNodesFromData();
        }
        else
        {
            GenerateNodes();
            if (nodeMapData != null)
            {
                SaveMapToData();
            }
        }

        Cursor.visible = true;

        ConnectNodes();
        ArrangeNodes();
        CenterMap();
        EvaluateNodeEnabledStates();
        CreateNodeButtons();
        CreateConnections();
        UpdateUI();
    }

    void RebuildNodesFromData()
    {
        if (nodeMapData == null || nodeMapData.nodes == null) return;

        nodes.Clear();

        for (int i = 0; i < nodeMapData.nodes.Count; i++)
        {
            NodeData data = nodeMapData.nodes[i];
            Node node = new Node
            {
                type = data.type,
                destination = data.destination,
                Position = data.position,
                cleared = data.cleared,
                enabled = data.enabled
            };
            nodes.Add(node);
        }

        for (int i = 0; i < nodeMapData.nodes.Count; i++)
        {
            NodeData data = nodeMapData.nodes[i];
            Node node = nodes[i];

            if (data.parentIndex >= 0 && data.parentIndex < nodes.Count)
            {
                node.Parent = nodes[data.parentIndex];
            }

            foreach (int childIndex in data.childrenIndices)
            {
                if (childIndex >= 0 && childIndex < nodes.Count)
                {
                    node.Children.Add(nodes[childIndex]);
                }
            }
        }
    }

    void SaveMapToData()
    {
        if (nodeMapData != null)
        {
            nodeMapData.nodes = new List<NodeData>();

            for (int i = 0; i < nodes.Count; i++)
            {
                Node node = nodes[i];
                NodeData data = new NodeData
                {
                    type = node.type,
                    destination = node.destination,
                    position = node.Position,
                    cleared = node.cleared,
                    enabled = node.enabled,
                    parentIndex = node.Parent != null ? nodes.IndexOf(node.Parent) : -1,
                    childrenIndices = new List<int>()
                };

                foreach (Node child in node.Children)
                {
                    int index = nodes.IndexOf(child);
                    if (index >= 0)
                    {
                        data.childrenIndices.Add(index);
                    }
                }

                nodeMapData.nodes.Add(data);
            }

            nodeMapData.Save();
        }
    }

    void GenerateNodes()
    {
        nodes.Clear();
        Node startNode = new Node { type = NodeType.PLAYABLE, destination = playableLevels[0] };
        Node endNode = new Node { type = NodeType.PLAYABLE, destination = playableLevels[playableLevels.Count - 1] };
        List<Node> intermediateNodes = new List<Node>();

        for (int i = 1; i < playableLevels.Count - 1; i++)
        {
            intermediateNodes.Add(new Node { type = NodeType.PLAYABLE, destination = playableLevels[i] });
        }

        foreach (string evLevel in eventLevels)
        {
            intermediateNodes.Add(new Node { type = NodeType.EVENT, destination = evLevel });
        }

        if (!string.IsNullOrEmpty(shopLevel))
        {
            Node shopNode = new Node { type = NodeType.SHOP, destination = shopLevel };
            intermediateNodes.Add(shopNode);
        }

        intermediateNodes = intermediateNodes.OrderBy(x => Random.value).ToList();
        nodes.Add(startNode);
        nodes.AddRange(intermediateNodes);
        nodes.Add(endNode);
    }

    void ConnectNodes()
    {
        List<List<Node>> layers = new List<List<Node>>();
        layers.Add(new List<Node> { nodes[0] });
        List<Node> intermediateNodes = nodes.Skip(1).Take(nodes.Count - 2).ToList();
        int numIntermediateLevels = Mathf.CeilToInt(intermediateNodes.Count / 3f);

        for (int i = 0; i < numIntermediateLevels; i++)
        {
            List<Node> layerNodes = new List<Node>();
            int start = i * 3;
            int end = Mathf.Min(start + 3, intermediateNodes.Count);
            for (int j = start; j < end; j++)
            {
                layerNodes.Add(intermediateNodes[j]);
            }
            layers.Add(layerNodes);
        }

        layers.Add(new List<Node> { nodes[nodes.Count - 1] });

        for (int l = 0; l < layers.Count - 1; l++)
        {
            List<Node> currentLayer = layers[l];
            List<Node> nextLayer = layers[l + 1];

            foreach (Node next in nextLayer)
            {
                Node parent = currentLayer[Random.Range(0, currentLayer.Count)];
                next.Parent = parent;
                if (!parent.Children.Contains(next))
                    parent.Children.Add(next);
            }

            foreach (Node current in currentLayer)
            {
                if (current.Children.Count == 0 && nextLayer.Count > 0)
                {
                    Node child = nextLayer[Random.Range(0, nextLayer.Count)];
                    current.Children.Add(child);
                    child.Parent = current;
                }

                int extraConnections = Random.Range(0, 2);
                for (int c = 0; c < extraConnections; c++)
                {
                    Node candidate = nextLayer[Random.Range(0, nextLayer.Count)];
                    if (!current.Children.Contains(candidate))
                    {
                        current.Children.Add(candidate);
                        candidate.Parent = current;
                    }
                }
            }
        }
    }

    void ArrangeNodes()
    {
        float ySpacing = 200f;
        float xSpacing = 300f;
        Dictionary<int, List<Node>> depthLevels = new Dictionary<int, List<Node>>();

        foreach (var node in nodes)
        {
            int depth = GetNodeDepth(node);
            if (!depthLevels.ContainsKey(depth))
                depthLevels[depth] = new List<Node>();
            depthLevels[depth].Add(node);
        }

        foreach (var level in depthLevels)
        {
            int count = level.Value.Count;
            float startX = -((count - 1) * xSpacing) / 2;
            for (int i = 0; i < count; i++)
            {
                level.Value[i].Position = new Vector2(startX + i * xSpacing, level.Key * ySpacing);
            }
        }
    }

    void CenterMap()
    {
        if (nodes.Count > 0 && Camera.main != null)
        {
            Vector3 pos = new Vector3(nodes[0].Position.x, nodes[0].Position.y + 150, Camera.main.transform.position.z);
            Camera.main.transform.position = pos;
        }
    }

    int GetNodeDepth(Node node)
    {
        int depth = 0;
        while (node.Parent != null)
        {
            depth++;
            node = node.Parent;
        }
        return depth;
    }

    void EvaluateNodeEnabledStates()
    {
        if (!nodes.Any(n => n.cleared))
        {
            foreach (var node in nodes)
            {
                node.enabled = (GetNodeDepth(node) == 0);
            }
        }
        else
        {
            Node deepestCleared = nodes.Where(n => n.cleared).OrderByDescending(n => GetNodeDepth(n)).First();
            lastClearedNode = deepestCleared;
            foreach (var node in nodes)
            {
                node.enabled = false;
            }
            foreach (var child in deepestCleared.Children)
            {
                child.enabled = true;
            }
        }
    }

    void CreateNodeButtons()
    {
        foreach (var node in nodes)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, transform);
            buttonObj.GetComponent<RectTransform>().anchoredPosition = node.Position;
            Button btn = buttonObj.GetComponent<Button>();
            Image img = buttonObj.GetComponent<Image>();

            switch (node.type)
            {
                case NodeType.PLAYABLE:
                    img.sprite = playableLevelsSprite;
                    break;
                case NodeType.EVENT:
                    img.sprite = eventLevelsSprite;
                    break;
                case NodeType.SHOP:
                    img.sprite = shopLevelSprite;
                    break;
            }

            string destination = node.destination;
            btn.onClick.AddListener(() => LoadScene(node, destination));
            nodeButtons[node] = btn;
        }
    }

    void CreateConnections()
    {
        float offset = 70f;
        foreach (var node in nodes)
        {
            foreach (var child in node.Children)
            {
                GameObject lineObj = Instantiate(linePrefab, transform);
                LineRenderer line = lineObj.GetComponent<LineRenderer>();
                Vector3 direction = (child.Position - node.Position).normalized;
                Vector3 startPos = new Vector3(node.Position.x, node.Position.y, transform.position.z) + direction * offset;
                Vector3 endPos = new Vector3(child.Position.x, child.Position.y, transform.position.z) - direction * offset;

                line.positionCount = 2;
                line.SetPosition(0, startPos);
                line.SetPosition(1, endPos);
                line.startWidth = 5f;
                line.endWidth = 5f;

                connections.Add(new Connection { parent = node, child = child, lineRenderer = line });
            }
        }
    }

    void UpdateUI()
    {
        EvaluateNodeEnabledStates();
        foreach (var kvp in nodeButtons)
        {
            Node node = kvp.Key;
            Button btn = kvp.Value;
            btn.interactable = node.enabled;

            var image = btn.GetComponent<Image>();
            if (node.cleared)
            {
                image.color = Color.grey;
            }
            else if (node.destination == nodeMapData.lastEnteredLevel)
            {
                image.color = Color.white;
            }
            else if (node.enabled)
            {
                image.color = Color.white;
            }
            else
            {
                image.color = new Color(0.75f, 0.75f, 0.75f, 0.8f);
            }
        }

        foreach (var connection in connections)
        {
            Color lineColor = (connection.parent.cleared && !connection.child.cleared && connection.child.enabled) ? Color.white : Color.grey;

            connection.lineRenderer.startColor = lineColor;
            connection.lineRenderer.endColor = lineColor;
        }
    }

    public void SetLevelEntered(Node node)
    {
        if (nodeMapData != null)
        {
            nodeMapData.lastEnteredLevel = node.destination;
            SaveMapToData();
        }
    }

    void LoadScene(Node pressedNode, string sceneName)
    {
        SetLevelEntered(pressedNode);

        if (pressedNode == nodes.Last())
        {
            if (nodeMapData != null)
            {
                nodeMapData.ClearNodes();
            }
        }
        else
        {
            SaveMapToData();
        }

        SceneManager.LoadScene(sceneName);
    }

    void Update()
    {
        if (lastClearedNode != null && Camera.main != null)
        {
            Vector3 currentPos = Camera.main.transform.position;
            Vector3 targetPos = new Vector3(lastClearedNode.Position.x, lastClearedNode.Position.y + 150, currentPos.z);
            Camera.main.transform.position = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * cameraSpeed);
        }
    }
}