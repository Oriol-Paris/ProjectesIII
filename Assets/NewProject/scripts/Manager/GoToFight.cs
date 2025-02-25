using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToFight : MonoBehaviour
{
    public void NextRound()
    {
        var playerData = FindAnyObjectByType<PlayerBase>().playerData;

        if (playerData.levelCompleted)
        {
            switch (playerData.lastLevel)
            {
                case "Level1":
                    SceneManager.LoadScene("Node Map");
                    break;
                case "Level2":
                    SceneManager.LoadScene("Node Map"); break;
                case "Level3":
                    SceneManager.LoadScene("Node Map"); break;
                case "Level4":
                    SceneManager.LoadScene("Node Map"); break;
                case "Level5":
                    SceneManager.LoadScene("Node Map"); break;
                case "Tutorial":
                    SceneManager.LoadScene("Node Map"); break;
            }
        }
        else
        {
            if (playerData.lastLevel == string.Empty || playerData.lastLevel == "Tutorial")
            {
                SceneManager.LoadScene("Level1");
            }
            else
            {
                SceneManager.LoadScene(playerData.lastLevel);
            }
        }
    }
}
