using System.Collections.Generic;
using UnityEngine;

public enum NodeType { PLAYABLE, EVENT, SHOP }

[CreateAssetMenu(fileName = "NewNodeMapData", menuName = "ScriptableObjects/Map Data")]
public class NodeMapData : ScriptableObject
{
    [System.Serializable]
    public class NodeData
    {
        public NodeType type;
        public string destination;
        public Vector2 position;
        public int parentIndex = -1;
        public List<int> childrenIndices = new List<int>();
        public bool cleared;
        public bool enabled;
    }

    public List<NodeData> nodes = new List<NodeData>();
}