using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToFight : MonoBehaviour
{
    public void NextRound()
    {
        PlayerPrefs.SetString("LastLevelCleared", SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("Node Map 2.0");
    }
}
