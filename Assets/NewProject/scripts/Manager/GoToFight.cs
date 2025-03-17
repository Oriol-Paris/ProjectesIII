using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToFight : MonoBehaviour
{
    [SerializeField] NodeMapData nodeMapData;

    public void NextRound()
    {
        nodeMapData.SetLevelCleared(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("Node Map");
    }
}
