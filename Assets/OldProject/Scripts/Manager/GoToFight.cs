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
                    SceneManager.LoadScene("Level2");
                    break;
                case "Level2":
                    SceneManager.LoadScene("Level3");
                    break;
                case "Level3":
                    SceneManager.LoadScene("Level4");
                    break;
                case "Level4":
                    SceneManager.LoadScene("Level5");
                    break;
                case "Level5":
                    SceneManager.LoadScene("Level1");
                    break;
                case "Tutorial":
                    SceneManager.LoadScene("Level1");
                    break;
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
