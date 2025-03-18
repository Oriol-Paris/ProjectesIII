using System.IO;
using UnityEngine;

public class NewGameButton : MonoBehaviour
{
    private const string LEVEL_START_PATH = "/PlayerDataLevelStart.json";
    private const string PLAYERDATA_START_PATH = "/PlayerData.json";
    private const string NODEMAP_START_PATH = "/NodeMapData.json";

    [SerializeField] private PlayerData playerData;

    public void NewGame()
    {
        string levelStartPath = Application.persistentDataPath + LEVEL_START_PATH;
        string playerDataPath = Application.persistentDataPath + PLAYERDATA_START_PATH;
        string nodeMapPath = Application.persistentDataPath + NODEMAP_START_PATH;

        File.Delete(levelStartPath);
        File.Delete(playerDataPath);
        File.Delete(nodeMapPath);

        playerData.LoadOriginalPlayer();
    }
}