using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum NodeType { PLAYABLE, EVENT, SHOP }

[Serializable]
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

[Serializable]
public class NodeMapDataWrapper
{
    public List<NodeData> nodes;
}

[CreateAssetMenu(fileName = "NewNodeMapData", menuName = "NodeMap/Map Data")]
public class NodeMapData : ScriptableObject
{
    public List<NodeData> nodes = new List<NodeData>();

    public void Save()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#else
        SaveToDisk();
#endif
    }

    public void SaveToDisk()
    {
        try
        {
            NodeMapDataWrapper wrapper = new NodeMapDataWrapper { nodes = this.nodes };
            string json = JsonUtility.ToJson(wrapper, true);
            string path = Application.persistentDataPath + "/NodeMapData.json";
            File.WriteAllText(path, json);
        }
        catch (Exception)
        {
            // Error
        }
    }

    public static NodeMapData LoadFromDisk()
    {
        string path = Application.persistentDataPath + "/NodeMapData.json";

        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                NodeMapDataWrapper wrapper = JsonUtility.FromJson<NodeMapDataWrapper>(json);
                NodeMapData data = CreateInstance<NodeMapData>();
                data.nodes = new List<NodeData>();

                if (wrapper != null && wrapper.nodes != null)
                {
                    data.nodes = wrapper.nodes;
                }

                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        return null;
    }

    public void ClearNodes()
    {
        nodes.Clear();
        Save();
    }
}