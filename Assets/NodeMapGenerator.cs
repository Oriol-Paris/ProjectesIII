using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NodeMapGenerator : MonoBehaviour
{
    public enum NodeType { PLAYABLE, EVENT, SHOP };

    public class Node
    {
        public NodeType type;
        public string destination;
        public Vector2 Position;
        public List<Node> Children = new List<Node>();
        public Node Parent;
    }

    public List<string> playableLevels = new List<string>();
    public List<string> eventLevels = new List<string>();
    public string shopLevel;

    public GameObject buttonPrefab;

    private List<Node> nodes = new List<Node>();

    public Sprite playableLevelsSprite;
    public Sprite eventLevelsSprite;
    public Sprite shopLevelSprite;

    public GameObject linePrefab;

    void Start()
    {
        GenerateNodes();

        ConnectNodes();

        ArrangeNodes();

        CenterMap();

        CreateNodeButtons();

        DrawConnections();
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

        foreach (string eventLevel in eventLevels)
        {
            intermediateNodes.Add(new Node { type = NodeType.EVENT, destination = eventLevel });
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
            Vector3 pos = new Vector3(nodes[0].Position.x, nodes[0].Position.y, Camera.main.transform.position.z);
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
            btn.onClick.AddListener(() => LoadScene(destination));
        }
    }

    void DrawConnections()
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

                line.material.color = Color.white;
            }
        }
    }

    void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}